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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Codon.ComponentModel;
using Codon.Reflection;

namespace Codon.UserOptionsModel
{
	/// <summary>
	/// This option is usually materialized as a drop down box
	/// or a selectable list.
	/// </summary>
	/// <typeparam name="TSetting">
	/// The type of setting. <seealso cref="Services.ISettingsService"/>
	/// </typeparam>
	public class ListOption<TSetting> : UserOptionBase<TSetting>, 
		IListOption<TSetting>, IProvider<IUserOptionReaderWriter>
	{
		public ListOption(
			Func<string> titleFunc,
			string settingKey,
			Func<TSetting> defaultValueFunc,
			IList<Func<TSetting>> options)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			AssertArg.IsNotNull(options, nameof(options));
			SetOptions(options);

			SetDefaultTemplateName();
		}

		public ListOption(
			Func<string> titleFunc,
			string settingKey,
			Func<TSetting> defaultValueFunc,
			IList<TSetting> options)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			AssertArg.IsNotNull(options, nameof(options));

			foreach (var pair in options)
			{
				this.options.Add(pair);
			}

			SetDefaultTemplateName();
		}

		public ListOption(
			Func<string> titleFunc,
			Func<Task<TSetting>> getSettingFunc,
			Func<TSetting, Task<SaveOptionResult>> saveSettingFunc,
			Func<TSetting> defaultValueFunc,
			IList<TSetting> options)
			: base(titleFunc, defaultValueFunc, saveSettingFunc, getSettingFunc)
		{
			AssertArg.IsNotNull(options, nameof(options));

			foreach (var pair in options)
			{
				this.options.Add(pair);
			}

			SetDefaultTemplateName();
		}

		void SetDefaultTemplateName()
		{
			var settingType = typeof(TSetting);
			if (settingType.IsEnum())
			{
				TemplateNameFunc = () => "Enum";
			}
			else
			{
				TemplateNameFunc = () => typeof(TSetting).Name;
			}
		}

		public void SetOptions(IEnumerable<Func<TSetting>> optionFuncs)
		{
			AssertArg.IsNotNull(optionFuncs, nameof(optionFuncs));

			options.Clear();

			foreach (var optionFunc in optionFuncs)
			{
				if (optionFunc == null)
				{
					throw new ArgumentException("Option func cannot be null.");
				}

				TSetting resolvedValue = optionFunc();
				options.Add(resolvedValue);
			}
		}

		public void SetOptions(IEnumerable<TSetting> optionValues)
		{
			AssertArg.IsNotNull(optionValues, nameof(optionValues));

			options.Clear();

			foreach (var optionValue in optionValues)
			{
				options.Add(optionValue);
			}
		}

		ObservableCollection<TSetting> options = new ObservableCollection<TSetting>();

		public ObservableCollection<TSetting> Options
		{
			get => options;
			private set => options = value;
		}

		public IUserOptionReaderWriter ProvidedItem => 
					new ListOptionReaderWriter<TSetting>(this);
	}
}