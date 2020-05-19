#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-12 14:15:10Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codon.SettingsModel
{
	/// <summary>
	/// Rudimentary implementation of an <c>ISettingsStore</c> 
	/// for storing data that should be discarded 
	/// when the application exits.
	/// </summary>
	public class InMemoryTransientSettingsStore : ISettingsStore
	{
		readonly Dictionary<string, object> dictionary 
			= new Dictionary<string, object>();

		public bool TryGetValue(string key, Type settingType, out object value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public bool Contains(string key)
		{
			return dictionary.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			return dictionary.Remove(key);
		}

		public Task ClearAsync()
		{
			dictionary.Clear();

			return Task.CompletedTask;
		}

		public Task SaveAsync()
		{
			/* Nothing to do. */

			return Task.CompletedTask;
		}

		public object this[string key]
		{
			get => dictionary[key];
			set => dictionary[key] = value;
		}

		public SettingsStoreStatus Status => SettingsStoreStatus.Ready;
	}
}

