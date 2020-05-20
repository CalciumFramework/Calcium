#if WINDOWS_UWP
using Windows.Graphics.Display;

namespace Calcium.UI
{
	public static class DisplayOrientationsExtensions
	{
		public static bool IsLandscape(this DisplayOrientations pageOrientation)
		{
			return (pageOrientation & DisplayOrientations.Landscape) != 0
				|| (pageOrientation & DisplayOrientations.LandscapeFlipped) != 0;
		}

		public static bool IsPortrait(this DisplayOrientations pageOrientation)
		{
			return (pageOrientation & DisplayOrientations.Portrait) != 0
				|| (pageOrientation & DisplayOrientations.PortraitFlipped) != 0;
		}
	}
}
#endif
