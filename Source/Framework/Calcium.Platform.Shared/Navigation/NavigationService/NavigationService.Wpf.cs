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
		object navigationArgument;

		public object NavigationArgument
		{
			get => navigationArgument;
			set => navigationArgument = value;
		}

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
			NavigationArgument = null;

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
				/* When an application is launching, 
				 * the window may not be initialized yet. 
				 * Hence the purpose of the retry. */
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

		public void NavigateUsingMainWindow(Uri uri, object navigationParameter = null)
		{
			NavigationArgument = navigationParameter;

			PerformActionOnMainWindowWhenReady(navigationService =>
			{
				if (navigationParameter != null)
				{
					navigationService.Navigate(uri, navigationParameter);
				}
				else
				{
					navigationService.Navigate(uri);
				}
			});
		}

		public void GoBack()
		{
			GoBackUsingWindow();
		}

		public bool CanGoBack => CanGoBackUsingWindow();

		public void Navigate(string relativeUrl, object navigationArgument = null)
		{
			NavigationArgument = navigationArgument;

			var routingService = (RoutingService)Dependency.Resolve<IRoutingService>();
			routingService.Navigate(relativeUrl, navigationArgument);
		}

		public void Navigate(object page, object navigationArgument = null)
		{
			NavigationArgument = navigationArgument;

			PerformActionOnMainWindowWhenReady(n =>
			{
				n.Navigate(page, navigationArgument);
			});
		}
	}
}
#endif