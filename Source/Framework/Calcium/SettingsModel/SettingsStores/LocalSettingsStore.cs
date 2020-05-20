#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-26 16:44:33Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// Default implementation of the <see cref="ISettingsStore"/>
	/// interface, which uses isolated storage to persist 
	/// and restore settings.
	/// </summary>
	public class LocalSettingsStore : ISettingsStore
	{
		public bool TryGetValue(string key, Type settingType, out object value)
		{
			return IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);
		}

		public bool Contains(string key)
		{
			return IsolatedStorageSettings.ApplicationSettings.Contains(key);
		}

		public bool Remove(string key)
		{
			return IsolatedStorageSettings.ApplicationSettings.Remove(key);
		}

		public Task ClearAsync()
		{
			IsolatedStorageSettings.ApplicationSettings.Clear();

			return Task.CompletedTask;
		}

		public Task SaveAsync()
		{
			IsolatedStorageSettings.ApplicationSettings.Save();

			return Task.CompletedTask;
		}

		public object this[string key]
		{
			get => IsolatedStorageSettings.ApplicationSettings[key];
			set => IsolatedStorageSettings.ApplicationSettings[key] = value;
		}

		public SettingsStoreStatus Status => SettingsStoreStatus.Ready;
	}
}
