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
	<CreationDate>2017-03-11 23:55:36Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;

using Calcium.IO;

namespace Calcium.StatePreservation
{
	/// <summary>
	/// UWP implementation of <see cref="ITransientState"/>.
	/// </summary>
	public class TransientState : ITransientState
	{
		internal static readonly string dictionaryKey = "__Calcium_TransientState";

		IDictionary<string, object> stateDictionary 
			= new Dictionary<string, object>();

		bool loaded;

		public IDictionary<string, object> StateDictionary
		{
			get
			{
				if (!loaded)
				{
					Load();
				}

				return stateDictionary;
			}
		}

		public Task LoadAsync()
		{
			Load();

			return Task.CompletedTask;
		}

		public void Load()
		{
			var bytes = ApplicationData.Current.LocalSettings.Values[dictionaryKey] as byte[];
			if (bytes == null)
			{
				return;
			}

			var serializer = Dependency.Resolve<IBinarySerializer>();
			stateDictionary = serializer.Deserialize<IDictionary<string, object>>(bytes);

			loaded = true;
		}

		public Task SaveAsync()
		{
			Save();

			return Task.CompletedTask;
		}

		public void Save()
		{
			if (stateDictionary == null || !stateDictionary.Any())
			{
				ApplicationData.Current.LocalSettings.Values[dictionaryKey] = null;
				return;
			}

			var serializer = Dependency.Resolve<IBinarySerializer>();
			var bytes = serializer.Serialize(stateDictionary);

			ApplicationData.Current.LocalSettings.Values[dictionaryKey] = bytes;
		}

		public void Clear()
		{
			stateDictionary.Clear();
			Save();
		}
	}
}
#endif
