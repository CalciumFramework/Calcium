#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codon.UIModel.Input
{
	/// <summary>
	/// This class allows you to activate a command
	/// from a list of commands. When a command is selected
	/// all calls to properties of the composite command
	/// are passed through to the selected command.
	/// This class is particularly useful when you wish 
	/// to bind multiple commands to a single button for example,
	/// and switch its text, icon, and behaviour dynamically.
	/// </summary>
	public class UICompositeCommand : IUICommand, INotifyPropertyChanged
	{
		IUICommand selectedCommand;
		readonly List<IUICommand> commands = new List<IUICommand>();
		
		/// <summary>
		/// Gets the readonly list of commands.
		/// </summary>
		public IReadOnlyList<IUICommand> Commands => commands.AsReadOnly();

		public string Id { get; set; }

		readonly Dictionary<IUICommand, object> commandParameters 
			= new Dictionary<IUICommand, object>();

		/// <summary>
		/// Add a command to the list of commands.
		/// </summary>
		/// <param name="command">The command to add.</param>
		/// <param name="parameter">An object that is passed
		/// to the command upon execution and during
		/// property value evaluation.</param>
		public void SetCommandParameter(IUICommand command, object parameter)
		{
			if (parameter != null)
			{
				commandParameters[command] = parameter;
			}
			else
			{
				commandParameters.Remove(command);
			}
		}

		public Task RefreshAsync(object commandParameter = null)
		{
			foreach (var command in commands.OfType<ICommandBase>())
			{
				commandParameters.TryGetValue(
					(IUICommand)command, out object parameter);

				command.Refresh(parameter ?? commandParameter);
			}

			return Task.FromResult<object>(null);
		}

		/// <summary>
		/// Retrieves the selected command from the list of commands.
		/// Calls to <see cref="IUICommand"/> properties
		/// are passed through to the selected command,
		/// as are calls to execute and so forth.
		/// </summary>
		public IUICommand SelectedCommand
		{
			get => selectedCommand;
			set
			{
				AssertArg.IsNotNull(value, nameof(value));

				if (value == selectedCommand)
				{
					return;
				}

				if (!commands.Contains(value))
				{
					throw new ArgumentOutOfRangeException(nameof(value),
						"Command is not contained within the collection.");
				}

				if (selectedCommand != null)
				{
					StopMonitoringCommand(selectedCommand);
				}
				selectedCommand = value;
				MonitorCommand(selectedCommand);

				OnPropertyChanged(string.Empty);
			}
		}

		/// <summary>
		/// Retrieves the selected command's index
		/// from the list of commands.
		/// Calls to <see cref="IUICommand"/> properties
		/// are passed through to the selected command,
		/// as are calls to execute and so forth.
		/// </summary>
		public int SelectedCommandIndex
		{
			get
			{
				if (selectedCommand == null)
				{
					return -1;
				}

				return commands.IndexOf(selectedCommand);
			}
			set
			{
				if (value < 0 || value > commands.Count - 1)
				{
					throw new ArgumentOutOfRangeException();
				}

				SelectedCommand = commands[value];
			}
		}

		void MonitorCommand(IUICommand uiCommand)
		{
			var inpc = uiCommand as INotifyPropertyChanged;
			if (inpc == null)
			{
				return;
			}

			inpc.PropertyChanged -= HandleSelectedCommandPropertyChanged;
			inpc.PropertyChanged += HandleSelectedCommandPropertyChanged;
		}

		void StopMonitoringCommand(IUICommand uiCommand)
		{
			var inpc = uiCommand as INotifyPropertyChanged;
			if (inpc == null)
			{
				return;
			}

			inpc.PropertyChanged -= HandleSelectedCommandPropertyChanged;
		}

		void HandleSelectedCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}

		public UICompositeCommand(params IUICommand[] commands)
		{
			this.commands.AddRange(commands);

			if (commands.Any())
			{
				SelectedCommandIndex = 0;
			}
		}

		public bool CanExecuteDefaultValue { get; set; }

		public bool CanExecute(object parameter)
		{
			if (selectedCommand != null)
			{
				return selectedCommand.CanExecute(parameter);
			}

			return CanExecuteDefaultValue;
		}

		public void Execute(object parameter)
		{
			selectedCommand?.Execute(parameter);
		}

#pragma warning disable CS0067 // The event 'UICompositeCommand.CanExecuteChanged' is never used
		public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // The event 'UICompositeCommand.CanExecuteChanged' is never used

		public string TextDefaultValue { get; set; }

		public string Text
		{
			get
			{
				if (selectedCommand != null)
				{
					return selectedCommand.Text;
				}

				return TextDefaultValue;
			}
		}

		bool visibleFallback = true;

		/// <summary>
		/// If there is no selected command, this value 
		/// determines the visibility of the composite command.
		/// </summary>
		public bool VisibleFallback
		{
			get => visibleFallback;
			set => visibleFallback = value;
		}

		public bool Visible
		{
			get
			{
				if (selectedCommand != null)
				{
					return selectedCommand.Visible;
				}

				return visibleFallback;
			}
		}
		
		public bool Enabled
		{
			get
			{
				if (selectedCommand != null)
				{
					return selectedCommand.Enabled;
				}

				return true;
			}
		}

		public string IconUrl => selectedCommand?.IconUrl;

		public string IconCharacter => selectedCommand?.IconCharacter;

		public string IconFont => selectedCommand?.IconFont;

		public bool IsChecked
		{
			get
			{
				if (selectedCommand != null)
				{
					return selectedCommand.IsChecked;
				}

				return false;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(
			[CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, 
				new PropertyChangedEventArgs(propertyName));
		}

		public void RaiseCanExecuteChanged()
		{
			selectedCommand?.RaiseCanExecuteChanged();
		}

		public void Refresh(object commandParameter)
		{
			selectedCommand?.Refresh(commandParameter);
		}
	}
}