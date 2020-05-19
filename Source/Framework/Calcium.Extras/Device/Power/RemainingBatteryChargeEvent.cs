#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2015-04-26 19:31:53Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.Device
{
	/// <summary>
	/// This class is used to convey information
	/// when the device's remaining charge changes.
	/// </summary>
	public class RemainingBatteryChargeEvent
	{
		public object Sender { get; }

		public int PercentRemaining { get; }

		public TimeSpan DischargeTimeRemaining { get; }

		public RemainingBatteryChargeEvent(object sender, int percentRemaining, TimeSpan dischargeTimeRemaining)
		{
			Sender = AssertArg.IsNotNull(sender, nameof(sender));

			PercentRemaining = percentRemaining;
			DischargeTimeRemaining = dischargeTimeRemaining;
		}
	}
}
