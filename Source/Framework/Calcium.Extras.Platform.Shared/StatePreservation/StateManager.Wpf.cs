#if WPF
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-17 15:50:52Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

using Calcium.Concurrency;
using Calcium.SettingsModel;
using Calcium.UIModel;

namespace Calcium.StatePreservation
{
	public class StateManager : IStateManager
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
			
			InitializeCore(mainWindow);
		}

		void InitializeCore(Window mainWindow)
		{
			try
			{
				var navigationService = NavigationService.GetNavigationService(mainWindow);

				if (navigationService == null)
				{
					if (Debugger.IsAttached)
					{
						throw new Exception(
							"Unable to resolve built-in NavigationService.");
					}

					return;
				}

				navigationService.Navigating -= HandleNavigating;
				navigationService.Navigating += HandleNavigating;
				navigationService.Navigated -= HandleNavigated;
				navigationService.Navigated += HandleNavigated;
			}
			catch (Exception ex)
			{
				throw new Exception(
					"StateManager Initialize method raised exception.", ex);
			}
		}

		IDictionary<string, object> GetTransientState()
		{
			if (!Dependency.TryResolve(out ITransientState transientState))
			{
				transientState = new TransientState();
				Dependency.Register<ITransientState>(transientState);
			}

			return transientState.StateDictionary;
		} 
		
		void HandleNavigating(object sender, NavigatingCancelEventArgs e)
		{
			var mainWindow = Application.Current.MainWindow;
			var element = mainWindow.Content as FrameworkElement;

			var navigationService = NavigationService.GetNavigationService(mainWindow);

			if (element != null 
				&& (navigationService == null 
					|| e.Uri != navigationService.CurrentSource))
			{
				Dependency.TryResolve(out ITransientState transientState);

				ProcessDataContext(element.DataContext, e,
					(preserver, y) => preserver.SaveState(
								IsolatedStorageSettings.ApplicationSettings,
								GetTransientState()));
			}
		}

		void HandleNavigated(object sender, NavigationEventArgs e)
		{
			FrameworkElement element = e.Content as FrameworkElement;
			if (element != null)
			{
				ProcessDataContext(element.DataContext, e,
					(preserver, y)
						=> preserver.LoadState(
								IsolatedStorageSettings.ApplicationSettings,
								GetTransientState(),
								shouldLoadTransientState));
			}
		}

		void ProcessDataContext<TEventArgs>(object viewModel, TEventArgs e,
			Action<IStateful, TEventArgs> raiseEventAction)
			where TEventArgs : class
		{
			if (viewModel == null)
			{
				return;
			}

			IStateful statePreserver = viewModel as IStateful;
			if (statePreserver != null)
			{
				raiseEventAction(statePreserver, e);
			}

			var compositeViewModel = viewModel as ICompositeViewModel;
			if (compositeViewModel != null)
			{
				/* Process children recursively. */
				foreach (var childViewModel in compositeViewModel.ChildViewModels)
				{
					ProcessDataContext(childViewModel, e, raiseEventAction);
				}
			}
		}

		bool shouldLoadTransientState = true;

		public bool ShouldLoadTransientState => shouldLoadTransientState;
	}
}
#endif
