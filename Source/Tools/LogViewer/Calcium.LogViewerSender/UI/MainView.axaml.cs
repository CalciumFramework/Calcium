using Avalonia.Controls;

namespace Calcium.LogViewerSender.UI
{
	public partial class MainView : UserControl
	{
		public MainView()
		{
			InitializeComponent();

			this.DataContext = new MainVM();
		}
	}
}
