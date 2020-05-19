#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-02-07 17:15:35Z</CreationDate>
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
	public sealed class UndoableUnit<T> : UndoableUnitBase<T>
	{
		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="UndoableUnit{T}"/> class.
		/// </summary>
		/// <param name="execute">The execution handler. 
		/// This will be performed when the unit is performed.</param>
		/// <param name="undo">The undo handler. 
		/// This will be performed when the unit is undone.</param>
		/// <param name="descriptionForUser">
		/// The description for the user.</param>
		public UndoableUnit(Action<UnitEventArgs<T>> execute, 
							Action<UnitEventArgs<T>> undo, 
							string descriptionForUser)
		{
//			AssertArg.IsNotNullOrEmpty(descriptionForUser, nameof(descriptionForUser));
//			AssertArg.IsNotNull(execute, nameof(execute));
//			AssertArg.IsNotNull(undo, nameof(undo));
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}

			if (undo == null)
			{
				throw new ArgumentNullException(nameof(undo));
			}

			DescriptionForUser = descriptionForUser ?? throw new ArgumentNullException(nameof(descriptionForUser));
			Execute += (o, args) => execute(args);
			Undo += (o, args) => undo(args);
		}
	}
}
