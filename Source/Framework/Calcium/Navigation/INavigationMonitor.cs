using Calcium.InversionOfControl;

namespace Calcium.Navigation
{
	/// <summary>
	/// Placeholder interface for a class that is tasked
	/// with monitoring page navigation within an app.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Navigation)
					+ ".NavigationMonitor, " + AssemblyConstants.PlatformAssembly, Singleton = true)]
	public interface INavigationMonitor
	{
		/// <summary>
		/// Commences monitoring the app for navigation 
		/// events.
		/// </summary>
		void Initialize();
	}
}
