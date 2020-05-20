#if __ANDROID__

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="Calcium.Services.ISettingsService"/> 
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
