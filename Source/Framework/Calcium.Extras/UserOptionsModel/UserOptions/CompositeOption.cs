#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Calcium.ComponentModel;

namespace Calcium.UserOptionsModel
{
	/// <summary>
	/// This class allows you to group a set of options within a single XAML data template.
	/// </summary>
	public class CompositeOption : IUserOption, IUserOptionReaderWriter, IProvider<IUserOptionReaderWriter>
	{

		public CompositeOption(Func<string> titleFunc)
		{
			TitleFunc = AssertArg.IsNotNull(titleFunc, nameof(titleFunc));
		}

		readonly List<IUserOption> userOptions = new List<IUserOption>();

		public IEnumerable<IUserOption> UserOptions => userOptions;

		public void AddOption(IUserOption userOption)
		{
			GetReaderWriter(userOption);
			userOptions.Add(userOption);
		}

		public void RemoveOption(IUserOption userOption)
		{
			userOptions.Remove(userOption);
		}

		public void ClearOptions()
		{
			userOptions.Clear();
		}

		IUserOptionReaderWriter GetReaderWriter(IUserOption userOption)
		{
			var result = userOption.ReaderWriter;
			if (result == null)
			{
				var strategy = Dependency.Resolve<IReaderWriterCreationStrategy, ReaderWriterCreationStrategy>(false);
				result = strategy.Create(userOption);
			}

			return result;
		}

		public Func<string> TitleFunc { get; private set; }

		public string Title => TitleFunc();

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

		/// <summary>
		/// Gets or sets the description func, which is used to retrieve 
		/// a description of the option that may be displayed to the user.
		/// </summary>
		/// <value>
		/// The description func.
		/// </value>
		public Func<string> DescriptionFunc { get; set; }

		public string Description => DescriptionFunc?.Invoke();

		public string SettingKey { get; private set; }

		/// <summary>
		/// Gets or sets the template name func, 
		/// which is used to display the option in the option view.
		/// </summary>
		/// <value>
		/// The template name func.
		/// </value>
		public Func<string> TemplateNameFunc { get; set; }

		public string TemplateName => TemplateNameFunc?.Invoke();

		public object DefaultValue { get; private set; }

		public IUserOptionReaderWriter ReaderWriter { get; set; }

		#region IUserOptionReaderWriter implementation

		public IUserOption UserOption
		{
			get => this;
			set
			{
			}
		}

		public bool Dirty
		{
			get
			{
				foreach (var userOption in userOptions)
				{
					var readerWriter = userOption.ReaderWriter;
					if (readerWriter != null && readerWriter.Dirty)
					{
						return true;
					}
				}

				return false;
			}
		}

		public async Task<SaveOptionResult> Save()
		{
			SaveOptionResult result = null;

			foreach (var userOption in userOptions)
			{
				var readerWriter = userOption.ReaderWriter;
				if (readerWriter != null)
				{
					result = await readerWriter.Save();
					if (result.ResultValue != SaveOptionResultValue.Success)
					{
						return result;
					}
				}
			}

			return result ?? new SaveOptionResult(SaveOptionResultValue.Success);
		}

		public async Task Refresh(bool reload = false)
		{
			foreach (var userOption in userOptions)
			{
				await userOption.Refresh(reload);
			}

			OnPropertyChanged(string.Empty);
		}

		public Type SettingType => null;

		public virtual string FormattedSetting => null;

		public Task<object> GetSettingAsync()
		{
			throw new NotSupportedException();
		}

		public virtual object Setting { get; set; }

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(
			[CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(
				this, new PropertyChangedEventArgs(propertyName));
		}

		public IUserOptionReaderWriter ProvidedItem => this;

		bool enabled = true;

		public bool Enabled
		{
			get => enabled;
			set
			{
				if (value != enabled)
				{
					enabled = value;
					foreach (var option in userOptions)
					{
						option.Enabled = enabled;
					}
					OnPropertyChanged();
				}
			}
		}
	}
}
