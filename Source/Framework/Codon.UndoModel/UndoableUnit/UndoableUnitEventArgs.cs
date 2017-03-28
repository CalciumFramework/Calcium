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

namespace Codon.UndoModel
{
	/// <summary>
	/// This class is used during the evaluation 
	/// of an <see cref="IUndoableUnit"/>'s CanUndo property.
	/// </summary>
	public class UndoableUnitEventArgs<TArgument> : UnitEventArgs<TArgument>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UndoableUnitEventArgs{TArgument}"/> class.
		/// </summary>
		/// <param name="argument">The argument used by the concrete
		/// <code>UnitBase</code> implementation, which is propagated during 
		/// the can and perform events.</param>
		public UndoableUnitEventArgs(TArgument argument)
			: base(argument)
		{
			/* Intentionally left blank. */
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UndoableUnitEventArgs{TArgument}"/> class.
		/// </summary>
		/// <param name="argument">The argument used by the concrete
		/// <code>UnitBase</code> implementation, which is propagated during 
		/// the can and perform events.</param>
		/// <param name="unitMode">Indicates whether this unit is being performed 
		/// for the fist time, if it is being redone, or if it is being repeated.</param>
		internal UndoableUnitEventArgs(TArgument argument, UnitMode unitMode)
			: base(argument, unitMode)
		{
			/* Intentionally left blank. */
		}

		bool enabled = true;

		/// <summary>
		/// Gets or sets a value indicating whether this instance can undo or redo.
		/// </summary>
		/// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>. 
		/// Default is <c>true</c></value>
		public bool Enabled
		{
			get => enabled;
			set => enabled = value;
		}
	}
}