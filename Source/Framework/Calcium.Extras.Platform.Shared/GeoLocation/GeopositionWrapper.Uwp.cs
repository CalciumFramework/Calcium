#if WINDOWS_UWP || NETFX_CORE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;

using Windows.Devices.Geolocation;
using CivicAddress = Windows.Devices.Geolocation.CivicAddress;

namespace Calcium.GeoLocation
{
	public partial class GeopositionWrapper
	{
		public CivicAddress CivicAddress { get; private set; }
		public Geocoordinate GeoCoordinate { get; private set; }

		public GeopositionWrapper(Geoposition geoposition)
		{
			CivicAddress = geoposition.CivicAddress;
			GeoCoordinate = geoposition.Coordinate;
			Timestamp = geoposition.Coordinate.Timestamp;
		}

		public GeopositionWrapper(Geocoordinate geoCoordinate)
		{
			GeoCoordinate = geoCoordinate;
			Timestamp = DateTime.UtcNow;
		}
	}
}
#endif
