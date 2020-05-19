#if __ANDROID__

namespace Codon.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="Codon.Services.ISettingsService"/> 
	/// implementation for Android.
	/// </summary>
    public class PlatformSettingsService : SettingsService
    {
		public PlatformSettingsService() 
			: base(new AndroidLocalSettingsStore(), 
				  null, 
				  new InMemoryTransientSettingsStore())
		{
			/* Intentionally left blank. */
		}
	}
}

#endif