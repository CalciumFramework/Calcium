#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-12 14:13:24Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This enum is used to specify where a setting is stored.
	/// </summary>
	public enum StorageLocation
	{
		/// <summary>
		/// Stored within the applications local storage.
		/// </summary>
		Local = 0,
		/// <summary>
		/// Stored in roaming storage associated with the user's account.
		/// </summary>
		Roaming = 1,
		/// <summary>
		/// Persists only for a single launch/exit cycle.
		/// </summary>
		Transient = 2
	}
}
