#if WPF || WINDOWS_UWP
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-09-21 17:24:28Z</CreationDate>
</File>
*/
#endregion

using System;
using System.ComponentModel;
using System.Linq;

#if WINDOWS_UWP || NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#endif

namespace Calcium.UI.Elements
{
	/// <summary>
	/// This class is used by UWP and WPF to enable 
	/// Silverlight like <c>INotifyDataErrorInfo</c> behaviour.
	/// </summary>
	public static class PropertyValidation
	{
#region ValidateProperty
		public static readonly DependencyProperty ValidateProperty
			= DependencyProperty.RegisterAttached(
				"Validate",
				typeof(string),
				typeof(PropertyValidation),
#if WINDOWS_UWP || NETFX_CORE
				new PropertyMetadata(null, OnValidatePropertyChanged));
#else
				new PropertyMetadata(OnValidatePropertyChanged));
#endif

		public static void SetValidate(Control obj, string value)
		{
			obj.SetValue(ValidateProperty, value);
		}

		public static string GetValidate(Control obj)
		{
			return (string)obj.GetValue(ValidateProperty);
		}
#endregion

#region ValidationBehaviorProperty
		static readonly DependencyProperty ValidationBehaviorProperty
			= DependencyProperty.RegisterAttached(
				"ValidationBehavior",
				typeof(ValidationBehavior),
				typeof(PropertyValidation),
				null);

		public static void SetValidationBehavior(Control obj, ValidationBehavior value)
		{
			obj.SetValue(ValidationBehaviorProperty, value);
		}

		public static ValidationBehavior GetValidationBehavior(Control obj)
		{
			return (ValidationBehavior)obj.GetValue(ValidationBehaviorProperty);
		}
#endregion

		static void OnValidatePropertyChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Control control = (Control)d;
			control.DataContextChanged -= OnDataContextChanged;
			control.DataContextChanged += OnDataContextChanged;
		}

		static ValidationBehavior GetOrCreateValidationBehavior(Control d)
		{
			var behaviour = GetValidationBehavior(d);
			if (behaviour == null)
			{
				behaviour = new ValidationBehavior(d);
				SetValidationBehavior(d, behaviour);
			}
			return behaviour;
		}

#if WINDOWS_UWP || NETFX_CORE
		static void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
		{
			HandleDataContextChanged((Control)sender, null, (INotifyDataErrorInfo)e.NewValue);
		}
#else
		static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			HandleDataContextChanged((Control)sender, (INotifyDataErrorInfo)e.OldValue, (INotifyDataErrorInfo)e.NewValue);
		}
#endif

		static void HandleDataContextChanged(Control sender, INotifyDataErrorInfo oldInfo, INotifyDataErrorInfo newInfo)
		{
			ValidationBehavior behavior = GetOrCreateValidationBehavior(sender);
			if (oldInfo != null)
			{
				behavior.Unsubscribe(oldInfo);
			}

			if (newInfo != null)
			{
				behavior.Unsubscribe(newInfo);
				behavior.Subscribe(newInfo);
			}
		}

		public class ValidationBehavior
		{
			WeakReference owner;

			public Control Owner
			{
				get => (Control)owner?.Target;
				private set => owner = new WeakReference(value);
			}

			public ValidationBehavior(Control owner)
			{
				Owner = AssertArg.IsNotNull(owner, nameof(owner));
			}

			public void Subscribe(INotifyDataErrorInfo errorNotifier)
			{
				AssertArg.IsNotNull(errorNotifier, nameof(errorNotifier));

				errorNotifier.ErrorsChanged += OnErrorsChanged;
			}

			public void Unsubscribe(INotifyDataErrorInfo errorNotifier)
			{
				AssertArg.IsNotNull(errorNotifier, nameof(errorNotifier));

				errorNotifier.ErrorsChanged -= OnErrorsChanged;
			}	

			void OnErrorsChanged(object sender, DataErrorsChangedEventArgs args)
			{
				var control = Owner;
				if (control == null)
				{
					Unsubscribe((INotifyDataErrorInfo)sender);
					return;
				}

				string propertyName = null;
				var textBox = control as TextBox;
				if (textBox != null)
				{
					BindingExpression expression
						= textBox.GetBindingExpression(TextBox.TextProperty);
					if (expression?.ParentBinding?.Path != null)
					{
						propertyName = expression.ParentBinding.Path.Path;
					}
				}

				if (string.IsNullOrEmpty(propertyName))
				{
					propertyName = (string)control.GetValue(ValidateProperty);
				}

				INotifyDataErrorInfo info = (INotifyDataErrorInfo)sender;
				var errors = info.GetErrors(propertyName);
				bool hasErrors = errors != null && errors.Cast<object>().Any();

				VisualStateManager.GoToState(control,
						   hasErrors ? "InvalidUnfocused" : "Valid", true);
			}

		}
	}
}
#endif
