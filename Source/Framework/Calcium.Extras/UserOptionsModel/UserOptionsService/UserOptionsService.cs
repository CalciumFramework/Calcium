#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-21 19:28:51Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Calcium.Collections;
using Calcium.Services;

namespace Calcium.UserOptionsModel
{
	public class UserOptionsService : IUserOptionsService
	{
		readonly Dictionary<IOptionCategory, List<IUserOption>> categoryDictionary
			= new Dictionary<IOptionCategory, List<IUserOption>>(
				new KeyEqualityComparer<IOptionCategory>(optionCategory => optionCategory.Id));

		public UserOptionsService()
		{
			/* Intentionally left blank. */
		}

		class UserOptionGroupingsType 
			: List<IGrouping<IOptionCategory, IUserOptionReaderWriter>>, IUserOptionGroupings
		{
			public void Refresh()
			{
				foreach (IGrouping<IOptionCategory, IUserOptionReaderWriter> grouping in this)
				{
					var category = grouping.Key;
					category.Refresh();

					var groupedList = grouping as GroupedList<IOptionCategory, IUserOptionReaderWriter>;
					groupedList?.RaiseOnTitlePropertyChanged();

					foreach (IUserOptionReaderWriter writer in grouping)
					{
						IUserOption userOption = writer.UserOption;
						if (userOption != null)
						{
							userOption.Refresh();
						}
						else
						{
							Debug.WriteLine("Warning: UserOptionsService.Refresh - user option is null.");
						}

						writer.Refresh();
					}
				}
			}
		}

		public IUserOptionGroupings UserOptionGroupings
		{
			get
			{
				var list = new UserOptionGroupingsType();
				/* Return all category grouping where there exists 
				 * at least one option for the category. */
				foreach (KeyValuePair<IOptionCategory, List<IUserOption>> pair in categoryDictionary)
				{
					IEnumerable<IUserOptionReaderWriter> readerWriters 
								= from userOption in pair.Value
									select GetReaderWriter(userOption);

					var grouping = new GroupedList<IOptionCategory, IUserOptionReaderWriter>(

					pair.Key, new List<IUserOptionReaderWriter>(readerWriters));
					list.Add(grouping);
				}
				return list;
			}
		}

		protected virtual IUserOptionReaderWriter GetReaderWriter(IUserOption userOption)
		{
			var result = userOption.ReaderWriter;
			if (result == null)
			{
				var strategy = Dependency.Resolve<IReaderWriterCreationStrategy, ReaderWriterCreationStrategy>(false);
				result = strategy.Create(userOption);
			}

			return result;
		}

		internal IEnumerable<IOptionCategory> Categories
		{
			get
			{
				var result = categoryDictionary.Keys.ToList();
				return result;
			}
		}

		public void Register(IUserOption userOption, IOptionCategory optionCategory)
		{
			AssertArg.IsNotNull(userOption, nameof(userOption));

			Tuple<IOptionCategory, List<IUserOption>> existingCategoryOptions = null;

			foreach (KeyValuePair<IOptionCategory, List<IUserOption>> pair in categoryDictionary)
			{
				IUserOption existingOption = null;

				foreach (IUserOption option in pair.Value)
				{
					if (!string.IsNullOrEmpty(option.SettingKey) 
							&& object.Equals(option.SettingKey, userOption.SettingKey))
					{
						existingOption = option;
						break;
					}
				}
				//IUserOption existingOption = (from option in pair.Value
				//				                where object.Equals(option.SettingKey, userOption.SettingKey)
				//				                select option).FirstOrDefault();
				
				if (existingOption != null)
				{
					throw new DuplicateItemException(existingOption, userOption, userOption.SettingKey);
				}

				if (pair.Key.Id == optionCategory.Id)
				{
					existingCategoryOptions = new Tuple<IOptionCategory, List<IUserOption>>(pair.Key, pair.Value);
				}
			}

			List<IUserOption> list;

			if (existingCategoryOptions != null)
			{
				list = existingCategoryOptions.Item2;
			}
			else
			{
				list = new List<IUserOption>();
				categoryDictionary.Add(optionCategory, list);
			}

			list.Add(userOption);
		}

		public void Register(IEnumerable<IUserOption> userOptions, IOptionCategory optionCategory)
		{
			AssertArg.IsNotNull(userOptions, nameof(userOptions));
			AssertArg.IsNotNull(optionCategory, nameof(optionCategory));

			/* We regect all options if any are already registered. */
			Tuple<IOptionCategory, List<IUserOption>> existingCategoryOptions = null;

			foreach (KeyValuePair<IOptionCategory, List<IUserOption>> pair in categoryDictionary)
			{
				foreach (IUserOption userOption in userOptions)
				{
					IUserOption existingOption = null;

					foreach (IUserOption option in pair.Value)
					{
						if (!string.IsNullOrEmpty(option.SettingKey) 
								&& object.Equals(option.SettingKey, userOption.SettingKey))
						{
							existingOption = option;
							break;
						} 
					}
					//IUserOption existingOption = (from option in pair.Value
					//					            where object.Equals(option.SettingKey, userOption.SettingKey)
					//					            select option).FirstOrDefault();

					if (existingOption != null)
					{
						throw new DuplicateItemException(existingOption, userOption);
					}
				}

				if (pair.Key.Id == optionCategory.Id)
				{
					existingCategoryOptions = new Tuple<IOptionCategory, List<IUserOption>>(pair.Key, pair.Value);
				}
			}

			List<IUserOption> list;

			if (existingCategoryOptions != null)
			{
				list = existingCategoryOptions.Item2;
			}
			else
			{
				list = new List<IUserOption>();
				categoryDictionary.Add(optionCategory, list);
			}

			foreach (IUserOption userOption in userOptions)
			{
				list.Add(userOption);
			}
		}

		public void AddCategory(IOptionCategory optionCategory)
		{
			var existingCategory = (from pair in categoryDictionary
			                        where pair.Key.Id == optionCategory.Id
			                        select new {Category = pair.Key, Options = pair.Value}).FirstOrDefault();

			foreach (KeyValuePair<IOptionCategory, List<IUserOption>> pair in categoryDictionary)
			{
				if (pair.Key.Id == optionCategory.Id)
				{
					existingCategory = new {Category = pair.Key, Options = pair.Value};
					throw new DuplicateItemException(existingCategory, optionCategory);
				}
			}

			categoryDictionary.Add(optionCategory, new List<IUserOption>());
		}

		public void ClearAll(bool removeCategories = true)
		{
			if (removeCategories)
			{
				categoryDictionary.Clear();
			}
			else
			{
				foreach (var list in categoryDictionary.Values)
				{
					list.Clear();
				}
			}
		}

		public bool Deregister(IUserOption userOption)
		{
			bool result = false;

			foreach (var list in categoryDictionary.Values)
			{
				result |= list.Remove(userOption);
			}

			return result;
		}
	}
}
