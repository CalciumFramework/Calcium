#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-07-30 17:32:22Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Codon.Collections;
using Codon.IO;
using Codon.Reflection;
using Codon.Services;

namespace Codon.SettingsModel
{
	/// <summary>
	/// This is the default implementation of the
	/// <see cref="ISettingsService"/>.
	/// To change where the service stores and retrieves its values,
	/// provide a custom <c>ISettingsStore</c> implementation.
	/// </summary>
	public class SettingsService : ISettingsService
	{
		readonly SettingsEventBroadcaster eventBroadcaster;
		readonly ISettingsStore localStore;
		readonly ISettingsStore transientStore;
		readonly ISettingsStore roamingStore;
		const string dateTimeFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";
		static readonly TypeInfo xmlConvertibleTypeInfo = typeof(IXmlConvertible).GetTypeInfo();

		public bool RaiseExceptionsOnConversionErrors { get; set; }

		public SettingsService(
			ISettingsStore localStore = null, 
			ISettingsStore roamingStore = null, 
			ISettingsStore transientStore = null)
		{
			this.localStore = localStore ?? new LocalSettingsStore();

			this.roamingStore = roamingStore;
			this.transientStore = transientStore ?? new InMemoryTransientSettingsStore();

			eventBroadcaster = new SettingsEventBroadcaster(this);
		}

		/// <summary>
		/// Provides thread safety for the dictionary of settings.
		/// </summary>
		readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

		readonly Dictionary<string, object> cache = new Dictionary<string,object>();

