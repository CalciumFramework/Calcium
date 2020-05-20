#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-01-23 17:21:10Z</CreationDate>
</File>
*/
#endregion
using System;
using System.Threading.Tasks;
using Calcium.InversionOfControl;
using Calcium.SettingsModel;

namespace Calcium.Services
{
	/// <summary>
	/// This service allow settings to be persisted and restored.
	/// </summary>
	[DefaultType(typeof(SettingsService), Singleton = true)]
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(SettingsModel) +
		".PlatformSettingsService, " + AssemblyConstants.PlatformAssembly, Singleton = true)]
	public interface ISettingsService
	{
		/// <summary>
		/// Gets the setting instance with the specified key that is of the specified type.
		/// If no entity has been saved, then the specified default value is returned.
		/// </summary>
		/// <typeparam name="TSetting">The type of the setting.</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The located setting, or the default value if <code>null</code>.</returns>
		TSetting GetSetting<TSetting>(string key, TSetting defaultValue);

		/// <summary>
		/// Gets the setting instance with the specified key that is of the specified type.
		/// If no entity has been saved, then the specified default value is returned.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <param name="settingType">The type of the setting value.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The located setting, or the default value if <code>null</code>.</returns>
		object GetSetting(string key, Type settingType, object defaultValue);

		/// <summary>
		/// Attempts to retrieve the specified setting.
		/// </summary>
		/// <param name="key">The unique key of the setting.</param>
		/// <param name="settingType">The object type which is used to construct the setting.</param>
		/// <param name="setting">The retrieved setting if found.</param>
		/// <returns><c>true</c> if the setting exists; false otherwise.</returns>
		bool TryGetSetting(string key, Type settingType, out object setting);

		/// <summary>
		/// Attempts to retrieve the specified setting.
		/// </summary>
		/// <param name="key">The unique key of the setting.</param>
		/// <param name="setting">The retrieved setting if found.</param>
		/// <returns><c>true</c> if the setting exists; false otherwise.</returns>
		bool TryGetSetting<TSettingType>(string key, out TSettingType setting);

		/// <summary>
		/// Retrieve the location of the setting. <see cref="StorageLocation"/>
		/// </summary>
		/// <param name="key">The setting key. Cannot be <c>null</c>.</param>
		/// <returns>The storage location of the setting, or <c>null</c> if setting is not found.</returns>
		StorageLocation? GetSettingLocation(string key);

		/// <summary>
		/// Determines whether the specified key has a registered setting value.
		/// Note: Favor, whereever possible, the use of the generic form of this method, 
		/// as it first checks the cache, which is faster.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		///   <c>true</c> if a setting exists with the specified key; otherwise, <c>false</c>.
		/// </returns>
		bool ContainsSetting(string key);

		/// <summary>
		/// Determines whether the specified key has a registered setting value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		///   <c>true</c> if a setting exists with the specified key; otherwise, <c>false</c>.
		/// </returns>
		bool ContainsSetting<TSetting>(string key);

		/// <summary>
		/// Records the setting using the specified key as a unique identifier.
		/// </summary>
		/// <param name="key">The setting's key.</param>
		/// <param name="value">The value.</param>
		/// <param name="storageLocation">When <c>Transient</c> the setting is retained only for the duration
		/// of current app's run. If <c>Local</c> the setting is retained across app restarts.
		/// When <c>Roaming</c> the value is associated with the user's account.</param>
		/// <returns><code>Successful</code> if the setting was correctly stored,
		/// <code>Cancelled</code> if a listener intervened an prevented cancelled the setting.
		/// Default is <c>Local</c>.</returns>
		SetSettingResult SetSetting<T>(string key, T value, StorageLocation storageLocation = StorageLocation.Local);

		/// <summary>
		/// Removes the setting with the specified key from all storage locations and cache.
		/// </summary>
		/// <param name="key">The name of the setting.</param>
		/// <returns><c>true</c> if a setting was removed; <c>false</c> otherwise.</returns>
		bool RemoveSetting(string key);

		/// <summary>
		/// Clears all settings.
		/// </summary>
		Task ClearSettings();

		/// <summary>
		/// Removes all entries from the cache. New GetSetting requests will resort 
		/// to retrieving items from the underlying storage.
		/// </summary>
		void ClearCache();

		/// <summary>
		/// Removes a particular item from the cache.
		/// </summary>
		/// <param name="key">The setting key.</param>
		void RemoveCacheItem(string key);

		/// <summary>
		/// Occurs when a setting is changed.
		/// </summary>
		event EventHandler<SettingChangeEventArgs> SettingChanged;

		/// <summary>
		/// Occurs when a setting is about to change.
		/// </summary>
		event EventHandler<SettingChangingEventArgs> SettingChanging;

		/// <summary>
		/// Occurs when a setting is removed, such that there is no longer 
		/// a value associated with the setting key located in any <see cref="ISettingsStore"/>.
		/// </summary>
		event EventHandler<SettingRemovedEventArgs> SettingRemoved;
	}
}
