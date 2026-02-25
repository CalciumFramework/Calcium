using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Calcium.LogViewer.UI;

namespace Calcium.LogViewer
{
	public partial class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow
				{
					//DataContext = new MainVM()
				};
			}
			else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
			{
				singleViewPlatform.MainView = new MainView
				{
					//DataContext = new MainVM()
				};
			}

			base.OnFrameworkInitializationCompleted();
		}
	}
}
