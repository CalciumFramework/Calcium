#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 13:00:23Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Calcium.StatePreservation
{
	/// <summary>
	/// This class provides a component to save and restore
	/// its state. A component may register its properties
	/// for state preservation, either by using 
	/// <see cref="RegisterStatefulProperty{T}"/>
	/// or by decorating properties with <see cref="StatefulAttribute"/>.
	/// It is the responsibility of a component, 
	/// such as the <see cref="IStateManager"/> implementation,
	/// to signal to the owner of the <c>StatePreservationStrategy</c>
	/// that state should be saved or loaded. A view-model may
	/// implement <see cref="IStateful"/> to indicate that it supports
	/// state preservation.
	/// </summary>
	public class StatePreservationStrategy
    {
	    readonly Type ownerType;
		ViewState viewState;
		readonly object viewStateCreationLock = new object();

		public StatePreservationStrategy(Type ownerType)
	    {
		    this.ownerType = AssertArg.IsNotNull(ownerType, nameof(ownerType));
	    }

		/// <summary>
		/// If <c>true</c> this object will have automatic state persistence applied.
		/// If <c>false</c> no state persistence is performed and state attributes 
		/// are not read. 
		/// The default value is <c>true</c>.
		/// </summary>
		public bool StatePreservationEnabled { get; set; } = true;

		/// <summary>
		/// Enables reading of the <see cref="StatefulAttribute"/>
		/// for class properties.
		/// Setting this property to <c>false</c> may improve performance
		/// because there is a cost to reading the state attributes for the object.
		/// The default value is <c>true</c>.
		/// </summary>
		public bool StatefulAttributesEnabled { get; set; } = true;

		public void DeregisterStatefulProperty(
			string propertyName,
			ApplicationStateType? applicationStateType)
		{
			AssertArg.IsNotNull(propertyName, nameof(propertyName));

			if (!StatePreservationEnabled)
			{
				throw new InvalidOperationException(
					"StatePreservationEnabled must be set to true "
					+ "to de-register a stateful property.");
			}

			EnsureStateAttributesRead();

			viewState.DeregisterState(propertyName, applicationStateType);
		}

		public void RegisterStatefulProperty<T>(
			string stateKey,
			Func<T> propertyGetter,
			Action<T> propertySetter,
			ApplicationStateType applicationStateType)
		{
			if (!StatePreservationEnabled)
			{
				throw new InvalidOperationException(
					"StatePreservationEnabled must be set to true " +
					"to register a stateful property.");
			}

			EnsureStateAttributesRead();

			viewState.RegisterState(
				stateKey, propertyGetter, propertySetter, 
				applicationStateType);
		}

		public void DeregisterState<T>(
			string stateKey, 
			ApplicationStateType? applicationStateType = null)
		{
			if (!StatePreservationEnabled)
			{
				throw new InvalidOperationException(
					"StatePreservationEnabled must be set to true " +
					"to de-register a stateful property.");
			}

			EnsureStateAttributesRead();

			viewState.DeregisterState(stateKey, applicationStateType);
		}

		public void LoadState(
			IDictionary<string, object> persistentStateDictionary,
			IDictionary<string, object> transientStateDictionary,
			bool shouldLoadTransientState)
		{
			EnsureStateAttributesRead();

			var args = new StateLoadEventArgs(
							persistentStateDictionary, 
							transientStateDictionary, 
							shouldLoadTransientState);
			OnStateLoading(args);

			viewState?.LoadPersistentState(persistentStateDictionary);
			if (shouldLoadTransientState)
			{
				viewState?.LoadTransientState(transientStateDictionary);
			}

			OnStateLoaded(args);
		}

		public void SaveState(
			IDictionary<string, object> persistentStateDictionary,
			IDictionary<string, object> transientStateDictionary)
		{
			EnsureStateAttributesRead();

			var args = new StateSaveEventArgs(
							persistentStateDictionary, 
							transientStateDictionary);

			OnStateSaving(args);

			viewState?.SavePersistentState(persistentStateDictionary);
			viewState?.SaveTransientState(transientStateDictionary);

			OnStateSaved(args);
		}
		
		static readonly Dictionary<Type, List<StatePropertyInfo>> propertyCache
			= new Dictionary<Type, List<StatePropertyInfo>>();

		class StatePropertyInfo
		{
			public StatePropertyInfo(PropertyInfo info, ApplicationStateType stateType)
			{
				PropertyInfo = info;
				PersistenceType = stateType;
			}

			public PropertyInfo PropertyInfo { get; }
			public ApplicationStateType PersistenceType { get; }
		}

		void ReadStateAttributes()
		{
			List<StatePropertyInfo> properties;

			if (!propertyCache.TryGetValue(ownerType, out properties))
			{
				properties = new List<StatePropertyInfo>();

				var propertyInfos = ownerType.GetTypeInfo().DeclaredProperties.ToList();

				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					var attributes = propertyInfo.GetCustomAttributes(
												typeof(StatefulAttribute), true).ToList();

					if (!attributes.Any())
					{
						continue;
					}

					StatefulAttribute attribute = (StatefulAttribute)attributes[0];
					var persistenceType = attribute.StateType;

					if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
					{
						throw new InvalidOperationException(string.Format(
							"Property {0} must have a getter and a setter to be persisted.",
							propertyInfo.Name));
					}

					properties.Add(new StatePropertyInfo(propertyInfo, persistenceType));
				}

				propertyCache[ownerType] = properties;
			}

			foreach (var statePropertyInfo in properties)
			{

				/* Prevents access to internal closure warning. */
				PropertyInfo info = statePropertyInfo.PropertyInfo;

				viewState.RegisterState(
					info.Name,
					() => info.GetValue(this, null),
					obj => info.SetValue(this, obj, null),
					statePropertyInfo.PersistenceType);
			}
		}

		void EnsureStateAttributesRead()
		{
			if (!StatePreservationEnabled)
			{
				return;
			}

			if (viewState == null)
			{
				lock (viewStateCreationLock)
				{
					if (viewState == null)
					{
						viewState = new ViewState();

						if (StatefulAttributesEnabled)
						{
							ReadStateAttributes();
						}
					}
				}
			}
		}

	    public event EventHandler<StateLoadEventArgs> StateLoading;
		public event EventHandler<StateLoadEventArgs> StateLoaded;
		public event EventHandler<StateSaveEventArgs> StateSaving;
		public event EventHandler<StateSaveEventArgs> StateSaved;

	    protected virtual void OnStateLoading(StateLoadEventArgs e)
	    {
		    StateLoading?.Invoke(this, e);
	    }

		protected virtual void OnStateLoaded(StateLoadEventArgs e)
		{
			StateLoaded?.Invoke(this, e);
		}

		protected virtual void OnStateSaving(StateSaveEventArgs e)
		{
			StateSaving?.Invoke(this, e);
		}

		protected virtual void OnStateSaved(StateSaveEventArgs e)
		{
			StateSaved?.Invoke(this, e);
		}
	}
}
