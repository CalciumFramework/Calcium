#if WINDOWS_UWP || NETFX_CORE
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
using System.Collections.Generic;
using System.Threading.Tasks;

using Calcium.StatePreservation;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="ISettingsStore"/> 
	/// implementation for UWP transient storage.
	/// </summary>
	public class TransientSettingsStore : ISettingsStore
	{
		readonly ITransientState transientState;

		public TransientSettingsStore(ITransientState transientState)
		{
			this.transientState = AssertArg.IsNotNull(transientState, nameof(transientState));
		}

		IDictionary<string, object> GetStateDictionary()
		{
			return transientState.StateDictionary;
		}

		public SettingsStoreStatus Status => SettingsStoreStatus.Ready;

		public bool TryGetValue(string key, Type settingType, out object value)
		{
			var dictionary = GetStateDictionary();
			return dictionary.TryGetValue(key, out value);
		}

		public bool Contains(string key)
		{
			var dictionary = GetStateDictionary();
			return dictionary.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			var dictionary = GetStateDictionary();
			return dictionary.Remove(key);
		}

		public Task ClearAsync()
		{
			transientState.Clear();

			return Task.CompletedTask;
		}

		public async Task SaveAsync()
		{
			await transientState.SaveAsync();
		}

		public object this[string key]
		{
			get
			{
				var dictionary = GetStateDictionary();
				return dictionary[key];
			}
			set
			{
				var dictionary = GetStateDictionary();
				dictionary[key] = value;
			}
		}
	}
}
#endif
