#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
    <CreationDate>2010-01-23 17:21:10Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.UndoModel
{
	/// <summary>
	/// The base class for classes implementing <see cref="IUndoableUnit"/>.
	/// <seealso cref="IUndoableUnit"/>
	/// </summary>
	public abstract class UndoableUnitBase<T> : UnitBase<T>, IUndoableUnit
	{
		protected UndoableUnitBase()
		{
			Undoable = true;
		}

		protected event EventHandler<UnitEventArgs<T>> Undo;

		void OnUndo(UnitEventArgs<T> e)
		{
			Undo?.Invoke(this, e);
		}

		UnitResult IUndoableUnit.Undo()
		{
			var args = new UnitEventArgs<T>(Argument);
			OnUndo(args);
			return args.UnitResult;
		}
	}
}
