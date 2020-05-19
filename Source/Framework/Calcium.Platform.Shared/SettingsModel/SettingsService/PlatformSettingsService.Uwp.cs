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
	<CreationDate>2017-03-11 23:52:58Z</CreationDate>
</File>
*/
#endregion

using Codon.StatePreservation;

namespace Codon.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="Codon.Services.ISettingsService"/> 
	/// implementation for UWP.
	/// </summary>
    public class PlatformSettingsService : SettingsService
    {
		public PlatformSettingsService() 
			: base(new LocalSettingsStore(), 
				  new RoamingSettingsStore(), 
				  new TransientSettingsStore(new TransientState()))
		{
			/* Intentionally left blank. */
		}
	}
}

#endif