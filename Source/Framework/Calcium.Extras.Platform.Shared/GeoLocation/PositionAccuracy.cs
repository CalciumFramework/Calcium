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
	/// <summary>
	/// Indicates the accuracy of reading arriving
	/// from the geo-location infrastructure.
	/// </summary>
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

	/// <summary>
	/// The status of the geo-location infrastructure.
	/// </summary>
	public enum PositionStatus
	{
		/// <summary>
		/// Geo-location is not currently enabled on the device.
		/// </summary>
		Disabled,
		/// <summary>
		/// The geo-location infrastructure is initializing.
		/// </summary>
		Initializing,
		/// <summary>
		/// There is currently no geo-location data available.
		/// </summary>
		NoData,
		/// <summary>
		/// Geo-location is not available on the device.
		/// This may indicate that the device does not have geo-location capability.
		/// </summary>
		NotAvailable,
		/// <summary>
		/// The geo-location infrastructure is not initialized.
		/// </summary>
		NotInitialized,
		/// <summary>
		/// The geo-location infrastructure is initialized and awaiting data.
		/// </summary>
		Ready
	}
}
#endif