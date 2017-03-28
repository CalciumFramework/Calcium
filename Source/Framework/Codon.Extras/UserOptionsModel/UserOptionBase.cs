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
	public abstract class UserOptionBase<TSetting> : UserOptionBase, IOptionStorage<TSetting>
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
			//TitleFunc = AssertArg.IsNotNull(getTitleAction, nameof(getTitleAction));
			//SettingKey = AssertArg.IsNotNull(settingKey, nameof(settingKey));
			DefaultValueFunc = defaultValueFunc;
			SaveSettingFunc = saveSettingFunc;
			GetSettingFunc = getSettingFunc;
		}

//		/// <summary>
//		/// Creates a new user option.
//		/// </summary>
//		/// <param name="titleFunc">The title func, which is used to retrieve 
//		/// the title of the option that is displayed on the options view.</param>
//		/// <param name="settingKey">The setting key, which is used to retrieve 
//		/// and save the setting to the settings service.</param>
//		/// <param name="saveSettingFunc">A func to save the result. 
//		/// This will typically be a call to the ISettingsService.</param>
//		/// <param name="defaultValueFunc">The default value func, 
//		/// which is used to retrieve the default value of the setting 
//		/// if not setting has been previously saved to the settings service.</param>
//		protected UserOptionBase(
//			Func<string> titleFunc,
//			Func<TSetting> defaultValueFunc = null)
//		{
//			TitleFunc = AssertArg.IsNotNull(getTitleAction, nameof(getTitleAction));
////			this.getSettingFunc = AssertArg.IsNotNull(getSettingFunc, nameof(getSettingFunc));
////			this.saveSettingFunc = AssertArg.IsNotNull(saveSettingFunc, nameof(saveSettingFunc));
//
//			DefaultValueFunc = defaultValueFunc;
//		}

		public override object DefaultValue => DefaultValueFunc != null ? DefaultValueFunc() : default(TSetting);

		//		public async void Save()
//		{
//			Action<Task<TSetting>> action = saveSettingFunc;
//			if (action != null)
//			{
//				await action();
//			}
//		}


		public bool CanGetSetting => getSettingFunc != null;

#if !MONODROID && !__IOS__
		public async Task<TSetting> GetSetting()
		{
			if (getSettingFunc == null)
			{
				throw new Exception("No custom get setting func provided.");
			}

			return await getSettingFunc();
		}
#else
		public Task<TSetting> GetSetting()
		{
			if (getSettingFunc == null)
			{
				throw new Exception("No custom get setting func provided.");
			}

			Task<TSetting> task = getSettingFunc();
			task.Wait();
			TSetting result = task.Result;
			return task;
		}
#endif

		public bool CanSaveSetting => saveSettingFunc != null;

		public async Task<SaveOptionResult> SaveSetting(TSetting setting)
		{
			if (saveSettingFunc == null)
			{
				throw new Exception("No custom save setting action provided.");
			}

			return await saveSettingFunc(setting);
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

	public abstract class UserOptionBase : IUserOption, INotifyPropertyChanged
	{
		readonly Type settingType;

		public UserOptionBase(Type settingType, Func<string> titleFunc, string settingKey)
		{
			this.settingType = AssertArg.IsNotNull(settingType, nameof(settingType));
			TitleFunc = AssertArg.IsNotNull(titleFunc, nameof(titleFunc));
			SettingKey = AssertArg.IsNotNull(settingKey, nameof(settingKey));
		}

		bool saveWhenSettingIsChanged = true;

		public bool SaveWhenSettingIsChanged
		{
			get => saveWhenSettingIsChanged;
			set => saveWhenSettingIsChanged = value;
		}

		internal void HandleSettingChanged(IUserOptionReaderWriter writer)
		{
			if (SaveWhenSettingIsChanged)
			{
				writer.Save();
			}
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

		public IUserOptionReaderWriter ReaderWriter { get; set; }

		public Task Refresh(bool reload = false)
		{
			OnPropertyChanged(string.Empty);

			return Task.FromResult(0);
		}

		public Type SettingType => settingType;

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
	}
}