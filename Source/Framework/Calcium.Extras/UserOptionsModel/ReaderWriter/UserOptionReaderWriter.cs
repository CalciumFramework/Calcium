#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-21 19:12:58Z</CreationDate>
</File>
*/
#endregion

using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

using Calcium.ComponentModel;
using Calcium.Services;
using Calcium.SettingsModel;

namespace Calcium.UserOptionsModel
{
	/// <summary>
	/// This non-generic <see cref="IUserOptionReaderWriter"/>
	/// is able to serialize and retrieve a setting for a 
	/// <see cref="IUserOption"/>.
	/// </summary>
	public class UserOptionReaderWriter : UserOptionReaderWriter<object>
	{
		public UserOptionReaderWriter(IUserOption userOption) : base(userOption)
		{
			/* Intentionally left blank. */
		}
	}

	public class UserOptionReaderWriter<TSetting> 
		: ObservableBase, IUserOptionReaderWriter, ICompositeObjectWriter
	{
		public IUserOption UserOption { get; set; }
		//public string SettingsKey { get; private set; }

		public UserOptionReaderWriter(IUserOption userOption)//, string settingsKey)
		{
			UserOption = AssertArg.IsNotNull(userOption, nameof(userOption));
			userOption.ReaderWriter = this;
			//SettingsKey = AssertArg.IsNotNullOrWhiteSpace(settingsKey, nameof(settingsKey));
		}

		bool dirty;

		public bool Dirty
		{
			get => dirty;
			private set => Set(ref dirty, value);
		}

		void ICompositeObjectWriter.SetDirty()
		{
			Dirty = true;
		}

		bool mayBeDirty;
		bool settingInitialized;
		TSetting setting;

		async Task<object> IUserOptionReaderWriter.GetSettingAsync()
		{
			return await GetSettingAsync();
		}

//#if !MONODROID && !__IOS__ && !NETFX_CORE
		public async Task<TSetting> GetSettingAsync()
		{
			var previousSetting = setting;

			if (!dirty && !settingInitialized && !mayBeDirty)
			{
				var userOption = UserOption;
				var reader = userOption as IOptionStorage<TSetting>;
				if (reader != null && reader.CanGetSetting)
				{
					setting = await reader.GetSetting();
					AttachForPropertyChanged(setting, previousSetting);
				}
				else if (!string.IsNullOrWhiteSpace(userOption.SettingKey))
				{
					setting = (TSetting)SettingsService.GetSetting(
								userOption.SettingKey, typeof(TSetting), UserOption.DefaultValue);
					AttachForPropertyChanged(setting, previousSetting);
				}

				settingInitialized = true;

				OnPropertyChanged(nameof(Setting));
			}

			return setting;
		}

		readonly bool settingNotifiesPropertyChanged = typeof(INotifyPropertyChanged).GetTypeInfo().IsAssignableFrom(typeof(TSetting).GetTypeInfo());

		void AttachForPropertyChanged(TSetting newSetting, TSetting previousSetting)
		{
			if (!settingNotifiesPropertyChanged)
			{
				return;
			}

			var previousItem = previousSetting as INotifyPropertyChanged;
			if (previousItem != null)
			{
				previousItem.PropertyChanged -= HandleSettingPropertyChanged;
			}

			var item = newSetting as INotifyPropertyChanged;
			if (item != null)
			{
				item.PropertyChanged -= HandleSettingPropertyChanged;
				item.PropertyChanged += HandleSettingPropertyChanged;
			}
		}

		void HandleSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Dirty = true;
		}

//#else
//		public TSetting GetSetting()
//		{
//			if (!dirty && !settingInitialized && !mayBeDirty)
//			{
//				var userOption = UserOption;
//				var reader = userOption as IOptionStorage<TSetting>;
//				if (reader != null && reader.CanGetSetting)
//				{
//					Task<TSetting> task = reader.GetSetting();
//					task.Wait();
//					setting = task.Result;
//				}
//				else
//				{
//					setting = (TSetting)SettingsService.GetSetting(
//								userOption.SettingKey, UserOption.DefaultValue);
//				}
//
//				settingInitialized = true;
//
//				OnPropertyChanged("Setting");
//			}
//
//			return setting;
//		}
//#endif

		object IUserOptionReaderWriter.Setting
		{
			get => this.Setting;
			set => Setting = (TSetting)value;
		}
		
		public TSetting Setting
		{
			get
			{
				if (!dirty && !settingInitialized && !mayBeDirty)
				{
#pragma warning disable 4014
					GetSettingAsync();
#pragma warning restore 4014
				}

				return setting;
			}
			set
			{
				var previousSetting = setting;

				/* Assignment of this property causes it to be read, so we need to set a flag. */
				mayBeDirty = true;
				AssignmentResult assignmentResult = Set(ref setting, value);
				if (assignmentResult == AssignmentResult.Success)
				{
					Dirty = true;

					AttachForPropertyChanged(value, previousSetting);

					var tempOption = UserOption as UserOptionBase;
					tempOption?.HandleSettingChanged(this);
				}
				else
				{
					mayBeDirty = false;
				}
			}
		}
		
//#if !MONODROID && !__IOS__ && !NETFX_CORE
		public async Task<SaveOptionResult> Save()
		{
			if (dirty)
			{
				var userOption = UserOption;
				TSetting temp = await GetSettingAsync();

				var storage = userOption as IOptionStorage<TSetting>;
				if (storage != null && storage.CanSaveSetting)
				{
					return await storage.SaveSetting(temp);
				}
				else
				{
					SetSettingResult setResult = SettingsService.SetSetting(userOption.SettingKey, temp);
					if (setResult != SetSettingResult.Successful)
					{
						return new SaveOptionResult(SaveOptionResultValue.Failed, setResult);
					}
				}
				//var defaultImplementation = userOption as UserOptionBase<TSetting>;
				//if (defaultImplementation != null && defaultImplementation.DefaultValueFunc != null)
				//{
				//					
				//}
			}

			return new SaveOptionResult();
		}
//#else
//		public void Save()
//		{
//			if (dirty)
//			{
//				var userOption = UserOption;
//				TSetting temp = GetSetting();
//
//				var storage = userOption as IOptionStorage<TSetting>;
//				if (storage != null && storage.CanSaveSetting)
//				{
//					storage.SaveSetting(temp);
//				}
//				else
//				{
//					SettingsService.SetSetting(userOption.SettingKey, temp);
//				}
//				//var defaultImplementation = userOption as UserOptionBase<TSetting>;
//				//if (defaultImplementation != null && defaultImplementation.DefaultValueFunc != null)
//				//{
//				//					
//				//}
//			}
//		}
//#endif

		public async Task Refresh(bool reload = false)
		{
			if (reload)
			{
				await Reset();
				return;
			}

			if (!dirty)
			{
				settingInitialized = false;
				await GetSettingAsync();
				OnPropertyChanged(nameof(Setting));
			}
		}

		async Task Reset()
		{
			dirty = false;
			settingInitialized = false;
			mayBeDirty = false;

			await GetSettingAsync();
		}

		ISettingsService SettingsService
		{
			get
			{
				var settingsService = Dependency.Resolve<ISettingsService>();
				return settingsService;
			}
		}
	}
}
