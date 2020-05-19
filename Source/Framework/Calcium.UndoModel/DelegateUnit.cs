#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-02-07 17:09:48Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.UndoModel
{
	/// <summary>
	/// Represents a <see cref="IUnit"/> that is provided with an action 
	/// to be executed when the unit is performed.
	/// </summary>
	/// <typeparam name="T">The type of argument provided 
	/// when the unit is performed.</typeparam>
	public sealed class DelegateUnit<T> : UnitBase<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateUnit{T}"/> class.
		/// </summary>
		/// <param name="execute">The execution handler. 
		/// This will be invoked when the unit is performed.</param>
		/// <param name="descriptionForUser">The description for the user.</param>
		public DelegateUnit(
			Action<UnitEventArgs<T>> execute, 
			string descriptionForUser)
		{
			//AssertArg.IsNotNull(execute, nameof(execute));
			//AssertArg.IsNotNullOrEmpty(descriptionForUser, nameof(descriptionForUser));

			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}

			DescriptionForUser = descriptionForUser ?? throw new ArgumentNullException(nameof(descriptionForUser));

			Execute += (o, args) => execute(args);
		}
	}
}
