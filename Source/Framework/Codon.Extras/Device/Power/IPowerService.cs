#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2015-04-26 19:31:53Z</CreationDate>
</File>
*/
#endregion

using System;
using Codon.Device;
using Codon.InversionOfControl;

namespace Codon.Services
{
	/// <summary>
	/// Classes implementing this interface are able
	/// to determine the power source that the device
	/// is connected to, either battery or mains.
	/// And, if battery, the amount of charge remaining.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Device) +
					".PowerService, " + AssemblyConstants.ExtrasPlatformAssembly, Singleton = true)]
	public interface IPowerService
	{
		/// <summary>
		/// Gets a value indicating the percentage
		/// of battery charge remaining.
		/// </summary>
		int RemainingBatteryChargePercent { get; }

		/// <summary>
		/// Gets a value indicating how long the battery
		/// will have sufficient charge to keep the device
		/// from shutting down.
		/// </summary>
		TimeSpan RemainingBatteryDischargeTime { get; }

		/// <summary>
		/// The type of connection; battery or mains.
		/// </summary>
		DevicePowerSource PowerSource { get; }
	}
}
