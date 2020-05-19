#if WINDOWS_UWP
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

using Windows.Storage;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="Calcium.SettingsModel.ISettingsStore"/> 
	/// implementation for UWP roaming storage.
	/// </summary>
	public class RoamingSettingsStore : ISettingsStore
	{
		ApplicationDataContainer settings_UseGetSettings;

		ApplicationDataContainer GetSettings()
		{
			return settings_UseGetSettings 
				?? (settings_UseGetSettings 
						= ApplicationData.Current.RoamingSettings);
		}

		public bool TryGetValue(
			string key, Type settingType, out object value)
		{
			var settings = GetSettings();
			return settings.Values.TryGetValue(key, out value);
		}

		public bool Contains(string key)
		{
			var settings = GetSettings();
			return settings.Values.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			var settings = GetSettings();
			return settings.Values.Remove(key);
		}

		public Task ClearAsync()
		{
			var settings = GetSettings();
			settings.Values.Clear();

			return Task.CompletedTask;
		}

		public Task SaveAsync()
		{
			/* Nothing to do. */
			return Task.CompletedTask;
		}

		public object this[string key]
		{
			get
			{
				var settings = GetSettings();
				return settings.Values[key];
			}
			set
			{
				var settings = GetSettings();
				settings.Values[key] = value;
			}
		}

		public SettingsStoreStatus Status => SettingsStoreStatus.Ready;
	}
}
#endif
