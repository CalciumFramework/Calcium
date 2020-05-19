#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-17 15:50:52Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Codon.IO;
using Codon.Logging;

namespace Codon.StatePreservation
{
	public class ViewState
	{
		readonly object transientStateLock = new object();
		readonly Dictionary<string, IStateAccessor> transientState
						= new Dictionary<string, IStateAccessor>();
		readonly object persistentStateLock = new object();
		readonly Dictionary<string, IStateAccessor> persistentState
						= new Dictionary<string, IStateAccessor>();

		public void RegisterState<T>(
			string stateKey,
			Func<T> getterFunc,
			Action<T> setterAction,
			ApplicationStateType stateType)
		{
			AssertArg.IsNotNull(stateKey, nameof(stateKey));
			AssertArg.IsNotNull(getterFunc, nameof(getterFunc));
			AssertArg.IsNotNull(setterAction, nameof(setterAction));

			if (stateType == ApplicationStateType.Persistent)
			{
				lock (persistentStateLock)
				{
					persistentState[stateKey]
						= new Accessor<T>(getterFunc, setterAction);
				}
			}
			else
			{
				lock (transientStateLock)
				{
					transientState[stateKey]
						= new Accessor<T>(getterFunc, setterAction);
				}
			}
		}

		public void DeregisterState(
			string stateKey,
			ApplicationStateType? applicationStateType)
		{
			AssertArg.IsNotNull(stateKey, nameof(stateKey));

			if (applicationStateType.HasValue)
			{
				if (applicationStateType == ApplicationStateType.Persistent)
				{
					DeregisterPersistentState(stateKey);
				}
				else
				{
					DeregisterTransientState(stateKey);
				}
			}
			else
			{
				DeregisterPersistentState(stateKey);
				DeregisterTransientState(stateKey);
			}
		}

		void DeregisterTransientState(string stateKey)
		{
			AssertArg.IsNotNull(stateKey, nameof(stateKey));

			lock (transientStateLock)
			{
				transientState.Remove(stateKey);
			}
		}

		void DeregisterPersistentState(string stateKey)
		{
			AssertArg.IsNotNull(stateKey, nameof(stateKey));

			lock (persistentStateLock)
			{
				persistentState.Remove(stateKey);
			}
		}

		public void SaveTransientState(IDictionary<string, object> stateDictionary)
		{
			SaveState(stateDictionary, transientState, transientStateLock);
		}

		public void SavePersistentState(IDictionary<string, object> stateDictionary)
		{
			SaveState(stateDictionary, persistentState, persistentStateLock);
		}

		public void LoadTransientState(IDictionary<string, object> stateDictionary)
		{
			LoadState(stateDictionary, transientState, transientStateLock);
		}

		public void LoadPersistentState(IDictionary<string, object> stateDictionary)
		{
			LoadState(stateDictionary, persistentState, persistentStateLock);
		}

		protected byte[] Serialize(object value)
		{
			var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
			byte[] state = serializer.Serialize(value);
			return state;
		}

		protected T Deserialize<T>(byte[] data) where T : class
		{
			var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
			T result = serializer.Deserialize<T>(data);
			return result;
		}

		protected object Deserialize(byte[] data)
		{
			var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
			var result = serializer.Deserialize<object>(data);
			return result;
		}

		void SaveState(
				IDictionary<string, object> stateDictionary,
				Dictionary<string, IStateAccessor> accessors,
				object propertiesLock)
		{
			lock (propertiesLock)
			{
				foreach (KeyValuePair<string, IStateAccessor> pair in accessors)
				{
					string stateKey = pair.Key;
					IStateAccessor accessor = pair.Value;

					object accessorValue = accessor.Value;

					if (accessorValue == null)
					{
						stateDictionary.Remove(stateKey);
						continue;
					}

					byte[] bytes;
					try
					{
						bytes = Serialize(accessorValue);
					}
					catch (Exception ex)
					{
						stateDictionary[pair.Key] = null;
						Debug.Assert(false, "Unable to serialize state value. " + ex);
						continue;
					}

					stateDictionary[stateKey] = bytes;
				}
			}
		}

		void LoadState(
				IDictionary<string, object> stateDictionary,
				Dictionary<string, IStateAccessor> accessors,
				object propertiesLock)
		{
			lock (propertiesLock)
			{
				foreach (KeyValuePair<string, IStateAccessor> pair in accessors)
				{
					object stateValue;
					string stateKey = pair.Key;
					IStateAccessor accessor = pair.Value;

					if (!stateDictionary.TryGetValue(stateKey, out stateValue))
					{
						continue;
					}

					byte[] bytes = stateValue as byte[];

					if (bytes == null)
					{
						Debug.Assert(false, "state value is not a byte[]");
						continue;
					}

					object deserializedValue;

					try
					{
						deserializedValue = Deserialize(bytes);
					}
					catch (Exception ex)
					{
						string message = "Unable to deserialize bytes. " + ex;
						var log = Dependency.Resolve<ILog>();
						log.Error(message);
						Debug.Assert(false, message);
						continue;
					}

					if (deserializedValue == null)
					{
						const string message = "Deserialized object should not be null.";
						var log = Dependency.Resolve<ILog>();
						log.Error(message);
						Debug.Assert(false, message);
						continue;
					}

					try
					{
						accessor.Value = deserializedValue;
					}
					catch (Exception ex)
					{
						var log = Dependency.Resolve<ILog>();
						log.Error("Unable to set state value. ", ex);
					}
				}
			}
		}

		class Accessor<T> : IStateAccessor
		{
			readonly Func<T> getter;
			readonly Action<T> setter;

			public Accessor(Func<T> getter, Action<T> setter)
			{
				this.getter = getter;
				this.setter = setter;
			}

			public object Value
			{
				get => getter();
				set => setter((T)value);
			}
		}

		interface IStateAccessor
		{
			object Value { get; set; }
		}
	}
}