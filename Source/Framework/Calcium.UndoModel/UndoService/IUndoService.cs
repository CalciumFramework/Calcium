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

using System;
using System.Collections.Generic;

//using Calcium.InversionOfControl;
using Calcium.UndoModel;

namespace Calcium.Services
{
	/// <summary>
	/// This interface describes a service that 
	/// is able to execute <see cref="IUnit"/>s.
	/// </summary>
	//[DefaultType(typeof(UndoService))]
	[System.ComponentModel.DefaultValue(typeof(UndoService))]
	public interface IUndoService
	{
		/// <summary>
		/// Executes the specified unit.
		/// </summary>
		/// <param name="unit">The command to execute.</param>
		/// <param name="argument">The argument passed to the unit on execution.</param>
		/// <param name="contextKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <returns>The result of performing the unit.</returns>
		UnitResult PerformUnit<T>(UnitBase<T> unit, T argument, object contextKey = null);

		/// <summary>
		/// Executes the specified unit.
		/// </summary>
		/// <param name="unit">The command to execute.</param>
		/// <param name="argument">The argument passed to the unit on execution.</param>
		/// <param name="ownerKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <returns>The result of performing the unit.</returns>
		UnitResult PerformUnit<T>(UndoableUnitBase<T> unit, T argument, object ownerKey = null);

		/// <summary>
		/// Gets a value indicating whether this instance can undo an unit.
		/// </summary>
		/// <param name="ownerKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
		bool CanUndo(object ownerKey = null);

		/// <summary>
		/// Undoes the last unit.
		/// </summary>
		/// <param name="ownerKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <returns>The result of the unit. <see cref="UnitResult"/></returns>
		/// <exception cref="InvalidOperationException">
		/// Occurs if there are no previously executed units to undo.</exception>
		/// <returns>The result of undoing the unit.</returns>
		UnitResult Undo(object ownerKey = null);

		/// <summary>Undoes the last number of units. 
		/// If any single unit does not complete the process is halted.
		/// </summary>
		/// <param name="undoCount">The number of units to undo.</param>
		/// <param name="ownerKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <returns>The result of the unit. <see cref="UnitResult"/></returns>
		/// <exception cref="InvalidOperationException">
		/// Occurs if there are no previously executed units to undo.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Occurs if the list of undoable units 
		/// is smaller in length than the specified undoCount.</exception>
		/// <returns>The result of undoing the unit.</returns>
		UnitResult Undo(int undoCount, object ownerKey = null);

		/// <summary>
		/// Gets a value indicating whether an unit can be redone, 
		/// after it has been undone.
		/// </summary>
		/// <param name="ownerKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <value><c>true</c> if this instance can redo the last unit; 
		/// otherwise, <c>false</c>.</value>
		bool CanRedo(object ownerKey = null);

		/// <summary>
		/// Executes the unit that was previously undone.
		/// </summary>
		/// <param name="ownerKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <returns>The result of redoing the unit.</returns>
		UnitResult Redo(object ownerKey = null);

		/// <summary>
		/// Causes the last <see cref="IUnit"/> that was performed to be performed again.
		/// </summary>
		/// <param name="ownerKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <returns>The result of repeating the last unit.</returns>
		UnitResult Repeat(object ownerKey = null);

		/// <summary>
		/// Gets a value indicating whether this instance can execute the last unit executed.
		/// </summary>
		/// <param name="ownerKey">A key representing the owner of the unit. 
		/// This might be, for example, a text editor.
		/// This allows for a set of units to be associated with a control. 
		/// Can be <c>null</c>, in which case the unit is deemed to be global.</param>
		/// <value>
		/// 	<c>true</c> if this instance can execute the last unit executed; otherwise, <c>false</c>.
		/// </value>
		bool CanRepeat(object ownerKey = null);

		/// <summary>
		/// Gets the units which are deemed undoable.
		/// </summary>
		/// <param name="ownerKey">The owner associated with a set of units. 
		/// For example, a text editor. Can be <c>null</c>. 
		/// If <c>null</c> those units not associated with an ownerKey (global units) are returned.</param>
		/// <returns>The undoable units.</returns>
		IEnumerable<IUnit> GetUndoableUnits(object ownerKey = null);

		/// <summary>
		/// Gets the units which are deemed redoable.
		/// </summary>
		/// <param name="ownerKey">The owner associated with a set of units. 
		/// For example, a text editor. Can be <c>null</c>. 
		/// If <c>null</c> those units not associated with an ownerKey (global units) are returned.</param>
		/// <returns>The redoable units.</returns>
		IEnumerable<IUnit> GetRedoableUnits(object ownerKey = null);

		/// <summary>
		/// Gets the units which are deemed repeatable.
		/// </summary>
		/// <param name="ownerKey">The owner associated with a set of units. 
		/// For example, a text editor. Can be <c>null</c>. 
		/// If <c>null</c> those units not associated with an ownerKey (global units) are returned.</param>
		/// <returns>The undoable units.</returns>
		IEnumerable<IUnit> GetRepeatableUnits(object ownerKey = null);

		/// <summary>
		/// Clears the unit list for a particular owner.
		/// </summary>
		/// <param name="ownerKey">The owner associated with a set of units. 
		/// For example, a text editor. Can be <c>null</c>. 
		/// If <c>null</c> those units not associated with an ownerKey (global units) are cleared.</param>
		void Clear(object ownerKey = null);

		/// <summary>
		/// Limits the number of units that can be undone to the specified value.
		/// This can help to reduce memory usage in some scenarios.
		/// </summary>
		/// <param name="count">The maximum number of undo units to be retained.</param>
		/// <param name="ownerKey">The context key. Can be <c>null</c>.</param>
		void SetMaximumUndoCount(int count, object ownerKey = null);
	}
}
