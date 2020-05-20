#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-28 13:23:32Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class is the <c>EventArgs</c> for the 
	/// <see cref="Services.ISettingsService.SettingChanged"/> event.
	/// </summary>
	public class SettingChangeEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the name of the setting.
		/// </summary>
		/// <value>The name of the setting.</value>
		public string SettingName { get; }

		/// <summary>
		/// Gets or sets the setting value.
		/// </summary>
		/// <value>The setting value.</value>
		public object SettingValue { get; }

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="SettingChangeEventArgs"/> class.
		/// </summary>
		/// <param name="settingName">Name of the setting.</param>
		/// <param name="settingValue">The setting value.</param>
		public SettingChangeEventArgs(string settingName, object settingValue)
		{
			SettingName = settingName;
			SettingValue = settingValue;
		}
	}
}
