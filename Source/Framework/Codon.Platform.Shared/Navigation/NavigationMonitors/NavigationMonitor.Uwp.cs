#if WINDOWS_UWP || NETFX_CORE
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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Codon.Concurrency;
using Codon.Services;
using Codon.UIModel;

namespace Codon.Navigation
{
	public class NavigationMonitor : INavigationMonitor
	{
		int attemptsToRetrieveService;

		public void Initialize()
		{
			var frame = GetFrame();

			if (frame == null)
			{
				if (++attemptsToRetrieveService < 5)
				{
					var context = Dependency.Resolve<ISynchronizationContext>();
					context.Post(Initialize);
				}
				else
				{
					throw new Exception("Exceeded attempts to retrieve frame.");
				}

				return;
			}

			frame.Navigating -= HandleNavigating;
			frame.Navigating += HandleNavigating;

			frame.Navigated -= HandleNavigated;
			frame.Navigated += HandleNavigated;
		}

		Frame GetFrame()
		{
			return (Frame)Window.Current?.Content;
		}

		void HandleNavigating(object sender, NavigatingCancelEventArgs e)
		{
			if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Back)
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
					var args = new NavigatingArgs(e.SourcePageType, mode);

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
			var frame = GetFrame();
			if (e.SourcePageType != frame.CurrentSourcePageType)
			{
				return;
			}

			
			ProcessDataContext(e,
				(navigationAware, eventArgs, _) =>
				{
					var mode = Translate(eventArgs.NavigationMode);
					var args = new NavigatedArgs(eventArgs.Content, eventArgs.SourcePageType, mode);

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
			var frame = GetFrame();
			FrameworkElement element = frame.Content as FrameworkElement;

			return element?.DataContext;
		}

		static NavigationType Translate(Windows.UI.Xaml.Navigation.NavigationMode mode)
		{
			switch (mode)
			{
				case Windows.UI.Xaml.Navigation.NavigationMode.Back:
					return NavigationType.Back;
				case Windows.UI.Xaml.Navigation.NavigationMode.Forward:
					return NavigationType.Forward;
				case Windows.UI.Xaml.Navigation.NavigationMode.New:
					return NavigationType.New;
				case Windows.UI.Xaml.Navigation.NavigationMode.Refresh:
					return NavigationType.Refresh;
			}

			return NavigationType.Unknown;
		}
	}
}
#endif