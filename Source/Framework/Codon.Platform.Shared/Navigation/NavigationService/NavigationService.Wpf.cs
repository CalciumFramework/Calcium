#if WPF
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-02-24 17:17:39Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Windows;
using System.Windows.Controls;

using Codon.Concurrency;
using Codon.Services;
using BuiltInNavigation = System.Windows.Navigation;

namespace Codon.Navigation
{
	/// <summary>
	/// WPF implementation of <see cref="INavigationService"/>.
	/// </summary>
	public class NavigationService : INavigationService
	{
		int attemptsToRetrieveService;

		bool CanGoBackUsingWindow()
		{
			var mainWindow = Application.Current.MainWindow;

			if (mainWindow == null)
			{
				return false;
			}

			var navigationService = BuiltInNavigation.NavigationService.GetNavigationService(mainWindow);
			if (navigationService == null)
			{
				throw new Exception("Built-in NavigationService cannot be resolved.");
			}

			return navigationService.CanGoBack;
		}

		void GoBackUsingWindow()
		{
			PerformActionOnMainWindowWhenReady(navigationService =>
			{
				navigationService.GoBack();
			});
		}

		void PerformActionOnMainWindowWhenReady(Action<BuiltInNavigation.NavigationService> action)
		{
			var mainWindow = Application.Current.MainWindow;

			if (mainWindow == null)
			{
				if (++attemptsToRetrieveService < 5)
				{
					var context = Dependency.Resolve<ISynchronizationContext>();
					context.Post(() => PerformActionOnMainWindowWhenReady(action));

					return;
				}

				throw new Exception("MainWindow cannot be resolved.");
			}
			
			var navigationService = BuiltInNavigation.NavigationService.GetNavigationService(mainWindow);
			if (navigationService == null)
			{
				var navigationWindow = Application.Current.MainWindow 
											as BuiltInNavigation.NavigationWindow;

				if (navigationWindow == null)
				{
					var frame = Application.Current.MainWindow?.Content as Frame;
					if (frame != null)
					{
						navigationService = frame.NavigationService;
					}
				}
				else
				{
					navigationService = navigationWindow.NavigationService;
				}

				if (navigationService == null)
				{
					throw new Exception(
						"Built-in NavigationService cannot be resolved.");
				}
			}

			action(navigationService);
		}

		public void NavigateUsingMainWindow(Uri uri)
		{
			PerformActionOnMainWindowWhenReady(navigationService =>
			{
				navigationService.Navigate(uri);
			});
		}

		public void GoBack()
		{
			GoBackUsingWindow();
		}

		public bool CanGoBack => CanGoBackUsingWindow();

		public void Navigate(string relativeUrl)
		{
			var routingService = (RoutingService)Dependency.Resolve<IRoutingService>();
			routingService.Navigate(relativeUrl);
		}

		public void Navigate(object page)
		{
			PerformActionOnMainWindowWhenReady(n =>
			{
				n.Navigate(page);
			});
		}
	}
}
#endif