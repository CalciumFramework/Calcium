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

using Codon.Messaging;

namespace Codon.Device
{
	/// <summary>
	/// This class is used to convey information
	/// when a device power source changes.
	/// </summary>
	public class PowerSourceChangeEvent 
		: ValueChangedMessageBase<DevicePowerSource>
	{
		public PowerSourceChangeEvent(
			object sender, 
			DevicePowerSource oldValue, 
			DevicePowerSource newValue)
			: base(sender, oldValue, newValue)
		{
		}
	}
}