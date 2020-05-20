#if WPF
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-18 22:01:16Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="Calcium.Services.ISettingsService"/> 
	/// implementation for WPF.
	/// </summary>
	public class PlatformSettingsService : SettingsService
    {
		public PlatformSettingsService() 
			: base(new LocalSettingsStoreForWpf(), 
				  null, /* No roaming storage for you WPF. Bad monkey! */
				  new InMemoryTransientSettingsStore())
		{

		}
	}
}

#endif
