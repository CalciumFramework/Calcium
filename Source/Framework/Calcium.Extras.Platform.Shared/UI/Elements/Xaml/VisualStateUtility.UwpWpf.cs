#if WINDOWS_UWP || WPF || WINDOWS_PHONE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-08-21 16:16:19Z</CreationDate>
</File>
*/
#endregion

using System;

#if WINDOWS_UWP || NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace Calcium.UI.Elements
{
	public static class VisualStateUtility
	{
		public static readonly DependencyProperty VisualStateProperty
			= DependencyProperty.RegisterAttached(
				"VisualState",
				typeof(String),
				typeof(VisualStateUtility),
#if WINDOWS_UWP || NETFX_CORE
				new PropertyMetadata(null, HandleVisualStateChanged));
#else
				new PropertyMetadata(HandleVisualStateChanged));
#endif

		public static string GetVisualState(DependencyObject obj)
		{
			return (string)obj.GetValue(VisualStateProperty);
		}

		public static void SetVisualState(DependencyObject obj, string value)
		{
			obj.SetValue(VisualStateProperty, value);
		}

		static void HandleVisualStateChanged(
			object sender, DependencyPropertyChangedEventArgs args)
		{
			var control = sender as Control;

			if (control == null)
			{
				throw new ArgumentException(
					"VisualState is only supported for Controls.");
			}

			string stateName = args.NewValue?.ToString();
			if (stateName != null)
			{
				/* Call is invoked as to avoid missing 
				* the initial state before the control has loaded. */
#if SILVERLIGHT || WINDOWS_PHONE
//                    Deployment.Current.Dispatcher.BeginInvoke(
//                        delegate
//                        {
	                        try
	                        {
		                        VisualStateManager.GoToState(control, stateName, true);
	                        }
	                        catch (Exception ex)
	                        {
								if (Debugger.IsLogging())
								{
									Debugger.Log(1, "VisualStateUtility", 
										string.Format("Unable to transition to state: {0} exception:{1}", stateName, ex));
								}
	                        }                            
//                        });
#else
				System.Threading.SynchronizationContext.Current.Post(
					_ => VisualStateManager.GoToState(control, stateName, true), null);
#endif
			}
		}
	}
}
#endif
