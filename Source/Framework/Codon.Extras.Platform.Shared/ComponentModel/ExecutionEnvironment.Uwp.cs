#if WINDOWS_UWP
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-20 12:57:08Z</CreationDate>
</File>
*/
#endregion

using Windows.Security.ExchangeActiveSyncProvisioning;

namespace Codon.ComponentModel
{
	public partial class ExecutionEnvironment
	{
		static readonly EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
		static bool? usingEmulator;
		
		public static bool UsingEmulator
		{
			get
			{
				if (!usingEmulator.HasValue)
				{
					usingEmulator = deviceInfo.SystemProductName == "Virtual";
				}

				return usingEmulator.Value;
			}
		}
	}
}
#endif
