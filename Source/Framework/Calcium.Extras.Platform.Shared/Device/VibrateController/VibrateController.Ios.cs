#if __IOS__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 15:38:54Z</CreationDate>
</File>
*/
#endregion

using AudioToolbox;

using Calcium.Services;

namespace Calcium.Device
{
	public class VibrateController : IVibrateController
	{
		public void Start(System.TimeSpan duration)
		{
			SystemSound.Vibrate.PlaySystemSound();
		}

		public void Stop()
		{
			/* Not supported. */
		}
	}
}
#endif
