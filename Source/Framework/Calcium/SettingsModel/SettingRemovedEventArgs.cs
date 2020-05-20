#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-09-14 20:36:25Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class is the <c>EventArgs</c> for the 
	/// <see cref="Services.ISettingsService.SettingRemoved"/> event.
	/// </summary>
	public class SettingRemovedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the name of the setting.
		/// </summary>
		/// <value>The name of the setting.</value>
		public string SettingName { get; }

		/// <inheritdoc />
		/// <param name="settingName">Name of the setting.</param>
		public SettingRemovedEventArgs(string settingName)
		{
			SettingName = settingName;
		}
	}
}
