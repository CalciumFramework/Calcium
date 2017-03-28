#if !WINDOWS_UWP && !NETFX_CORE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-18 13:14:43Z</CreationDate>
</File>
*/
#endregion

namespace Windows.Devices.Geolocation
{
	public enum PositionAccuracy
	{
		Default,
		High
#if __ANDROID__
		,Low,
		Medium,
		Coarse,
		Fine,
		NoRequirement
#endif
	}

	public enum PositionStatus
	{
		Disabled,
		Initializing,
		NoData,
		NotAvailable,
		NotInitialized,
		Ready
	}
}
#endif