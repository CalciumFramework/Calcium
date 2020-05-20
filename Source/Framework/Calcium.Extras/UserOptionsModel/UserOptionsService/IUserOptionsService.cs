#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-02-23 21:49:29Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;

using Calcium.InversionOfControl;
using Calcium.UserOptionsModel;

namespace Calcium.Services
{
	/// <summary>
	/// A user options service is sits on top of the 
	/// <see cref="ISettingsService"/> and allows the user
	/// to configure the application via a view that displays
	/// all user options. User options are also exportable
	/// and importable.
	/// </summary>
	[DefaultType(typeof(UserOptionsService), Singleton = true)]
	public interface IUserOptionsService
	{
		/// <summary>
		/// Gets the user options grouped into categories.
		/// </summary>
		IUserOptionGroupings UserOptionGroupings
		{
			get;
		}

		void Register(IUserOption userOption, IOptionCategory optionCategory);
		void Register(IEnumerable<IUserOption> userOptions, IOptionCategory optionCategory);

		void AddCategory(IOptionCategory optionCategory);

		void ClearAll(bool removeCategories = true);

		bool Deregister(IUserOption userOption);
	}
}
