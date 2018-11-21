#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-09-06 16:53:52Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Codon.Concurrency;
using Codon.Logging;

namespace Codon.ComponentModel
{
	/// <summary>
	/// This class provides an implementation of the <see cref="INotifyPropertyChanged"/>
	/// and <see cref="INotifyPropertyChanging"/> interfaces. 
	/// Extended <see cref="PropertyChangedEventArgs"/> and <see cref="PropertyChangingEventArgs"/>
	/// are used to provides the old value and new value for the property. 
	/// <seealso cref="PropertyChangedEventArgs{TProperty}"/>
	/// <seealso cref="PropertyChangingEventArgs{TProperty}"/>
	/// </summary>
	//[Serializable]
	public sealed class PropertyChangeNotifier 
		: INotifyPropertyChanged, INotifyPropertyChanging,
			ISuspendChangeNotification
	{
		readonly WeakReference ownerWeakReference;
		
		readonly Dictionary<string, PropertyChangedEventArgs> propertyChangedEventArgsCache
			= new Dictionary<string, PropertyChangedEventArgs>();
		readonly Dictionary<string, PropertyChangingEventArgs> propertyChangingEventArgsCache
			= new Dictionary<string, PropertyChangingEventArgs>();

		ISynchronizationContext synchronizationContextUseProperty;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ISynchronizationContext SynchronizationContext
		{
			get => synchronizationContextUseProperty
						?? (synchronizationContextUseProperty
							= Dependency.Resolve<ISynchronizationContext>());
			set => synchronizationContextUseProperty = value;
		}

		/// <summary>
		/// Gets the owner for testing purposes.
		/// </summary>
		/// <value>The owner.</value>
		internal object Owner => ownerWeakReference?.Target;

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="PropertyChangeNotifier"/> class.
		/// </summary>
		/// <param name="owner">The intended sender 
		/// of the <code>PropertyChanged</code> event.</param>
		public PropertyChangeNotifier(object owner) : this(owner, true)
		{
			/* Intentionally left blank. */
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="PropertyChangeNotifier"/> class.
		/// </summary>
		/// <param name="owner">The intended sender 
		/// <param name="useExtendedEventArgs">If <c>true</c> the
		/// generic <see cref="PropertyChangedEventArgs{TProperty}"/>
		/// and <see cref="PropertyChangingEventArgs{TProperty}"/> 
		/// are used when raising events. 
		/// Otherwise, the non-generic types are used, and they are cached 
		/// to decrease heap fragmentation.</param>
		/// of the <code>PropertyChanged</code> event.</param>
		public PropertyChangeNotifier(object owner, bool useExtendedEventArgs)
		{
			AssertArg.IsNotNull(owner, nameof(owner));

			ownerWeakReference = new WeakReference(owner);
			this.useExtendedEventArgs = useExtendedEventArgs;
		}

		#region event PropertyChanged

		//[field: NonSerialized]
#pragma warning disable IDE1006 // Naming Styles
		event PropertyChangedEventHandler propertyChanged;
#pragma warning restore IDE1006 // Naming Styles

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (OwnerDisposed)
				{
					return;
				}
				propertyChanged += value;
			}
			remove
			{
				propertyChanged -= value;

				CleanUp();
			}
		}

		#region Experimental Explicit UI Thread

		bool maintainThreadAffinity = true;

		/// <summary>
		/// Gets or sets a value indicating whether events will be raised 
		/// on the thread of subscription (either the UI or ViewModel layer).
		/// <c>true</c> by default.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if raising events on the thread 
		/// of subscription; otherwise, <c>false</c>.
		/// </value>
		public bool MaintainThreadAffinity
		{
			get => maintainThreadAffinity;
			set => maintainThreadAffinity = value;
		}

		#endregion

		bool blockWhenRaisingEvents = true;

		public bool BlockWhenRaisingEvents
		{
			get => blockWhenRaisingEvents;
			set => blockWhenRaisingEvents = value;
		}

