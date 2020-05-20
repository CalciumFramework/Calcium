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
	<CreationDate>2017-03-20 12:57:08Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.ComponentModel
{
	public partial class ExecutionEnvironment
	{
		static bool? usingEmulator;

		public static bool UsingEmulator
		{
			get
			{
				if (!usingEmulator.HasValue)
				{
					string fingerprint = Android.OS.Build.Fingerprint;

					if (fingerprint != null)
					{
						usingEmulator = fingerprint.Contains("vbox") || fingerprint.Contains("generic");
					}
					else
					{
						usingEmulator = false;
					}
				}
				return usingEmulator.Value;
			}
		}
	}
}
#endif
