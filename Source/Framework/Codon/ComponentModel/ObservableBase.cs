#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-04-10 20:33:35Z</CreationDate>
</File>
*/
#endregion

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Codon.ComponentModel
{
	/// <summary>
	/// A base class for property change notification.
	/// Automatically raises <c>PropertyChanged</c>
	/// and <c>PropertyChanging</c> events when using
	/// its <see cref="Set{TProperty,TField}"/> methods.
	/// <seealso cref="ComponentModel.PropertyChangeNotifier"/>.
	/// </summary>
	//[Serializable]
	public abstract class ObservableBase
		: INotifyPropertyChanged, INotifyPropertyChanging
	{
		//[field: NonSerialized]
		PropertyChangeNotifier notifier;

		readonly object notifierCreationLock = new object();

		readonly bool useExtendedEventArgs;

		/// <summary>
		/// Gets the notifier. It is lazy loaded.
		/// </summary>
		/// <value>The notifier.</value>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected PropertyChangeNotifier PropertyChangeNotifier
		{
			get
			{
				/* We use lazy instantiation because hooking up the events 
				 * for many instances is expensive. */
				if (notifier == null)
				{
					lock (notifierCreationLock)
					{
						if (notifier == null)
						{
							notifier = new PropertyChangeNotifier(this, useExtendedEventArgs);
						}
					}
				}

				return notifier;
			}
		}

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected virtual void OnPropertyChanged(
			[CallerMemberName]string propertyName = "")
		{
			PropertyChangeNotifier.NotifyChanged(propertyName);
		}

		/// <summary> 
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// Assignment nor notification will occur if the specified
		/// property and newValue are equal. 
		/// Use this method when a downcast is required.
		/// </summary>
		/// <typeparam name="TProperty">
		/// The type of the property.</typeparam>
		/// <typeparam name="TField">The type of the field. 
		/// When assignment occurs, a downcast is applied.</typeparam>
		/// <param name="field">
		/// A reference to the property that is to be assigned.
		/// </param>
		/// <param name="newValue">
		/// The value to assign the property.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.
		/// </exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected AssignmentResult Set<TProperty, TField>(
			ref TField field, TField newValue)
			where TField : TProperty
		{
			return PropertyChangeNotifier.Set<TProperty, TField>(
												ref field, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="propertyName">Name of the property. 
		/// Can not be null.</param>
		/// <param name="field">
		/// A reference to the property that is to be assigned.</param>
		/// <param name="newValue">
		/// The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.
		/// </exception>
		protected AssignmentResult Set<TProperty>(
			string propertyName, ref TProperty field, TProperty newValue)
		{
			return PropertyChangeNotifier.Set(
						propertyName, ref field, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// </summary>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="propertyName">Name of the property. 
		/// Can not be null.</param>
		/// <param name="property">
		/// A reference to the property that is to be assigned.</param>
		/// <param name="newValue">
		/// The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.
		/// </exception>
		protected AssignmentResult Set<TField>(
			ref TField property, TField newValue, 
			[CallerMemberName]string propertyName = "")
		{
			return PropertyChangeNotifier.Set<TField>(
						propertyName, ref property, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified 
		/// WeakReference field ref and then notifies listeners 
		/// that the property has changed.
		/// Assignment nor notification will occur if the specified
		/// property and newValue are equal. 
		/// Uses an <see cref="System.Linq.Expressions.Expression"/> 
		/// to determine the property name, 
		/// which is slower than using the string property name overload.
		/// </summary>
		/// <typeparam name="TProperty">
		/// The type of the property.</typeparam>
		/// <param name="propertyName">
		/// The name of the property being changed.</param>
		/// <param name="fieldReference">
		/// A reference to the field <see cref="WeakReference"/> 
		/// that is to be assigned.</param>
		/// <param name="newValue">
		/// The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.
		/// </exception>
		protected AssignmentResult Set<TProperty>(
			string propertyName, 
			ref WeakReference fieldReference, 
			TProperty newValue)
			where TProperty : class
		{
			return PropertyChangeNotifier.Set(
						propertyName, ref fieldReference, newValue);
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="ObservableBase"/> class.
		/// </summary>
		/// <param name="useExtendedEventArgs">if set to <c>true</c> 
		/// the PropertyChangeNotifier will use extended event args.
		/// Default is <c>true</c>.</param>
		protected ObservableBase(bool useExtendedEventArgs = true)
		{
			this.useExtendedEventArgs = useExtendedEventArgs;
		}

		#region Property change notification

		/// <summary>
		/// Occurs when a property value changes.
		/// <seealso cref="ComponentModel.PropertyChangeNotifier"/>
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				PropertyChangeNotifier.PropertyChanged += value;
			}
			remove
			{
				PropertyChangeNotifier.PropertyChanged -= value;
			}
		}

		/// <summary>
		/// Occurs when a property value is changing.
		/// <seealso cref="ComponentModel.PropertyChangeNotifier"/>
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging
		{
			add
			{
				PropertyChangeNotifier.PropertyChanging += value;
			}
			remove
			{
				PropertyChangeNotifier.PropertyChanging -= value;
			}
		}

		#endregion
	}
}
