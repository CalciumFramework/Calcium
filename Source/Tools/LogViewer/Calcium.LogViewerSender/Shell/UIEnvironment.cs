using Avalonia.Controls;

namespace Orpius.Shell
{
	public class UIEnvironment
	{
		public static bool DesignTime => Design.IsDesignMode;

		public static bool UsingEmulator => false;
	}
}