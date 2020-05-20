#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-08 13:33:02Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calcium.StatePreservation
{
	/// <summary>
	/// Transient state is used to persist an application's
	/// state, which should not persist across an application
	/// exit/start cycle. It may be persisted if the application
	/// is tombstoned.
	/// </summary>
	public interface ITransientState
	{
		/// <summary>
		/// A dictionary of keyed state values.
		/// </summary>
		IDictionary<string, object> StateDictionary { get; }

		/// <summary>
		/// Load state from transient storage. When calling this
		/// method the instance should initialize its
		/// state dictionary. See the <c>TransientState</c>
		/// UWP implementation for example. It saves and loads
		/// its state from local storage.
		/// </summary>
		Task LoadAsync();

		/// <summary>
		/// Save state to transient storage. When calling this
		/// method the instance should initialize its
		/// state dictionary. See the <c>TransientState</c>
		/// UWP implementation for example. It saves and loads
		/// its state from local storage.
		/// </summary>
		Task SaveAsync();

		/// <summary>
		/// Clear the state dictionary 
		/// and remove it from transient storage.
		/// </summary>
		void Clear();
	}
}
