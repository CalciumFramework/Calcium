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
	<CreationDate>2017-04-06 17:38:02Z</CreationDate>
</File>
*/
#endregion

using Android.App;

namespace Calcium.Device
{
	/// <summary>
	/// Android implementation of the <see cref="IMemoryUsage"/> 
	/// interface.
	/// </summary>
	public class MemoryUsage : IMemoryUsage
	{
		public long ApplicationMemoryUsageMB
		{
			get
			{
				var runtime = Java.Lang.Runtime.GetRuntime();
				long usedMemoryMB = (runtime.TotalMemory() - runtime.FreeMemory()) / 1048576L;
				/*
					Explanation of the number 1048576

					1024 bytes      == 1 kilobyte  
					1024 kilobytes  == 1 megabyte  

					1024 * 1024     == 1048576
				*/
				return usedMemoryMB;
			}
		}

		public long ApplicationAvailableMemoryMB
		{
			get
			{
				var memoryInfo = new ActivityManager.MemoryInfo();
				var context = Application.Context;
				ActivityManager activityManager = (ActivityManager)context.GetSystemService(Application.ActivityService);
				activityManager.GetMemoryInfo(memoryInfo);
				long availableMB = memoryInfo.AvailMem / 1048576L;
				/*
					Explanation of the number 1048576

					1024 bytes      == 1 kilobyte  
					1024 kilobytes  == 1 megabyte  

					1024 * 1024     == 1048576
				*/

				//Percentage can be calculated for API 16+
				//long percentAvail = mi.availMem / mi.totalMem;

				return availableMB;
			}
		}

		long? maxMemory;

		public long ApplicationMemoryLimitMB
		{
			get
			{
				if (!maxMemory.HasValue)
				{
					var runTime = Java.Lang.Runtime.GetRuntime();
					long maxMemoryBytes = runTime.MaxMemory();
					maxMemory = maxMemoryBytes / 1048576L;
				}

				return maxMemory.Value;
			}
		}
	}
}
#endif
