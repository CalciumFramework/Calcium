﻿#if __IOS__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-18 22:02:04Z</CreationDate>
</File>
*/
#endregion

namespace Codon.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="Codon.Services.ISettingsService"/> 
	/// implementation for iOS.
	/// </summary>
    public class PlatformSettingsService : SettingsService
    {
		public PlatformSettingsService() 
			: base(new LocalSettingsStore(), 
				  null, 
				  new InMemoryTransientSettingsStore())
		{

		}
	}
}

#endif