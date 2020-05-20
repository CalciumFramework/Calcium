#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-26 16:14:40Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// Indicates the availability of an <see cref="ISettingsStore"/>.
	/// </summary>
	public enum SettingsStoreStatus
	{
		/// <summary>
		/// The settings store is able to save and restore settings.
		/// </summary>
		Ready,

		/// <summary>
		/// The settings store is not able to save and restore settings.
		/// This may be because the store is in the process of
		/// initialising.
		/// </summary>
		Unavailable
	}

	/// <summary>
	/// This interface the code contract for a class
	/// that is able to save and restore a keyed
	/// object collection.
	/// </summary>
	public interface ISettingsStore
	{
		/// <summary>
		/// Attempts to retrieve a value from the store
		/// using the specified item key.
		/// </summary>
		/// <param name="key">
		/// The unique key of the setting.</param>
		/// <param name="settingType">
		/// The type of the setting.
		/// This is used to convert a serialized
		/// object back to its original form.</param>
		/// <param name="value">The resulting setting.</param>
		/// <returns><c>true</c> if the setting is located;
		/// <c>false</c> otherwise.</returns>
		bool TryGetValue(string key, Type settingType, out object value);

		/// <summary>
		/// Deterimes if a setting with the specified
		/// key exists in the store.
		/// </summary>
		/// <param name="key">The unique setting key.</param>
		/// <returns><c>true</c> if the setting is located;
		/// <c>false</c> otherwise.</returns>
		bool Contains(string key);

		/// <summary>
		/// Remove the setting with the specified key
		/// from the store.
		/// </summary>
		/// <param name="key">The unique setting key.</param>
		/// <returns><c>true</c> if the setting is located
		/// and removed; <c>false</c> otherwise.</returns>
		bool Remove(string key);

		/// <summary>
		/// Removes all settings from the store.
		/// </summary>
		Task ClearAsync();

		/// <summary>
		/// Requests that all settings be persisted
		/// immediately
		/// </summary>
		Task SaveAsync();

		/// <summary>
		/// Gets the settings value for the specified
		/// setting key.
		/// </summary>
		/// <param name="key">The unique setting key.</param>
		/// <returns>
		/// The setting value, or <c>null</c>
		/// if a setting is not found.</returns>
		object this[string key] { get; set; }
		
		/// <summary>
		/// Indicates the current state of the store.
		/// <see cref="SettingsStoreStatus"/>
		/// </summary>
		SettingsStoreStatus Status { get; }
	}
}
