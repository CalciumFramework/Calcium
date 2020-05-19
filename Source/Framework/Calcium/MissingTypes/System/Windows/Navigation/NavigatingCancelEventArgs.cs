using System;
using System.ComponentModel;
using Calcium.Navigation;

namespace Calcium.MissingTypes.System.Windows.Navigation
{
	/// <summary>
	/// Placeholder class to for compatibility.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class NavigatingCancelEventArgs
	{
		public object Content { get; set; }
		public object ExtraData { get; set; }
		public bool IsNavigationInitiator { get; set; }
		public NavigationType NavigationType { get; set; }
		public object Navigator { get; set; }
		public Uri Uri { get; set; }
		public bool Cancel { get; set; }
	}
}
