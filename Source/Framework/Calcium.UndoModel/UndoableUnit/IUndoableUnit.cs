#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-01-23 17:21:10Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.UndoModel
{
	/// <summary>
	/// Defines the contract for an unit that can be undone. 
	/// <seealso cref="IUnit"/>
	/// </summary>
	interface IUndoableUnit : IUnit
	{
		/// <summary>
		/// Undoes the unit using the specified argument.
		/// </summary>
		/// <returns></returns>
		UnitResult Undo();
	}
}
