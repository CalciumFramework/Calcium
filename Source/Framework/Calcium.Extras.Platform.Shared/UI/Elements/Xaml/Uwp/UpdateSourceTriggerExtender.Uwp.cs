#if WINDOWS_UWP || NETFX_CORE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/
#endregion

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Calcium.UI.Elements
{
	public class UpdateSourceTriggerExtender
	{
		public static readonly DependencyProperty UpdateSourceOnTextChanged
			= DependencyProperty.RegisterAttached(
				"UpdateSourceOnTextChanged", typeof(bool),
				typeof(UpdateSourceTriggerExtender),
				new PropertyMetadata(false, HandleUpdatePropertyChanged));

		public static bool GetUpdateSourceOnTextChanged(DependencyObject d)
		{
			return (bool)d.GetValue(UpdateSourceOnTextChanged);
		}

		public static void SetUpdateSourceOnTextChanged(
			DependencyObject d, bool value)
		{
			d.SetValue(UpdateSourceOnTextChanged, value);
		}

		static void HandleUpdatePropertyChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextBox textBox = d as TextBox;
			if (textBox != null)
			{
				if ((bool)e.OldValue)
				{
					textBox.TextChanged -= HandleTextBoxTextChanged;
					textBox.LostFocus -= HandleBoxLostFocus;
				}

				if ((bool)e.NewValue)
				{
					textBox.TextChanged += HandleTextBoxTextChanged;
					textBox.LostFocus += HandleBoxLostFocus;
				}
				return;
			}

			PasswordBox passwordBox = d as PasswordBox;
			if (passwordBox == null)
			{
				throw new Exception("UpdateSourceTrigger can only be used "
									+ "on a TextBox or PasswordBox.");
			}

			/* Wire up for password box. */
			if ((bool)e.OldValue)
			{
				passwordBox.PasswordChanged -= HandlePasswordBoxTextChanged;
				passwordBox.LostFocus -= HandleBoxLostFocus;
			}

			if ((bool)e.NewValue)
			{
				passwordBox.PasswordChanged += HandlePasswordBoxTextChanged;
				passwordBox.LostFocus += HandleBoxLostFocus;
			}
		}

		static void HandlePasswordBoxTextChanged(object sender, RoutedEventArgs e)
		{
			UpdateSource((PasswordBox)sender, PasswordBox.PasswordProperty);
		}

		static void HandleTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateSource((TextBox)sender, TextBox.TextProperty);
		}

		static void UpdateSource(
			FrameworkElement element, DependencyProperty property)
		{
			if (element == null)
			{
				return;
			}

			var bindingExpression = element.GetBindingExpression(property);
			bindingExpression?.UpdateSource();
		}

		static void HandleBoxLostFocus(object sender, RoutedEventArgs e)
		{
			/* This method prevents the control from being placed 
			 * into the valid state when it loses focus. */
			var control = sender as Control;
			if (control == null)
			{
				return;
			}
			bool hasError = Validation.GetHasError(control);
			if (hasError)
			{
				VisualStateManager.GoToState(control, "InvalidFocused", false);
			}
		}
	}
}

#endif
