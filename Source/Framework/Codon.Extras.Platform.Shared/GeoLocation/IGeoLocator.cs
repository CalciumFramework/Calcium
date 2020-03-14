#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-18 13:16:37Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;

namespace Codon.GeoLocation
{
	/// <summary>
	/// Provides a platform agnostic abstraction to monitor location changes.
	/// </summary>
	public interface IGeoLocator
	{
		/// <summary>
		/// Occurs when the location of the device changes.
		/// <seealso cref="PositionChangedProxyEventArgs"/>
		/// </summary>
		event EventHandler<PositionChangedProxyEventArgs> PositionChanged;

		/// <summary>
		/// Occurs when the geo-location system changes from being in,
		/// for example, an initializing state to a ready state.
		/// <seealso cref="PositionStatus"/>
		/// </summary>
		event EventHandler<StatusChangedProxyEventArgs> StatusChanged;

		/// <summary>
		/// Returns the last known location for the device.
		/// </summary>
		/// <returns></returns>
		Task<GeopositionWrapper> GetGeoCoordinateAsync();

		/// <summary>
		/// Returns the last known location for the device.
		/// </summary>
		/// <param name="maximumAge">If the time stamp
		/// of last known location is older, then a new value is retrieved.
		/// </param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		Task<GeopositionWrapper> GetGeoCoordinateAsync(
					TimeSpan maximumAge, TimeSpan timeout);

		PositionAccuracy DesiredAccuracy { get; set; }
		PositionStatus LocationStatus { get; }

		double MovementThresholdMeters { get; set; }
		uint ReportInterval { get; set; }

		void Start();
		void Stop();

#if __ANDROID__
		Task<Android.Locations.Address> GetAddressOfLocation(Android.Locations.Location location);
#endif
	}

	public class PositionChangedProxyEventArgs : EventArgs
	{
		public GeopositionWrapper Position { get; private set; }

		public PositionChangedProxyEventArgs(GeopositionWrapper position)
		{
			Position = position;
		}
	}

	public class StatusChangedProxyEventArgs : EventArgs
	{
		/// <summary>
		/// The status at the time of the event.
		/// </summary>
		public PositionStatus Status { get; private set; }

		/// <summary>
		/// </summary>
		/// <param name="status">The location status.</param>
		public StatusChangedProxyEventArgs(PositionStatus status)
		{
			Status = status;
		}
	}
}