		public void ClearCache()
		{
			lockSlim.EnterWriteLock();
			try
			{
				cache.Clear();
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}
		
		public void RemoveCacheItem(string key)
		{
			string cacheKey = AssertArg.IsNotNull(key, nameof(key));

			lockSlim.EnterWriteLock();
			try
			{
				cache.Remove(cacheKey);
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		public bool RemoveSetting(string key)
		{
			AssertArg.IsNotNull(key, nameof(key));

			bool result = false;

			lockSlim.EnterWriteLock();
			try
			{
				if (localStore.Status == SettingsStoreStatus.Ready)
				{
					result = localStore.Remove(key);
				}

				if (roamingStore != localStore && roamingStore != null && roamingStore.Status == SettingsStoreStatus.Ready)
				{
					result |= roamingStore.Remove(key);
				}

				if (transientStore != null && transientStore.Status == SettingsStoreStatus.Ready)
				{
					result |= transientStore.Remove(key);
				}

				cache.Remove(key);
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}

			if (result)
			{
				OnSettingRemoved(new SettingRemovedEventArgs(key));
			}

			return result;
		}

		public bool TryGetSetting(string key, Type settingType, out object setting)
		{
			setting = GetSettingCore(key, settingType, null, out bool settingExists);
			return settingExists;
		}

		public bool TryGetSetting<TSettingType>(string key, out TSettingType setting)
		{
			setting = (TSettingType)GetSettingCore(
							key, 
							typeof(TSettingType), 
							default(TSettingType), 
							out bool settingExists);

			return settingExists;
		}

		public StorageLocation? GetSettingLocation(string key)
		{
			StorageLocation? result = null;

			lockSlim.EnterReadLock();
			try
			{
				if (localStore.Status == SettingsStoreStatus.Ready && localStore.Contains(key))
				{
					result = StorageLocation.Local;
				}
				else if (localStore != roamingStore && roamingStore.Status == SettingsStoreStatus.Ready && roamingStore.Contains(key))
				{
					result = StorageLocation.Roaming;
				}
				else if (transientStore.Status == SettingsStoreStatus.Ready && transientStore.Contains(key))
				{
					result = StorageLocation.Transient;
				}
			}
			finally
			{
				lockSlim.ExitReadLock();
			}

			return result;
		}

		public T GetSetting<T>(string key, T defaultValue)
		{
			AssertArg.IsNotNull(key, nameof(key));

			return (T)GetSetting(key, typeof(T), defaultValue);
		}

		public object GetSetting(string key, Type settingType, object defaultValue)
		{
			var result = GetSettingCore(
							key, 
							settingType, 
							defaultValue, 
#pragma warning disable 168
							out bool settingExists);
#pragma warning restore 168

			return result;
		}

		object GetSettingCore(string key, Type settingType, object defaultValue, out bool settingExists)
		{
			AssertArg.IsNotNull(key, nameof(key));

			/* [DV] We first test if the setting type is IXmlConvertible. 
			 * If it isn't then it may mean that the type <c>object</c> has been passed in via an overload. 
			 * The type of the return value may be IXmlConvertible and hence we test the default value 
			 * as a secondary precaution. It means that in some edge scenario you may find a type that is passed 
			 * as the default value has implemented IXmlConvertible whereas the stored value doesn't implement it. */
			var reflectionCache = Dependency.Resolve<IReflectionCache>();

			bool xmlConvertible = reflectionCache.IsAssignableFrom(typeof(IXmlConvertible), settingType);
			xmlConvertible |= settingType == typeof(object) && defaultValue is IXmlConvertible;

			bool returningDefaultValue;
			object result;

			lockSlim.EnterReadLock();
			try
			{
				result = GetSettingFromStore(key, xmlConvertible, settingType, defaultValue, out returningDefaultValue);
			}
			finally
			{
				lockSlim.ExitReadLock();
			}

			settingExists = !returningDefaultValue;

			return result;
		}

		object GetSettingFromStore(string key, bool xmlConvertible, 
			Type settingType, object defaultValue, out bool returningDefaultValue)
		{
			returningDefaultValue = false;
			string cacheKey = key;

			if (cache.TryGetValue(cacheKey, out object cacheResult))
			{
				return cacheResult;
			}

			object entry = null;
			bool retrievedExisting = localStore.Status == SettingsStoreStatus.Ready && localStore.TryGetValue(key, settingType, out entry);
			/* Don't use |= here because doing so prevents short circuiting from occurring. */
			retrievedExisting = retrievedExisting || roamingStore != null && roamingStore.Status == SettingsStoreStatus.Ready && roamingStore.TryGetValue(key, settingType, out entry);
			retrievedExisting = retrievedExisting || transientStore != null && transientStore.Status == SettingsStoreStatus.Ready && transientStore.TryGetValue(key, settingType, out entry);

			if (retrievedExisting)
			{
				if (xmlConvertible && entry != null)
				{
					Type concreteType;
					if (settingType.IsInterface())
					{
						if (defaultValue == null)
						{
							throw new SettingsException($"Unable to retrieve IXmlConvertible setting object specified by interface type {settingType.FullName}. "
														+ "Retrieve this value using a concrete implementation type.");
						}

						concreteType = defaultValue.GetType();
					}
					else
					{
						concreteType = settingType;
					}

					if (TryConvertFromXml(concreteType, entry, out object objectConvertedFromXml))
					{
						return objectConvertedFromXml;
					}
				}

				/* Android and perhaps other platforms don't have built in support for complex types.
				 * In the case where the entry is stored as a string but the setting type is not a string, 
				 * we attempt to convert the value.
				 * Here we deal with dates and resort to a type converter if all else fails. */
				Type entryType = entry?.GetType();

				if (entry != null && entryType != settingType
					&& settingType != typeof(string) && entryType == typeof(string))
				{
					string entryString = (string)entry;

					try
					{
						var stringEntry = (string)entry;
						if (settingType == typeof(DateTime))
						{
							DateTime d = DateTime.Parse(stringEntry);
							entry = d;
						}
						else if (settingType == typeof(DateTime?))
						{
							DateTime? d = null;
							if (!string.IsNullOrEmpty(stringEntry))
							{
								d = DateTime.Parse(stringEntry);
							}

							entry = d;
						}
						else if (settingType == typeof(DateTimeOffset))
						{
							DateTimeOffset d = DateTimeOffset.Parse(stringEntry);
							entry = d;
						}
						else if (settingType == typeof(DateTimeOffset?))
						{
							DateTimeOffset? d = null;
							if (!string.IsNullOrEmpty(stringEntry))
							{
								d = DateTimeOffset.Parse(stringEntry);
							}

							entry = d;
						}
						else if (entryString.StartsWith(SerializationContants.Base64EncodingPrefix))
						{
							int lengthOfPrefix = SerializationContants.Base64EncodingPrefix.Length;
							var dataPart = entryString.Substring(lengthOfPrefix, entryString.Length - lengthOfPrefix);
							entry = Convert.FromBase64String(dataPart);
						}
						else
						{
							//							var converter = new FromStringConverter(settingType);
							//							var convertedValue = converter.ConvertFrom(entry);
							//							entry = convertedValue;
							entry = entryString;
						}
					}
					catch (Exception ex)
					{
						string message = $"Unable to parse setting that is of type {settingType} but is stored as a string";
						throw new SettingsException(message, ex);
					}
				}

				/* Dictionaries and other complex types can be difficult to serialize. 
				 * In some cases the SilverlightSerializer is used to serializer such objects.
				 */
				entry = InflateEntry(settingType, entry);

				cache[cacheKey] = entry;

				return entry;
			}

			returningDefaultValue = true;
			return defaultValue;
		}

		static object InflateEntry(Type settingType, object entry)
		{
			if (entry != null && entry.GetType() != settingType && settingType != typeof(byte[]) && entry is byte[])
			{
				try
				{
					var byteArray = (byte[])entry;
					var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
					entry = serializer.Deserialize<object>(byteArray);
				}
				catch (Exception ex)
				{
					string message = string.Format("Unable to parse setting that is of type {0} but is stored as a byte[]", settingType);
					throw new SettingsException(message, ex);
				}
			}
			return entry;
		}

		bool TryConvertFromXml(Type settingType, object xmlFragment, out object result)
		{
			if (xmlFragment != null)
			{
				try
				{
					var convertible = (IXmlConvertible)Activator.CreateInstance(settingType);
					XElement element = XElement.Parse(xmlFragment.ToString());
					convertible.FromXElement(new XElement("Root", element));
					result = convertible;
					return true;
				}
				catch (Exception ex)
				{
					Debug.WriteLine("SettingsService: Unable to convert to IXmlConvertible. " + ex);

					if (RaiseExceptionsOnConversionErrors)
					{
						throw new SettingsException("SettingsService: Unable to convert to IXmlConvertible. " 
												+ settingType + " " + xmlFragment, ex);
					}
				}
			}

			result = null;
			return false;
		}

		public bool ContainsSetting<TSetting>(string key)
		{
			AssertArg.IsNotNull(key, nameof(key));

			lockSlim.EnterReadLock();
			try
			{
				string cacheKey = key;
				if (cache.TryGetValue(cacheKey, out object cachedValue) 
					&& cachedValue != null)
				{
					return true;
				}
				else
				{
					return ContainsSettingNotThreadSafe(key);
				}
			}
			finally
			{
				lockSlim.ExitReadLock();
			}
		}

		public bool ContainsSetting(string key)
		{
			AssertArg.IsNotNull(key, nameof(key));

			lockSlim.EnterReadLock();
			try
			{
				return ContainsSettingNotThreadSafe(key);
			}
			finally
			{
				lockSlim.ExitReadLock();
			}
		}

		bool ContainsSettingNotThreadSafe(string key)
		{
			bool hasSetting = localStore.Status == SettingsStoreStatus.Ready && localStore.Contains(key);
			hasSetting |= transientStore.Status == SettingsStoreStatus.Ready && transientStore.Contains(key);
			hasSetting |= localStore != roamingStore && roamingStore != null
				&& roamingStore.Status == SettingsStoreStatus.Ready && roamingStore.Contains(key);
			return hasSetting;
		}

		public SetSettingResult SetSetting<T>(string key, T value, StorageLocation storageLocation = StorageLocation.Local)
		{
			Type settingType = typeof(T);
			var settingTypeInfo = settingType.GetTypeInfo();
			bool xmlConvertible = xmlConvertibleTypeInfo.IsAssignableFrom(settingTypeInfo);

			/* Check to see if the value is already set. 
			 * This avoids raising events unnecessarily. */
			bool alreadySet;

			lockSlim.EnterWriteLock();
			try
			{
				alreadySet = false;
				cache.Remove(key);

				if (localStore.Status == SettingsStoreStatus.Ready && localStore.TryGetValue(key, settingType, out object existingValue)
					|| (roamingStore != null && roamingStore.Status == SettingsStoreStatus.Ready && roamingStore.TryGetValue(key, settingType, out existingValue))
					|| (transientStore != null && transientStore.Status == SettingsStoreStatus.Ready && transientStore.TryGetValue(key, settingType, out existingValue)))
				{
					if (existingValue == null && value == null || existingValue != null && existingValue.Equals(value))
					{
						alreadySet = true;
					}
					else if (xmlConvertible)
					{
						if (existingValue != null)
						{
							Type concreteType = settingType.IsInterface() ? value?.GetType() : settingType;

							if (concreteType != null)
							{
								if (TryConvertFromXml(concreteType, existingValue, out object existingObject))
								{
									if (Equals(existingObject, value))
									{
										alreadySet = true;
									}
								}
							}
						}
					}
					else if (existingValue != null && settingType.IsEnum() && existingValue is int)
					{
						int intValue = (int)(object)value;//(int)Convert.ChangeType(value, typeof(int));
						if (AreEqual(intValue, existingValue))
						{
							alreadySet = true;
						}
					}
					else if (existingValue != null && existingValue is byte[] && settingType != typeof(byte[]))
					{
						var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
						var existingObject = serializer.Deserialize<object>((byte[])existingValue);
						if (AreEqual(value, existingObject))
						{
							alreadySet = true;
						}
					}
				}
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
			
			if (alreadySet)
			{
				return SetSettingResult.Successful;
			}

			/* Allows this action to be cancelled. */
			var args = new SettingChangingEventArgs(key, value);
			OnSettingChanging(args);

			if (args.Cancel)
			{
				Debug.WriteLine($"Setting change cancelled. Key: {key} New Value: {value}");
				return SetSettingResult.Cancelled;
			}

			lockSlim.EnterWriteLock();
			try
			{
				string cacheKey = key;
				cache[cacheKey] = value;

				if (storageLocation == StorageLocation.Transient)
				{
					if (transientStore.Status == SettingsStoreStatus.Ready)
					{
						SetTransientStateValue(transientStore, key, value);
					}
					else
					{
						/* SettingsService is being used before app is completely initialized. */
						UIContext.Instance.Post(delegate
						{
							if (transientStore.Status == SettingsStoreStatus.Ready)
							{
								SetTransientStateValue(transientStore, key, value);
							}
						});
					}
				}
				else
				{
					ISettingsStore store;
					if (storageLocation == StorageLocation.Roaming)
					{
						store = roamingStore ?? localStore;
					}
					else
					{
						store = localStore;
					}

					if (store.Status == SettingsStoreStatus.Ready)
					{
						SaveValueToStore(store, key, value, xmlConvertible, settingType);
					}
					else
					{
						/* We push the save value request onto the UI thread queue in case 
						 * some further initialization needs to occur. Otherwise it is up to 
						 * the ISettingsStore implementation to queue the update. */
						UIContext.Instance.Post(delegate
						{
							if (store.Status == SettingsStoreStatus.Ready)
							{
								SaveValueToStore(store, key, value, xmlConvertible, settingType);
							}
						});
					}
				}
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}

			OnSettingChanged(new SettingChangeEventArgs(key, value));

			return SetSettingResult.Successful;
		}

//		void SetSettingAuxiliary<T>(string key, T value, Type settingType, 
//			bool xmlConvertible, out bool alreadySet)
//		{
//			alreadySet = false;
//			cache.Remove(key);
//
//			object existingValue;
//			if (localStore.Status == SettingsStoreStatus.Ready
//					&& localStore.TryGetValue(key, settingType, out existingValue)
//				|| (roamingStore != null && roamingStore.Status == SettingsStoreStatus.Ready
//						&& roamingStore.TryGetValue(key, settingType, out existingValue)))
//			{
//				if (xmlConvertible)
//				{
//					object existingObject;
//					if (TryConvertFromXml(settingType, existingValue, out existingObject))
//					{
//						if (object.Equals(existingObject, value))
//						{
//							alreadySet = true;
//							return;
//						}
//					}
//				}
//				else if (existingValue != null && existingValue is byte[] && settingType != typeof(byte[]))
//				{
//					var existingObject = SilverlightSerializer.Deserialize((byte[])existingValue);
//					if (AreEqual(value, existingObject))
//					{
//						alreadySet = true;
//						return;
//					}
//				}
//
//				if (existingValue != null && existingValue.Equals(value))
//				{
//					alreadySet = true;
//					return;
//				}
//			}
//
//			if (transientStore.Status == SettingsStoreStatus.Ready
//					&& transientStore.TryGetValue(key, settingType, out existingValue)
//					&& existingValue.Equals(value))
//			{
//				alreadySet = true;
//			}
//		}

		static bool AreEqual<T>(T value, object existingObject)
		{
			if (existingObject != null 
				&& existingObject is IDictionary 
				&& value is IDictionary)
			{
				if (CollectionComparer.AreEqualDictionaries((IDictionary)existingObject, (IDictionary)value))
				{
					return true;
				}
			}
			else if (existingObject != null 
				&& existingObject is IList 
				&& value is IList)
			{
				if (CollectionComparer.AreEqualLists((IList)existingObject, (IList)value))
				{
					return true;
				}
			}

			if (object.Equals(existingObject, value))
			{
				return true;
			}

			return false;
		}

		void SaveValueToStore<T>(ISettingsStore settingsStore, string key, T value, bool xmlConvertible, Type settingType)
		{
			object existingValue = null;
			bool valueRestorable = false;
			bool isNew = false;

			if (value == null)
			{
				settingsStore.Remove(key);
			}
			else
			{
				valueRestorable = settingsStore.TryGetValue(key, settingType, out existingValue);
				isNew = !valueRestorable;

				if (!xmlConvertible)
				{
					object savableValue = GetSavableValue(settingType, value);

					settingsStore[key] = savableValue;
				}
				else
				{
					object xmlRepresentationOfValue;
					try
					{
						IXmlConvertible convertible = (IXmlConvertible)value;
						var element = convertible.ToXElement();
						xmlRepresentationOfValue = element.ToString(SaveOptions.None);
					}
					catch (Exception ex)
					{
						Debug.WriteLine("SettingsService.SetSetting error raised converting to XML." + settingType + " " + ex);

						if (RaiseExceptionsOnConversionErrors)
						{
							throw new SettingsException(
								"SettingsService.SetSetting error raised converting to XML. Value:" + value + ", Setting type:" + settingType,
								ex);
						}

						xmlRepresentationOfValue = GetSavableValue(settingType, value);
					}

					settingsStore[key] = xmlRepresentationOfValue;
				}
			}

			try
			{
				/* We attempt to save the value to the settings store. 
				 * If the value cannot be saved, we attempt to roll back. */
				settingsStore.SaveAsync();
			}
			catch (Exception ex)
			{
				try
				{
					if (valueRestorable)
					{
						settingsStore[key] = existingValue;
					}
					else if (isNew)
					{
						settingsStore.Remove(key);
					}

					if (valueRestorable || isNew)
					{
						settingsStore.SaveAsync();
					}
				}
				catch (Exception ex2)
				{
					Debug.WriteLine("Unable to restore value after exception raised in SettingsService" + ex2);
				}

				if (RaiseExceptionsOnConversionErrors)
				{
					throw new SettingsException(
						"SettingsService.SetSetting error raised calling settingsStore.Save(). Value:" + value + ", Setting type:" + settingType,
						ex);
				}
			}
		}

		static void SetTransientStateValue<T>(ISettingsStore transientState, string key, T value)
		{
			if (value == null)
			{
				transientState.Remove(key);
			}
			else
			{
				Type settingType = typeof(T);

				object savableValue = GetSavableValue(settingType, value);

				transientState[key] = savableValue;
			}
		}

		static object GetSavableValue<T>(Type settingType, T value)
		{
			object savableValue = value;

			if (settingType.IsEnum())
			{
				savableValue = (int)(object)value;
			}
			else if (!settingType.IsPrimitive()
				&& settingType != typeof(string) 
				&& settingType != typeof(DateTime) 
				&& settingType != typeof(Guid)
				&& settingType != typeof(byte[])
				//&& !settingType.IsSerializable 
				//&& !Attribute.IsDefined(settingType, typeof(DataContractAttribute))
				)
			{
				if (savableValue is IXmlConvertible xmlConvertible)
				{
					savableValue = xmlConvertible.ToXElement().ToString();
				}
				else if (savableValue is DateTimeOffset offset)
				{
					savableValue = offset.ToString(dateTimeFormatString);
				}
				else
				{
					var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
					savableValue = serializer.Serialize(value);
				}
			}

			return savableValue;
		}

		public async Task ClearSettings()
		{
			lockSlim.EnterWriteLock();
			try
			{
				if (localStore.Status == SettingsStoreStatus.Ready)
				{
					await localStore.ClearAsync();
					await localStore.SaveAsync();
				}

				if (roamingStore != null && roamingStore != localStore
					&& roamingStore.Status == SettingsStoreStatus.Ready)
				{
					await roamingStore.ClearAsync();
					await roamingStore.SaveAsync();
				}

				if (transientStore.Status == SettingsStoreStatus.Ready)
				{
					await transientStore.ClearAsync();
					await transientStore.SaveAsync();
				}
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

#region event SettingChanging

		public event EventHandler<SettingChangingEventArgs> SettingChanging;

		protected virtual void OnSettingChanging(SettingChangingEventArgs e)
		{
			SettingChanging?.Invoke(this, e);
		}

#endregion

		#region event SettingChanged

		public event EventHandler<SettingChangeEventArgs> SettingChanged;

		protected virtual void OnSettingChanged(SettingChangeEventArgs e)
		{
			SettingChanged?.Invoke(this, e);
		}

		#endregion

		#region event SettingRemoved

		public event EventHandler<SettingRemovedEventArgs> SettingRemoved;

		protected virtual void OnSettingRemoved(SettingRemovedEventArgs e)
		{
			SettingRemoved?.Invoke(this, e);
		}

		#endregion
	}
}
