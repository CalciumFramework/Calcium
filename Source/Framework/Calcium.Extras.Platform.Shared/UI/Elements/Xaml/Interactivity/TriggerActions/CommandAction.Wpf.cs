#if IGNOREFORNOW && WPF
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-01-30 13:31:10Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Calcium.UI.Elements
{
	public class CommandAction : TriggerAction<FrameworkElement>
	{
		protected override void Invoke(object parameter)
		{
			if (IsAssociatedElementDisabled())
			{
				return;
			}

			ICommand command = Command;
			if (command != null && command.CanExecute(CommandParameter))
			{
				command.Execute(CommandParameter);
			}
		}

		bool IsAssociatedElementDisabled()
		{
#if SILVERLIGHT
			var control = AssociatedObject as Control;
			if (control != null)
			{
				return !control.IsEnabled;
			}
			return false;
#else
			return AssociatedObject != null && !AssociatedObject.IsEnabled;
#endif
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssignEnabledState();
		}

		public static readonly DependencyProperty CommandProperty
			= DependencyProperty.Register(
				"Command",
				typeof(ICommand),
				typeof(CommandAction),
				new PropertyMetadata(null,
					(d, e) =>
					{
						var commandAction = d as CommandAction;
						if (commandAction == null)
						{
							return;
						}

						if (e.OldValue != null)
						{
							((ICommand)e.OldValue).CanExecuteChanged
								-= commandAction.OnCommandCanExecuteChanged;
						}

						var command = e.NewValue as ICommand;

						if (command != null)
						{
							command.CanExecuteChanged
								+= commandAction.OnCommandCanExecuteChanged;
						}

						commandAction.AssignEnabledState();
					}));

		public ICommand Command
		{
			get
			{
				return (ICommand)GetValue(CommandProperty);
			}
			set
			{
				SetValue(CommandProperty, value);
			}
		}

		void AssignEnabledState()
		{
			if (AssociatedObject == null || Command == null)
			{
				return;
			}
#if SILVERLIGHT
			var control = AssociatedObject as Control;
			if (control != null)
			{
				control.IsEnabled = Command.CanExecute(this.CommandParameter);
			}
#else
			AssociatedObject.IsEnabled = Command.CanExecute(this.CommandParameter);
#endif
		}

		void OnCommandCanExecuteChanged(object sender, EventArgs e)
		{
			AssignEnabledState();
		}

		public static readonly DependencyProperty CommandParameterProperty
			= DependencyProperty.Register(
				"CommandParameter",
				typeof(object),
				typeof(CommandAction),
				new PropertyMetadata(null,
					(d, e) =>
					{
						var commandAction = d as CommandAction;
						if (commandAction != null
							&& commandAction.AssociatedObject != null)
						{
							commandAction.AssignEnabledState();
						}
					}));

		public object CommandParameter
		{
			get
			{
				return GetValue(CommandParameterProperty);
			}
			set
			{
				SetValue(CommandParameterProperty, value);
			}
		}
	}
}
#endif
