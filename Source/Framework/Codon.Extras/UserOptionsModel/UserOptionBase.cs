#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-23 17:06:31Z</CreationDate>
</File>
*/
#endregion

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codon.UserOptionsModel
{
	/// <summary>
	/// The base class for a generic <see cref="UserOptionBase"/>.
	/// </summary>
	/// <typeparam name="TSetting">
	/// The type of the setting that serves as the backing field
	/// for this user option.</typeparam>
	public abstract class UserOptionBase<TSetting> 
		: UserOptionBase, IOptionStorage<TSetting>, IUserOption
	{

		Func<TSetting, Task<SaveOptionResult>> saveSettingFunc;

		public Func<TSetting, Task<SaveOptionResult>> SaveSettingFunc
		{
			get => saveSettingFunc;
			set => saveSettingFunc = value;
		}

		Func<Task<TSetting>> getSettingFunc;

		public Func<Task<TSetting>> GetSettingFunc
		{
			get => getSettingFunc;
			set => getSettingFunc = value;
		}

		public Func<TSetting> DefaultValueFunc { get; set; }


		/// <summary>
		/// Gets or sets the validate func, which is used to validate user input.
		/// </summary>
		/// <value>
		/// The validate func.
		/// </value>
		public Func<TSetting, ValidationResult> ValidateFunc { get; set; }

		/// <summary>
		/// Creates a new user option.
		/// </summary>
		/// <param name="titleFunc">The title func, which is used to retrieve 
		/// the title of the option that is displayed on the options view.</param>
		/// <param name="settingKey">The setting key, which is used to retrieve 
		/// and save the setting to the settings service.</param>
		/// <param name="defaultValueFunc">The default value func, 
		/// which is used to retrieve the default value of the setting 
		/// if not setting has been previously saved to the settings service.</param>
		/// <param name="saveSettingFunc">A custom action to save the setting.
		/// This is used to override the default behavour or to add further logic.</param>
		/// <param name="getSettingFunc">A custom action to retrieve the setting.
		/// This is used to override the default behavour or to add further logic.</param>
		protected UserOptionBase(
			Func<string> titleFunc, 
			string settingKey,
			Func<TSetting> defaultValueFunc = null,
			Func<TSetting, Task<SaveOptionResult>> saveSettingFunc = null,
			Func<Task<TSetting>> getSettingFunc = null)
			: base(typeof(TSetting), titleFunc, settingKey)
		{
			DefaultValueFunc = defaultValueFunc;
			SaveSettingFunc = saveSettingFunc;
			GetSettingFunc = getSettingFunc;
		}

		protected UserOptionBase(
			Func<string> titleFunc,
			Func<TSetting> defaultValueFunc,
			Func<TSetting, Task<SaveOptionResult>> saveSettingFunc,
			Func<Task<TSetting>> getSettingFunc)
			: base(typeof(TSetting), titleFunc)
		{
			DefaultValueFunc = defaultValueFunc;
			SaveSettingFunc = AssertArg.IsNotNull(saveSettingFunc, nameof(saveSettingFunc));
			GetSettingFunc = AssertArg.IsNotNull(getSettingFunc, nameof(getSettingFunc));
		}

		public override object DefaultValue => DefaultValueFunc != null 
			? DefaultValueFunc() : default(TSetting);

		public bool CanGetSetting => getSettingFunc != null;

		public async Task<TSetting> GetSetting()
		{
			TSetting newValue;
			preventGetSettingFromBeingCalled = true;

			if (getSettingFunc == null)
			{
				if (ReaderWriter == null)
				{
					throw new Exception("ReaderWriter is null and no custom get setting func provided.");
				}

				newValue = (TSetting)await ReaderWriter.GetSettingAsync();
			}
			else
			{
				newValue = await getSettingFunc();
			}

			if (!Equals(newValue, lastSetting))
			{
				lastSetting = newValue;
				OnPropertyChanged(nameof(Setting));
				OnPropertyChanged(nameof(FormattedSetting));
			}

			preventGetSettingFromBeingCalled = false;
			return lastSetting;
		}

		public bool CanSaveSetting => saveSettingFunc != null;

		public async Task<SaveOptionResult> SaveSetting(TSetting setting)
		{
			if (saveSettingFunc == null)
			{
				if (ReaderWriter == null)
				{
					throw new Exception("No custom save setting action provided.");
				}

				/* OnPropertyChanged for Setting property is raised next. */
				ReaderWriter.Setting = setting;
				
				return new SaveOptionResult(); //await ReaderWriter.Save();
			}

			var saveResult = await saveSettingFunc(setting);
			if (saveResult.ResultValue == SaveOptionResultValue.Success)
			{
				preventGetSettingFromBeingCalled = true;
				lastSetting = setting;

				OnPropertyChanged(nameof(Setting));
				OnPropertyChanged(nameof(FormattedSetting));

				preventGetSettingFromBeingCalled = true;
			}
			return saveResult;
		}

		Func<object, string> formatSettingFunc;

		public Func<object, string> FormatSettingFunc
		{
			get => formatSettingFunc;
			set
			{
				if (formatSettingFunc != value)
				{
					formatSettingFunc = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(FormattedSetting));
				}
			}
		}

		public override string FormattedSetting
		{
			get
			{
				if (formatSettingFunc != null)
				{
					return formatSettingFunc(Setting);
				}

				return Setting?.ToString();
			}
		}

		object IUserOption.Setting
		{
			get => Setting;
			set => Setting = (TSetting)value;
		}

		public TSetting Setting
		{
			get
			{
				if (!preventGetSettingFromBeingCalled)
				{
					preventGetSettingFromBeingCalled = true;
					GetSetting();
				}

				return lastSetting;
			}
			set
			{
				SaveSetting(value);
			}
		}

		TSetting lastSetting;
		bool preventGetSettingFromBeingCalled;

		internal override void HandleSettingChanged(IUserOptionReaderWriter writer)
		{
			try
			{
				preventGetSettingFromBeingCalled = true;
				lastSetting = (TSetting)writer.Setting;
				OnPropertyChanged(nameof(Setting));
				OnPropertyChanged(nameof(FormattedSetting));
			}
			finally
			{
				preventGetSettingFromBeingCalled = false;
			}
			
			base.HandleSettingChanged(writer);
		}
	}

	public enum SaveOptionResultValue
	{
		Success,
		Failed
	}

	public class SaveOptionResult
	{
		public SaveOptionResultValue ResultValue { get; private set; }
		public object FurtherInformation { get; private set; }

		public SaveOptionResult(
			SaveOptionResultValue resultValue = SaveOptionResultValue.Success, 
			object furtherInformation = null)
		{
			ResultValue = resultValue;
			FurtherInformation = furtherInformation;
		}
	}

	public abstract class UserOptionBase 
		: IUserOption, INotifyPropertyChanged
	{
		readonly Type settingType;

		protected UserOptionBase(Type settingType, Func<string> titleFunc, string settingKey)
		{
			this.settingType = AssertArg.IsNotNull(settingType, nameof(settingType));
			TitleFunc = AssertArg.IsNotNull(titleFunc, nameof(titleFunc));
			SettingKey = AssertArg.IsNotNull(settingKey, nameof(settingKey));
		}

		protected UserOptionBase(Type settingType, Func<string> titleFunc)
		{
			this.settingType = AssertArg.IsNotNull(settingType, nameof(settingType));
			TitleFunc = AssertArg.IsNotNull(titleFunc, nameof(titleFunc));
		}

		bool saveWhenSettingIsChanged = true;

		public bool SaveWhenSettingIsChanged
		{
			get => saveWhenSettingIsChanged;
			set => saveWhenSettingIsChanged = value;
		}

		internal virtual void HandleSettingChanged(IUserOptionReaderWriter writer)
		{
			if (SaveWhenSettingIsChanged)
			{
				writer.Save();
			}

			OnPropertyChanged(nameof(FormattedSetting));
		}

		public Func<string> TitleFunc { get; private set; }

		public string SettingKey { get; private set; }

		public string Title => TitleFunc();

		/// <summary>
		/// Gets or sets the description func, which is used to retrieve 
		/// a description of the option that may be displayed to the user.
		/// </summary>
		/// <value>
		/// The description func.
		/// </value>
		public Func<string> DescriptionFunc { get; set; }

		public string Description => DescriptionFunc?.Invoke();

		/// <summary>
		/// Gets or sets the template name func, 
		/// which is used to display the option in the option view.
		/// </summary>
		/// <value>
		/// The template name func.
		/// </value>
		public Func<string> TemplateNameFunc { get; set; }

		public string TemplateName => TemplateNameFunc?.Invoke();

		public abstract object DefaultValue { get; }

		IUserOptionReaderWriter readerWriter;

		public IUserOptionReaderWriter ReaderWriter
		{
			get
			{
				if (readerWriter == null)
				{
					var strategy = Dependency.Resolve<IReaderWriterCreationStrategy, ReaderWriterCreationStrategy>(false);
					readerWriter = strategy.Create(this);
				}

				return readerWriter;
			}
			set
			{
				if (readerWriter != value)
				{
					if (readerWriter != null)
					{
						readerWriter.UserOption = null;
					}
					readerWriter = value;
				}
			}
		}

		public Task Refresh(bool reload = false)
		{
			OnPropertyChanged(string.Empty);

			return Task.CompletedTask;
		}

		public Type SettingType => settingType;
		public abstract string FormattedSetting { get; }
		public virtual object Setting
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

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

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		Func<bool> readOnlyFunc;

		public Func<bool> ReadOnlyFunc
		{
			get => readOnlyFunc;
			set
			{
				if (readOnlyFunc != value)
				{
					readOnlyFunc = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(ReadOnly));
				}
			}
		}

		public bool ReadOnly
		{
			get
			{
				if (readOnlyFunc != null)
				{
					return readOnlyFunc();
				}

				return false;
			}
		}

		//		string settingStringFormat;
		//
		//		public string SettingStringFormat
		//		{
		//			get => settingStringFormat ?? "{0}";
		//			set
		//			{
		//				if (settingStringFormat != value)
		//				{
		//					settingStringFormat = value;
		//					OnPropertyChanged();
		//				}
		//			}
		//		}
	}
}