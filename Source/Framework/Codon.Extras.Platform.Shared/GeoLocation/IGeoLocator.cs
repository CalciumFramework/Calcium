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
	public interface IGeoLocator
	{
		event EventHandler<PositionChangedProxyEventArgs> PositionChanged;
		event EventHandler<StatusChangedProxyEventArgs> StatusChanged;

		Task<GeopositionWrapper> GetGeoCoordinateAsync();

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
		public PositionStatus Status { get; private set; }

		public StatusChangedProxyEventArgs(PositionStatus status)
		{
			Status = status;
		}
	}
}
