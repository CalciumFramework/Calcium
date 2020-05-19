#if __ANDROID__
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

using Android.Locations;
using System;

namespace Calcium.GeoLocation
{
	public partial class GeopositionWrapper
	{
		public Location Location { get; private set; }

		public GeopositionWrapper(Location location)
		{
			Location = location;
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			DateTimeOffset time = epoch.AddMilliseconds(location.Time);

			Timestamp = time;
		}
	}
}
#endif
