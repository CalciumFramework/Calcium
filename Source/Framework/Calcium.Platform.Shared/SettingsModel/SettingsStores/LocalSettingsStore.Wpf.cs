#if WPF
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-26 16:44:33Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

using Calcium.Concurrency;

using Settings = Calcium.SettingsModel.IsolatedStorageSettingsWpf;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="Calcium.SettingsModel.ISettingsStore"/> 
	/// implementation for WPF persistent local settings.
	/// </summary>
	public class LocalSettingsStoreForWpf : ISettingsStore
	{
		public bool TryGetValue(string key, Type settingType, out object value)
		{
			return Settings.ApplicationSettings.TryGetValue(key, out value);
		}

		public bool Contains(string key)
		{
			return Settings.ApplicationSettings.Contains(key);
		}

		public bool Remove(string key)
		{
			return Settings.ApplicationSettings.Remove(key);
		}

		public Task ClearAsync()
		{
			Settings.ApplicationSettings.Clear();

			return Task.CompletedTask;
		}

		public Task SaveAsync()
		{
			Settings.ApplicationSettings.Save();

			return Task.CompletedTask;
		}

		public object this[string key]
		{
			get => Settings.ApplicationSettings[key];
			set => Settings.ApplicationSettings[key] = value;
		}

		public SettingsStoreStatus Status => SettingsStoreStatus.Ready;
	}
}
#endif
