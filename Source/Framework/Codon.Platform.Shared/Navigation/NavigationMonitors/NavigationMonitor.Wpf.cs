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
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Windows;
using System.Windows.Navigation;

using Codon.Concurrency;
using Codon.Services;
using Codon.UIModel;
using BuiltInNavigation = System.Windows.Navigation;

namespace Codon.Navigation
{
	public class NavigationMonitor : INavigationMonitor
	{
		int attemptsToRetrieveService;

		public void Initialize()
		{
			var mainWindow = Application.Current.MainWindow;

			if (mainWindow == null)
			{
				if (++attemptsToRetrieveService < 5)
				{
					var context = Dependency.Resolve<ISynchronizationContext>();
					context.Post(Initialize);
				}

				return;
			}

			var navigationService = BuiltInNavigation.NavigationService.GetNavigationService(mainWindow);

			if (navigationService != null)
			{
				navigationService.Navigating -= HandleNavigating;
				navigationService.Navigating += HandleNavigating;
				navigationService.Navigated -= HandleNavigated;
				navigationService.Navigated += HandleNavigated;
			}
		}

		void HandleNavigating(object sender, NavigatingCancelEventArgs e)
		{
			if (e.NavigationMode == BuiltInNavigation.NavigationMode.Back)
			{
				return;
			}

			ProcessDataContext(e,
				(navigationAware, eventArgs, parent) =>
				{
					if (eventArgs.Cancel)
					{
						return;
					}

					var mode = Translate(e.NavigationMode);
					var args = new NavigatingArgs(e.Uri, mode, true, e.IsNavigationInitiator, e.ExtraData, e);

					var messenger = Dependency.Resolve<IMessenger>();
					messenger.PublishAsync(new NavigatingMessage(args));

					navigationAware.HandleNavigatingFrom(args);

					if (eventArgs.Cancel)
					{
						parent?.ActivateViewModel((ViewModelBase)navigationAware);
					}
				});
		}

		void HandleNavigated(object sender, NavigationEventArgs e)
		{
			ProcessDataContext(e,
				(navigationAware, eventArgs, _) =>
				{
					var args = new NavigatedArgs(e.Content, e.Uri, 
										isNavigationInitiator: e.IsNavigationInitiator, 
										parameter: e.ExtraData, 
										builtInArgs: e);
					var messenger = Dependency.Resolve<IMessenger>();
					messenger.PublishAsync(new NavigatedMessage(args));

					navigationAware.HandleNavigatedTo(args);
				});
		}

		void ProcessDataContext<TEventArgs>(TEventArgs e,
			Action<INavigationAware, TEventArgs, ICompositeViewModel> notifyAction)
		{
			object dataContext = GetContentDataContext();
			if (dataContext == null)
			{
				return;
			}

			ProcessDataContext(dataContext, e, notifyAction, null);
		}

		static void ProcessDataContext<TEventArgs>(
			object dataContext, TEventArgs e, Action<INavigationAware, TEventArgs, ICompositeViewModel> notifyAction,
			ICompositeViewModel parent)
		{
			if (dataContext is INavigationAware navigationAware)
			{
				notifyAction(navigationAware, e, parent);
			}

			/* Process child view models recursively. */
			var compositeViewModel = dataContext as ICompositeViewModel;
			if (compositeViewModel?.ChildViewModels != null)
			{
				foreach (var childViewModel in compositeViewModel.ChildViewModels)
				{
					ProcessDataContext(childViewModel, e, notifyAction, compositeViewModel);
				}
			}
		}

		object GetContentDataContext()
		{
			var window = Application.Current.MainWindow;
			FrameworkElement element = window.Content as FrameworkElement;

			return element?.DataContext;
		}

		static NavigationType Translate(BuiltInNavigation.NavigationMode mode)
		{
			switch (mode)
			{
				case BuiltInNavigation.NavigationMode.Back:
					return NavigationType.Back;
				case BuiltInNavigation.NavigationMode.Forward:
					return NavigationType.Forward;
				case BuiltInNavigation.NavigationMode.New:
					return NavigationType.New;
				case BuiltInNavigation.NavigationMode.Refresh:
					return NavigationType.Refresh;
			}

			return NavigationType.Unknown;
		}
	}
}
#endif