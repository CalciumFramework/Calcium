#if WPF || WINDOWS_PHONE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-01-04 15:14:10Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Calcium.Logging;

namespace Calcium.UI.Elements
{
	public static class FocusForcer
	{
		/// <summary>
		/// Focuses the specified control.
		/// If the control resides in a <see cref="TabControl"/>
		/// the <see cref="TabItem"/> is made visible
		/// and the control is focussed. 
		/// </summary>
		/// <param name="control">The control.</param>
		public static void Focus(UIElement control)
		{
			AssertArg.IsNotNull(control, nameof(control));
			DependencyObject parent;
			var childControl = control as FrameworkElement;
			if (childControl == null)
			{
				/* What else can we do in this scenario? */
				FocusControl(control);
				return;
			}

			var listBoxItem = control as ListBoxItem;
			if (listBoxItem != null)
			{
				var itemParent = (VirtualizingStackPanel)VisualTreeHelper.GetParent(listBoxItem);
				childControl = ItemsControl.GetItemsOwner(itemParent);
			}

			var expander = control as Expander;
			if (expander != null)
			{
				expander.IsExpanded = true;
			}

			while (childControl != null && (parent = childControl.Parent) != null)
			{
				var tabItem = parent as TabItem;
				if (tabItem != null)
				{
					var tabControl = tabItem.Parent as TabControl;
					if (tabControl != null)
					{
						tabControl.SelectedItem = tabItem;
						break;
					}
				}

				expander = parent as Expander;
				if (expander != null)
				{
					expander.IsExpanded = true;
				}

				childControl = parent as FrameworkElement;
				if (childControl == null)
				{
					return; /* Shouldn't get here. */
				}
			}

			listBoxItem?.BringIntoView();

			FocusControl(control);
		}

		static void FocusControl(UIElement element)
		{
			AssertArg.IsNotNull(element, nameof(element));

			Keyboard.Focus(element);
			var focusResult = element.Focus();

			if (focusResult)
			{
				return;
			}
            
			element.Dispatcher.Invoke(DispatcherPriority.Background, (Action)delegate
				{
					focusResult = element.Focus();
					Keyboard.Focus(element);

					if (!focusResult)
					{
						CommitFocusedElement();
						focusResult = element.Focus();
						Keyboard.Focus(element);
					}

					if (!focusResult)
					{
						WriteLog(string.Format("Unable to focus UIElement {0} " 
							+ "IsVisible: {1}, Focusable: {2}, Enabled: {3}",
							element, element.IsVisible, element.Focusable, 
							element.IsEnabled));
					}
				});
		}

		static void WriteLog(string message, Exception exception = null)
		{
			ILog log;
			if (Dependency.TryResolve(out log))
			{
				log.Warn(message, exception);
			}
		}

		/* FROM: http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/087cfa7e-3cdc-4c50-8d75-e375d47d5015/
		 Tor Langlo*/
		/// <summary>
		/// Force a focus change away from the currently focused element (if any) to ensure that
		/// field changes are commited to databound sources.
		/// </summary>
		static void CommitFocusedElement()
		{
			var focusedElement = Keyboard.FocusedElement as UIElement;

			if (focusedElement != null)
			{
				var traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);

				if (!focusedElement.MoveFocus(traversalRequest))
				{
					traversalRequest = new TraversalRequest(FocusNavigationDirection.Previous);
					focusedElement.MoveFocus(traversalRequest);
				}

			}

		}
	}
}
#endif
