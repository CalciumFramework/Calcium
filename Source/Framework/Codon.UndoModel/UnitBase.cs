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
	/// The base class for <see cref="IUnit"/>s.
	/// An unit performs an application unit.
	/// An unit may be a command behaviour, that is, it may encapsulate 
	/// the logic performed when a command is initiated.
	/// </summary>
	public abstract class UnitBase<T> : IInternalUnit
	{
		public bool Undoable { get; protected internal set; }

		#region event PerformUnit

		event EventHandler<UnitEventArgs<T>> execute;

		/// <summary>
		/// Occurs when the unit is being performed. 
		/// This is the event to handler for your unit logic.
		/// </summary>
		protected event EventHandler<UnitEventArgs<T>> Execute
		{
			add
			{
				execute += value;
			}
			remove
			{
				execute -= value;
			}
		}

		void OnExecute(UnitEventArgs<T> e)
		{
			execute?.Invoke(this, e);
		}

		#endregion

		internal T Argument { get; private set; }

		object IInternalUnit.Argument => Argument;

		UnitResult IInternalUnit.PerformUnit(object argument, UnitMode unitMode)
		{
			Argument = (T)argument;

			var eventArgs = new UnitEventArgs<T>(Argument, unitMode);
			OnExecute(eventArgs);
			return eventArgs.UnitResult;
		}

		internal UnitResult PerformUnit(object argument, UnitMode unitMode)
		{
			var internalUnit = (IInternalUnit)this;
			return internalUnit.PerformUnit(argument, unitMode);
		}

		internal UnitResult Repeat()
		{
			var eventArgs = new UnitEventArgs<T>(Argument, UnitMode.Repeat);
			OnExecute(eventArgs);
			return eventArgs.UnitResult;
		}

		public abstract string DescriptionForUser { get; }

		bool repeatable;

		public bool Repeatable
		{
			get => repeatable;
			protected set
			{
				if (repeatable != value)
				{
					repeatable = value;
					UndoService?.NotifyUnitRepeatableChanged(this);
				}
			}
		}

		internal IInternalUndoService UndoService { get; set; }
	}
}
