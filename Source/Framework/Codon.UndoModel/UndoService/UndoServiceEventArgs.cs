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
	/// Used to communicated with implementations 
	/// of the <see cref="UndoService"/> class.
	/// </summary>
	public class UndoServiceEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the unit the is being executed/undone etc.
		/// </summary>
		/// <value>The unit.</value>
		public IUnit Unit { get; private set; }

		public UndoServiceEventArgs()
		{
			/* Intentionally left blank. */
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="UndoServiceEventArgs"/> class.
		/// </summary>
		/// <param name="unit">The unit. Can be <c>null</c>.</param>
		public UndoServiceEventArgs(IUnit unit)
		{
			Unit = unit;
		}
	}

	/// <summary>
	/// Used to communicated with implementations 
	/// of the <see cref="UndoService"/> class,
	/// and to provide the means to cancel an operation.
	/// </summary>
	public class CancellableUndoServiceEventArgs : UndoServiceEventArgs
	{
		bool cancelled;

		/// <summary>
		/// Gets or sets a value indicating whether 
		/// this <see cref="CancellableUndoServiceEventArgs"/> 
		/// has been cancelled by a handler. 
		/// This means that the operation will not proceed, 
		/// e.g., the unit will not be executed.
		/// </summary>
		/// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
		public bool Cancel
		{
			get => cancelled;
			set
			{
				/* We don't allow a handler to 'uncancel'. 
				 * Once it's cancelled, that's it. */
				if (cancelled)
				{
					return;
				}
				cancelled = value;
			}
		}

		internal object OwnerKey { get; set; }

		public CancellableUndoServiceEventArgs()
		{
			/* Intentionally left blank. */
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="UndoServiceEventArgs"/> class.
		/// </summary>
		/// <param name="unit">The unit. Can be <c>null</c>.</param>
		public CancellableUndoServiceEventArgs(IUnit unit) :  base(unit)
		{
			/* Intentionally left blank. */
		}
	}
}