#if DEBUG
using Android.App;

namespace Calcium
{
	[Activity(/*MainLauncher = true*/)]
	class WorkAroundForBug43553 : Activity
	{
	}
}
#endif
