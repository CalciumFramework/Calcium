#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-09-12 22:07:15Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.Device
{
	/// <summary>
	/// Indicates if a battery is charging or not.
	/// </summary>
	public enum BatteryState
	{
		Unknown = 0,
		Charging = 1,
		Discharging = 2,
		Full = 4,
		NotCharging = 8
	}
}