		/// <summary>
		/// Raises the <see cref="E:PropertyChanged"/> event.
		/// If the owner has been GC'd then the event will not be raised.
		/// </summary>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> 
		/// instance containing the event data.</param>
		void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			var owner = ownerWeakReference.Target;
			var eventCopy = propertyChanged;
			if (owner != null && eventCopy != null)
			{
				if (maintainThreadAffinity)
				{
					Exception exception = null;

					var context = SynchronizationContext;

					if (blockWhenRaisingEvents)
					{
						/* TODO: change ui syncronization API to use Boolean blocking and non blocking. */
						
						if (!context.InvokeRequired)
						{
							try
							{
								eventCopy(owner, e);
							}
							catch (Exception ex)
							{
								exception = ex;
							}
						}
						else
						{
							context.PostAsync(() =>
							{
								try
								{
									eventCopy(owner, e);
								}
								catch (Exception ex)
								{
									exception = ex;
								}
							});
						}

						if (exception != null)
						{
							throw exception;
						}
					}
					else
					{
						context.Post(() =>
						{
							try
							{
								eventCopy(owner, e);
							}
							catch (Exception ex)
							{
								var log = Dependency.Resolve<ILog>();
								log.Error("Exception raised while invoking on UI thread in PropertyChangeNotifier.", ex);
								exception = ex;
							}
						});
					}
				}
				else
				{
					eventCopy(owner, e);
				}
			}
		}

		#endregion

		/// <summary>
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// </summary>
		/// <typeparam name="TField">The type of the backing field.</typeparam>
		/// <param name="propertyName">Name of the property. Can not be null.</param>
		/// <param name="property">A reference to the property that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public AssignmentResult Set<TField>(
			string propertyName, ref TField property, TField newValue)
		{
			if (OwnerDisposed)
			{
				return AssignmentResult.OwnerDisposed;
			}

			AssertArg.IsNotNullOrEmpty(propertyName, nameof(propertyName));
			ValidatePropertyName(propertyName);

			return AssignWithNotification(propertyName, ref property, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// Note: This is new for WP8.
		/// </summary>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="propertyName">Name of the property. Can not be null.</param>
		/// <param name="field">A reference to the property that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public AssignmentResult Set<TField>(
			ref TField field, TField newValue, [CallerMemberName] string propertyName = "")
		{
			var result = Set<TField>(propertyName, ref field, newValue);
			return result;
		}

		/// <summary> 
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// Assignment nor notification will occur if the specified
		/// property and newValue are equal. 
		/// Use this method when a downcast is required.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <typeparam name="TField">The type of the field. 
		/// When assignment occurs, a downcast is applied.</typeparam>
		/// <param name="field">A reference to the property that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <param name="propertyName">The caller member name.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public AssignmentResult Set<TProperty, TField>(
			ref TField field, TField newValue,
			[CallerMemberName] string propertyName = "")
			where TField : TProperty
		{
			if (OwnerDisposed)
			{
				return AssignmentResult.OwnerDisposed;
			}

			return AssignWithNotification<TField>(propertyName, ref field, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified WeakReference field ref
		/// and then notifies listeners that the property has changed.
		/// Assignment nor notification will occur if the specified
		/// property and newValue are equal. 
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="propertyName">The name of the property being changed.</param>
		/// <param name="property">A reference to the property 
		/// that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		internal AssignmentResult Set<TProperty>(
			string propertyName, ref WeakReference property, TProperty newValue) 
			where TProperty : class
		{
			if (OwnerDisposed)
			{
				return AssignmentResult.OwnerDisposed;
			}

			AssertArg.IsNotNullOrEmpty(propertyName, nameof(propertyName));
			ValidatePropertyName(propertyName);

			return AssignWithNotification(propertyName, ref property, newValue);
		}

		AssignmentResult AssignWithNotification<TProperty>(
			string propertyName, ref TProperty property, TProperty newValue)
		{
			bool preliminarilyFoundEqual = true;

			/* Equality of Uri's does not take into account the Uri Fragment component 
			 * and hence we must deal with that special case. */
			if (typeof(TProperty) == typeof(Uri))
			{
				Uri propertyUri = (Uri)(object)property;
				Uri newUri = (Uri)(object)newValue;
				if (propertyUri != null && newUri != null
										&& propertyUri.IsAbsoluteUri
										&& newUri.IsAbsoluteUri /* Fragment throws an InvalidOperationException for relative Uri's. */
										&& propertyUri.Fragment != newUri.Fragment)
				{
					preliminarilyFoundEqual = false;
				}
			}

			if (preliminarilyFoundEqual)
			{
#if WINDOWS_PHONE
/* Hack for GeoCoordinate comparison bug. */
				if (EqualityComparer<TProperty>.Default.Equals(property, newValue))
				{
					return AssignmentResult.AlreadyAssigned;
				}
#else
				/* Boxing may occur here. We should consider 
				 * providing some overloads for primitives. */
				if (Equals(property, newValue))
				{
					return AssignmentResult.AlreadyAssigned;
				}
#endif
			}

			bool notify = !ChangeNotificationSuspended;

			if (useExtendedEventArgs)
			{
				if (notify)
				{
					var args = new PropertyChangingEventArgs<TProperty>(propertyName, property, newValue);

					OnPropertyChanging(args);
					if (args.Cancelled)
					{
						return AssignmentResult.Cancelled;
					}
				}

				var oldValue = property;
				property = newValue;

				if (notify)
				{
					OnPropertyChanged(new PropertyChangedEventArgs<TProperty>(
						propertyName, oldValue, newValue));
				}
			}
			else
			{
				if (notify)
				{
					var args = RetrieveOrCreatePropertyChangingEventArgs(propertyName);
					OnPropertyChanging(args);
				}

				property = newValue;

				if (notify)
				{
					var changedArgs = RetrieveOrCreatePropertyChangedEventArgs(propertyName);
					OnPropertyChanged(changedArgs);
				}
			}

			return AssignmentResult.Success;
		}

		AssignmentResult AssignWithNotification<TProperty>(
			string propertyName, ref WeakReference field, TProperty newValue)
			where TProperty : class
		{
			var typedOldValue = (TProperty)field?.Target;
#if WINDOWS_PHONE
			if (EqualityComparer<TProperty>.Default.Equals(
					typedOldValue, newValue))
			{
				return AssignmentResult.AlreadyAssigned;
			}
#else
			/* Boxing may occur here. We should consider 
			 * providing some overloads for primitives. */
			if (Equals(typedOldValue, newValue))
			{
				return AssignmentResult.AlreadyAssigned;
			}
#endif
			bool notify = !ChangeNotificationSuspended;

			if (useExtendedEventArgs)
			{
				if (notify)
				{
					var args = new PropertyChangingEventArgs<TProperty>(
						propertyName, typedOldValue, newValue);

					OnPropertyChanging(args);
					if (args.Cancelled)
					{
						return AssignmentResult.Cancelled;
					}
				}

				field = newValue != null ? new WeakReference(newValue) : null;

				if (notify)
				{
					OnPropertyChanged(new PropertyChangedEventArgs<TProperty>(
						propertyName, typedOldValue, newValue));
				}
			}
			else
			{
				if (notify)
				{
					var args = RetrieveOrCreatePropertyChangingEventArgs(propertyName);
					OnPropertyChanging(args);
				}

				field = newValue != null ? new WeakReference(newValue) : null;

				if (notify)
				{
					var changedArgs = RetrieveOrCreatePropertyChangedEventArgs(propertyName);
					OnPropertyChanged(changedArgs);
				}
			}

			return AssignmentResult.Success;
		}

		/// <summary>
		/// Notifies listeners that the specified property has changed.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="propertyName">Name of the property. Can not be null.</param>
		/// <param name="oldValue">The old value before the change occurred.</param>
		/// <param name="newValue">The new value after the change occurred.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public void NotifyChanged<TProperty>(
			string propertyName, TProperty oldValue, TProperty newValue)
		{
			if (OwnerDisposed)
			{
				return;
			}

			if (ChangeNotificationSuspended)
			{
				throw new InvalidOperationException("Change notification is suspended.");
			}

			AssertArg.IsNotNullOrEmpty(propertyName, nameof(propertyName));
			ValidatePropertyName(propertyName);

			if (ReferenceEquals(oldValue, newValue))
			{
				return;
			}

			var args = useExtendedEventArgs
				? new PropertyChangedEventArgs<TProperty>(propertyName, oldValue, newValue)
				: RetrieveOrCreatePropertyChangedEventArgs(propertyName);

			OnPropertyChanged(args);
		}
		
		#region INotifyPropertyChanging Implementation

		//[field: NonSerialized]
