#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2015-04-26 19:39:08Z</CreationDate>
</File>
*/
#endregion

namespace Codon.Device
{
	/// <summary>
	/// Indicates the type of power connection
	/// that a device has.
	/// </summary>
	public enum DevicePowerSource
	{
		/// <summary>
		/// The device is powered by its battery.
		/// </summary>
		Battery,

		/// <summary>
		/// The device is powered by mains power.
		/// </summary>
		External
	}
}