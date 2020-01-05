using System.ComponentModel;

namespace Codon.Platform
{
	/// <summary>
	/// This class's only purpose is to allow identification
	/// of the platform at runtime.
	/// Its presence in an assembly indicates what platform
	/// the application is running on.
	/// Platform identification is needed to resolve dependencies 
	/// in platform specific assemblies.
	/// See the <c>PlatformDetector</c> class.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[System.Runtime.CompilerServices.CompilerGenerated]
	public class PlatformIdentifier : IPlatformIdentifier
	{
		public PlatformId PlatformId { get; } =
#if __ANDROID__
			PlatformId.Android;
#elif WINDOWS_UWP
			PlatformId.Uwp;
#elif __IOS__
			PlatformId.Ios;
#elif WPF
			PlatformId.Wpf;
#elif WPF_CORE
			PlatformId.WpfCore;
#else
			PlatformId.Unknown;
#endif
	}
}
