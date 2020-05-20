#if WINDOWS_PHONE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-18 13:23:07Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Device.Location;

using Windows.Devices.Geolocation;
using CivicAddress = Windows.Devices.Geolocation.CivicAddress;

namespace Calcium.GeoLocation
{
	public partial class GeopositionWrapper
	{
		public CivicAddress CivicAddress { get; private set; }
		public GeoCoordinate GeoCoordinate { get; private set; }

		public GeopositionWrapper(Geoposition geoposition)
		{
			CivicAddress = geoposition.CivicAddress;
			GeoCoordinate = geoposition.Coordinate.ToGeoCoordinate();
			Timestamp = geoposition.Coordinate.Timestamp;
		}

		public GeopositionWrapper(GeoCoordinate geoCoordinate)
		{
			GeoCoordinate = geoCoordinate;
			Timestamp = DateTime.UtcNow;
		}
	}
}
#endif
