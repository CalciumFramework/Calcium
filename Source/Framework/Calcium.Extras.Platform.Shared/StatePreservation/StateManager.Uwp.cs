

using Calcium.Concurrency;
#if WINDOWS_UWP || NETFX_CORE
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
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Calcium.SettingsModel;
using Calcium.UIModel;

namespace Calcium.StatePreservation
{
	public class StateManager : IStateManager
	{
		public void Initialize()
		{
			var frame = Window.Current.Content as Frame;
            if (frame == null)
			{
				/* This is invoked in case this call is made before the RootVisual has been assigned. */
				SynchronizationContext.Post(InitializeCore);
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
				Application.Current.Suspending += HandleSuspending;
				var frame = GetFrame();

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

		Frame GetFrame()
		{
			var result = (Frame)Window.Current.Content;
			return result;
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
			var frame = GetFrame();
			var element = frame.Content as FrameworkElement;
			if (element != null && e.SourcePageType != frame.CurrentSourcePageType)
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

		void HandleSuspending(object sender, SuspendingEventArgs e)
		{
			shouldLoadTransientState = false;
		}

		bool shouldLoadTransientState = true;

		public bool ShouldLoadTransientState => shouldLoadTransientState;

		ISynchronizationContext synchronizationContext;

		/// <summary>
		/// Use this property to override the current synchronization context.
		/// </summary>
		public ISynchronizationContext SynchronizationContext
		{
			get => synchronizationContext ?? (synchronizationContext = UIContext.Instance);
			set => synchronizationContext = value;
		}
	}
}
#endif
