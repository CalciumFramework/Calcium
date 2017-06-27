using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Codon.UserOptionsModel.UserOptions
{
	/// <summary>
	/// This option allows you to place, for example,
	/// a button within your options list. When the button
	/// is activated, the command is executed.
	/// </summary>
	public class CommandOption : CommandOption<object>
	{
		public CommandOption(Func<string> titleFunc, ICommand command, 
			Func<CommandOption<object>, Task<SaveOptionResult>> saveAction = null, 
			Func<object> defaultValueFunc = null)
			: base(titleFunc, command, saveAction, defaultValueFunc)
		{
			/* Intentionally left blank. */
		}
	}

	/// <summary>
	/// This option allows you to place, for example,
	/// a button within your options list. When the button
	/// is activated, the command is executed.
	/// </summary>
	public class CommandOption<TSetting> : IUserOption, IUserOptionReaderWriter, INotifyPropertyChanged
	{
		public CommandOption(Func<string> titleFunc, ICommand command,
			Func<CommandOption<TSetting>, Task<SaveOptionResult>> saveAction = null,
			Func<TSetting> defaultValueFunc = null)
		{
			TitleFunc = AssertArg.IsNotNull(titleFunc, nameof(titleFunc));
			Command = AssertArg.IsNotNull(command, nameof(command));

			DefaultValueFunc = defaultValueFunc;
			//ExecuteAction = AssertArg.IsNotNull(executeAction, nameof(executeAction));
			SaveAction = saveAction;
		}

		public ICommand Command { get; private set; }

		public Func<string> TitleFunc { get; private set; }

		public string Title => TitleFunc();

		public Func<TSetting> DefaultValueFunc { get; private set; }

		public object DefaultValue => DefaultValueFunc != null
			? (object)DefaultValueFunc()
			: null;

		public IUserOptionReaderWriter ReaderWriter
		{
			get => this;
			set
			{
			}
		}

		TSetting optionValue;

		public TSetting Value
		{
			get => optionValue;
			set
			{
				if (!Equals(optionValue, value))
				{
					optionValue = value;
					OnPropertyChanged();
				}
			}
		}

		//public Action ExecuteAction { get; private set; }
		//
		//public void Execute()
		//{
		//	ExecuteAction();
		//}

		/// <summary>
		/// Gets or sets the description func, which is used to retrieve 
		/// a description of the option that may be displayed to the user.
		/// </summary>
		/// <value>
		/// The description func.
		/// </value>
		public Func<string> DescriptionFunc { get; set; }

		public string Description => DescriptionFunc?.Invoke();

		/* Unused. */
		public string SettingKey { get; set; }

		string templateName = "Command";

		public string TemplateName
		{
			get => templateName;
			set
			{
				if (templateName != value)
				{
					templateName = value;
					OnPropertyChanged();
				}
			}
		}

		public Func<Task> RefreshAction { get; set; }

		public IUserOption UserOption
		{
			get => this;
			set
			{
			}
		}

		bool dirty;

		public bool Dirty
		{
			get => dirty;
			protected set
			{
				if (dirty != value)
				{
					dirty = value;
					OnPropertyChanged();
				}
			}
		}

		public Task<object> GetSettingAsync()
		{
			throw new NotSupportedException();
		}

		public object Setting { get; set; }

		public Func<CommandOption<TSetting>, Task<SaveOptionResult>> SaveAction { get; private set; }

		public async Task<SaveOptionResult> Save()
		{
			if (SaveAction != null)
			{
				return await SaveAction(this);
			}

			return new SaveOptionResult();
		}

		public async Task Refresh(bool reload = false)
		{
			OnPropertyChanged("Title");

			if (RefreshAction != null)
			{
				await RefreshAction();
			}
		}

		public Type SettingType => null;

		public string FormattedSetting => Title;

		string imagePath;

		public string ImagePath
		{
			get => imagePath;
			set
			{
				if (imagePath != value)
				{
					imagePath = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(
			[CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(
				this, new PropertyChangedEventArgs(propertyName));
		}
	}
}