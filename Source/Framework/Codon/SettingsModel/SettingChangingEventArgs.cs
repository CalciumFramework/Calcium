#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-28 13:24:09Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.SettingsModel
{
	/// <summary>
	/// This class is the <c>EventArgs</c> for the 
	/// <see cref="Services.ISettingsService.SettingChanging"/> event.
	/// </summary>
	public class SettingChangingEventArgs : SettingChangeEventArgs
	{
		bool cancel;

		/// <summary>
		/// Gets or sets a value indicating whether 
		/// this <see cref="SettingChangingEventArgs"/> has been canceled.
		/// Can only be set to <c>true</c>.
		/// </summary>
		/// <value><c>true</c> if this instance's Cancel property 
		/// was set to <c>true</c>; otherwise, <c>false</c>.</value>
		public bool Cancel
		{
			get => cancel;
			set
			{
				/* Cancelled can only be set to true. */
				if (!value)
				{
					return;
				}
				cancel = true;
			}
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="SettingChangingEventArgs"/> class.
		/// </summary>
		/// <param name="settingName">Name of the setting.</param>
		/// <param name="settingValue">The setting value.</param>
		public SettingChangingEventArgs(
			string settingName, object settingValue)
			: base(settingName, settingValue)
		{
		}
	}
}