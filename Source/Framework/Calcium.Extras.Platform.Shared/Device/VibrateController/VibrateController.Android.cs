#if __ANDROID__
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

using System;
using Android.App;
using Android.OS;
using Calcium.Services;

namespace Calcium.Device
{
	public class VibrateController : IVibrateController
	{
		public void Start(TimeSpan duration)
		{
			var vibrator = (Vibrator)Application.Context.GetSystemService(
													Android.Content.Context.VibratorService);
			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				vibrator.Vibrate(VibrationEffect.CreateOneShot((long)duration.TotalMilliseconds, 
																	VibrationEffect.DefaultAmplitude));
			}
			else
			{
#pragma warning disable CS0618 // Type or member is obsolete
				vibrator.Vibrate((long)duration.TotalMilliseconds);
#pragma warning restore CS0618 // Type or member is obsolete
			}
		}

		public void Stop()
		{
			var vibrator = (Vibrator)Application.Context.GetSystemService(
													Android.Content.Context.VibratorService);
			vibrator.Cancel();
		}
	}
}
#endif