#pragma warning disable IDE1006 // Naming Styles
		event PropertyChangingEventHandler propertyChanging;
#pragma warning restore IDE1006 // Naming Styles

		public event PropertyChangingEventHandler PropertyChanging
		{
			add
			{
				if (OwnerDisposed)
				{
					return;
				}
				propertyChanging += value;
			}
			remove
			{
				if (OwnerDisposed)
				{
					return;
				}
				propertyChanging -= value;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:PropertyChanging"/> event.
		/// If the owner has been GC'd then the event will not be raised.
		/// </summary>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangingEventArgs"/> 
		/// instance containing the event data.</param>
		void OnPropertyChanging(PropertyChangingEventArgs e)
		{
			var owner = ownerWeakReference.Target;
			if (owner != null)
			{
				propertyChanging?.Invoke(owner, e);
			}
		}
		#endregion

		bool cleanupOccurred;

		bool OwnerDisposed
		{
			get
			{
				/* We slightly improve performance here 
				 * by avoiding multiple Owner property calls 
				 * after the Owner has been disposed. */
				if (cleanupOccurred)
				{
					return true;
				}

				var owner = Owner;
				if (owner != null)
				{
					return false;
				}

				CleanUp();

				return true;
			}
		}

		void CleanUp()
		{
			if (cleanupOccurred || Owner != null)
			{
				return;
			}

			cleanupOccurred = true;

			var changedSubscribers = propertyChanged?.GetInvocationList();
			if (changedSubscribers != null)
			{
				foreach (var subscriber in changedSubscribers)
				{
					propertyChanged -= (PropertyChangedEventHandler)subscriber;
				}
			}

			var changingSubscribers = propertyChanging?.GetInvocationList();
			if (changingSubscribers != null)
			{
				foreach (var subscriber in changingSubscribers)
				{
					propertyChanging -= (PropertyChangingEventHandler)subscriber;
				}
			}

			/* Events should be null at this point. Nevertheless... */
			propertyChanged = null;
			propertyChanging = null;
			propertyChangedEventArgsCache.Clear();
			propertyChangingEventArgsCache.Clear();
		}

		[Conditional("DEBUG")]
		void ValidatePropertyName(string propertyName)
		{
#if !NETSTANDARD && !SILVERLIGHT && !MONODROID && !__IOS__ && !NETFX_CORE
			var propertyDescriptor = TypeDescriptor.GetProperties(Owner)[propertyName];
			if (propertyDescriptor == null)
			{
				/* TODO: Make localizable resource. */
				throw new Exception(string.Format(
					"The property '{0}' does not exist.", propertyName));
			}
#endif
		}

		bool useExtendedEventArgs;

		public bool UseExtendedEventArgs
		{
			get => useExtendedEventArgs;
			set => useExtendedEventArgs = value;
		}

		PropertyChangedEventArgs RetrieveOrCreatePropertyChangedEventArgs(string propertyName)
		{
			AssertArg.IsNotNull(propertyName, nameof(propertyName));
			var result = RetrieveOrCreateArgs(
				propertyName,
				propertyChangedEventArgsCache,
				x => new PropertyChangedEventArgs(x));

			return result;
		}

		static TArgs RetrieveOrCreateArgs<TArgs>(string propertyName, Dictionary<string, TArgs> argsCache,
			Func<string, TArgs> createFunc)
		{
			AssertArg.IsNotNull(propertyName, nameof(propertyName));

			if (argsCache.TryGetValue(propertyName, out TArgs result))
			{
				return result;
			}

			result = createFunc(propertyName);
			argsCache[propertyName] = result;
			return result;
		}

		PropertyChangingEventArgs RetrieveOrCreatePropertyChangingEventArgs(string propertyName)
		{
			AssertArg.IsNotNull(propertyName, nameof(propertyName));

			var result = RetrieveOrCreateArgs(
				propertyName,
				propertyChangingEventArgsCache,
				x => new PropertyChangingEventArgs(x));

			return result;
		}

		public void NotifyChanged(string propertyName)
		{
			if (ChangeNotificationSuspended)
			{
				throw new InvalidOperationException("Change notification is suspended.");
			}

			var args = RetrieveOrCreatePropertyChangedEventArgs(propertyName);
			OnPropertyChanged(args);
		}

		public bool ChangeNotificationSuspended { get; set; }
	}
}