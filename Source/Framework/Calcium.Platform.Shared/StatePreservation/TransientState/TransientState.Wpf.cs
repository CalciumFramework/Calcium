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
	<CreationDate>2017-03-11 23:55:51Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calcium.StatePreservation
{
	/// <summary>
	/// WPF implementation of <see cref="ITransientState"/>.
	/// </summary>
	public class TransientState : ITransientState
	{
		readonly IDictionary<string, object> stateDictionary 
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
			loaded = true;
		}

		public Task SaveAsync()
		{
			Save();

			return Task.CompletedTask;
		}

		public void Save()
		{
			/* Nothing to do in WPF implementation. */
		}

		public void Clear()
		{
			stateDictionary.Clear();
			Save();
		}
	}
}
#endif
