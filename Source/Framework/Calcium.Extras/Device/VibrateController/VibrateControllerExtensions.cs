#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-11-22 12:13:34Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;


namespace Calcium.Services
{
	public static class VibrateControllerExtensions
	{
		/// <summary>
		/// Causes the device to vibrate for the specified duration.
		/// </summary>
		/// <param name="vibrateController">This is used to control
		/// the vibration on the device.</param>
		/// <param name="durationMS">The duration for which to vibrate,
		/// in milliseconds. Default is 100 MS.</param>
		/// <param name="repetitions">The number of times to repeat the cycle
		/// of vibration and pausing.</param>
		/// <param name="intervalMS">The pause time in milliseconds between repetitions.</param>
		/// <returns></returns>
		public static Task VibrateAsync(this IVibrateController vibrateController, 
			uint durationMS = 100, uint intervalMS = 0, uint repetitions = 0)
		{
			var durationTimeSpan = TimeSpan.FromMilliseconds(durationMS);

			return Task.Run(async () =>
			{
				for (int i = 0; i < repetitions + 1; i++)
				{
					vibrateController.Start(durationTimeSpan);
					await Task.Delay(durationTimeSpan);
					if (intervalMS > 0)
					{
						await Task.Delay((int)intervalMS);
					}
				}
			});
		}
	}
}
