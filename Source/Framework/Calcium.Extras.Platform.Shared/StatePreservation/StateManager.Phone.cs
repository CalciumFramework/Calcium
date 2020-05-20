#if WINDOWS_PHONE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-17 15:50:52Z</CreationDate>
</File>
*/
#endregion

using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Shell;

namespace Calcium.StatePreservation
{
	public class StateManager : IStateManager
	{
		public void Initialize()
		{
			if (PhoneApplicationService.Current == null || !(Application.Current.RootVisual is Frame))
			{
				/* This is invoked in case this call is made before the RootVisual has been assigned. */
				Deployment.Current.Dispatcher.BeginInvoke(InitializeCore);
			}
			else
			{
				InitializeCore();
			}
		}

		void InitializeCore()
		{
			try
			{
				PhoneApplicationService.Current.Deactivated += HandleDeactivated;
				var frame = (Frame)Application.Current.RootVisual;
				frame.Navigating -= HandleNavigating;
				frame.Navigating += HandleNavigating;
				frame.Navigated -= HandleNavigated;
				frame.Navigated += HandleNavigated;
			}
			catch (Exception ex)
			{
				throw new Exception("StateManager Initialize method raised exception.", ex);
			}
		}

		void HandleNavigating(object sender, NavigatingCancelEventArgs e)
		{
			var frame = (Frame)Application.Current.RootVisual;
			var element = frame.Content as FrameworkElement;
			if (element != null && !e.Uri.Equals(frame.CurrentSource))
			{
				ProcessDataContext(element.DataContext, e,
					(preserver, y)
						=> preserver.SaveState(
								IsolatedStorageSettings.ApplicationSettings,
								PhoneApplicationService.Current.State));
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
								PhoneApplicationService.Current.State,
								shouldLoadTransientState));
			}
		}

		void ProcessDataContext<TEventArgs>(object viewModel, TEventArgs e,
			Action<IStatePreservation, TEventArgs> raiseEventAction)
			where TEventArgs : EventArgs
		{
			if (viewModel == null)
			{
				return;
			}

			IStatePreservation statePreserver = viewModel as IStatePreservation;
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

		void HandleDeactivated(object sender, DeactivatedEventArgs e)
		{
			shouldLoadTransientState = false;
		}

		bool shouldLoadTransientState = true;

		public bool ShouldLoadTransientState
		{
			get
			{
				return shouldLoadTransientState;
			}
		}
	}
}
#endif
