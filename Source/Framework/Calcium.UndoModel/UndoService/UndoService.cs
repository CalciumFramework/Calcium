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
using System.Collections.Generic;
using System.Linq;

using Codon.Services;

namespace Codon.UndoModel
{
	/// <summary>
	/// Default implementation of the <see cref="IUndoService"/> interface.
	/// This class provides for execution, undoing, and redoing
	/// of <see cref="IUnit"/> instances.
	/// See the interface for API documentation.
	/// </summary>
	public class UndoService : IUndoService, IInternalUndoService
	{
		readonly Dictionary<object, UnitCollection<IInternalUnit>> repeatableDictionary = new Dictionary<object, UnitCollection<IInternalUnit>>();
		readonly Dictionary<object, UnitCollection<IUndoableUnit>> redoableDictionary = new Dictionary<object, UnitCollection<IUndoableUnit>>();
		readonly Dictionary<object, UnitCollection<IUndoableUnit>> undoableDictionary = new Dictionary<object, UnitCollection<IUndoableUnit>>();

		readonly UnitCollection<IInternalUnit> globallyRepeatableUnits = new UnitCollection<IInternalUnit>();
		readonly UnitCollection<IUndoableUnit> globallyRedoableUnits = new UnitCollection<IUndoableUnit>();
		readonly UnitCollection<IUndoableUnit> globallyUndoableUnits = new UnitCollection<IUndoableUnit>();

		#region PerformUnit

		/// <summary>Executes the specified unit.</summary>
		/// <param name="unit">The command to execute.</param>
		/// <param name="argument">The argument passed to the unit on execution.</param>
		/// <param name="ownerKey">An object identifying the owner of the unit.</param>
		public UnitResult PerformUnit<T>(UnitBase<T> unit, T argument, object ownerKey)
		{
			//AssertArg.IsNotNull(unit, nameof(unit));
			if (unit == null)
			{
				throw new ArgumentNullException(nameof(unit));
			}

			if (ownerKey == null)
			{
				return PerformUnit(unit, argument);
			}

			var eventArgs = new CancellableUndoServiceEventArgs(unit);
			OnExecuting(eventArgs);

			if (eventArgs.Cancel)
			{
				return UnitResult.Cancelled;
			}        
			
			/* Clear the undoable units for this context. */
			undoableDictionary.Remove(ownerKey);
			redoableDictionary.Remove(ownerKey);

			if (!repeatableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IInternalUnit> repeatableUnits))
			{
				repeatableUnits = new UnitCollection<IInternalUnit>();
				repeatableDictionary[ownerKey] = repeatableUnits;
			}
			repeatableUnits.AddLast(unit);

			var result = unit.PerformUnit(argument, UnitMode.FirstTime);

			TrimIfRequired(ownerKey);

			OnExecuted(new UndoServiceEventArgs(unit));
			return result;
		}

		UnitResult PerformUnit<T>(UnitBase<T> unit, T argument)
		{
			var eventArgs = new CancellableUndoServiceEventArgs(unit);
			OnExecuting(eventArgs);

			if (eventArgs.Cancel)
			{
				return UnitResult.Cancelled;
			}

			globallyRedoableUnits.Clear();
			globallyUndoableUnits.Clear();

			globallyRepeatableUnits.AddLast(unit);
			
			UnitResult result = unit.PerformUnit(argument, UnitMode.FirstTime);

			TrimIfRequired();

			OnExecuted(new UndoServiceEventArgs(unit));
			return result;
		}

		/// <summary>Executes the specified unit.</summary>
		/// <param name="unit">The command to execute.</param>
		/// <param name="argument">The argument passed to the unit on execution.</param>
		/// <param name="ownerKey">An object identifying the owner of the unit.</param>
		public UnitResult PerformUnit<T>(
			UndoableUnitBase<T> unit, T argument, object ownerKey)
		{
			//AssertArg.IsNotNull(unit, nameof(unit));
			if (unit == null)
			{
				throw new ArgumentNullException(nameof(unit));
			}

			if (ownerKey == null)
			{
				return PerformUnit(unit, argument);
			}

			var eventArgs = new CancellableUndoServiceEventArgs(unit) {OwnerKey = ownerKey};
			OnExecuting(eventArgs);
			if (eventArgs.Cancel)
			{
				return UnitResult.Cancelled;
			}

			redoableDictionary.Remove(ownerKey);

			if (!repeatableDictionary.TryGetValue(ownerKey, 
						out UnitCollection<IInternalUnit> repeatableUnits))
			{
				repeatableUnits = new UnitCollection<IInternalUnit>();
				repeatableDictionary[ownerKey] = repeatableUnits;
			}
			repeatableUnits.AddLast(unit);

			if (!undoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> undoableUnits))
			{
				undoableUnits = new UnitCollection<IUndoableUnit>();
				undoableDictionary[ownerKey] = undoableUnits;
			}
			undoableUnits.AddLast(unit);

			UnitResult result = unit.PerformUnit(argument, UnitMode.FirstTime);

			TrimIfRequired(ownerKey);

			OnExecuted(new UndoServiceEventArgs(unit));
			return result;
		}

		UnitResult PerformUnit<T>(UndoableUnitBase<T> unit, T argument)
		{
			var eventArgs = new CancellableUndoServiceEventArgs(unit);
			OnExecuting(eventArgs);
			if (eventArgs.Cancel)
			{
				return UnitResult.Cancelled;
			}

			globallyRedoableUnits.Clear();
			globallyRepeatableUnits.AddLast(unit);
			globallyUndoableUnits.AddLast(unit);

			UnitResult result = unit.PerformUnit(argument, UnitMode.FirstTime);

			TrimIfRequired();

			OnExecuted(new UndoServiceEventArgs(unit));
			return result;
		}
             
		#endregion

		/// <inheritdoc />
		public bool CanUndo(object ownerKey = null)
		{
			if (ownerKey == null)
			{
				return globallyUndoableUnits.Count > 0;
			}

			if (undoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> undoableUnits))
			{
				return undoableUnits.Count > 0;
			}
			return false;
		}

		/// <summary>
		/// Undoes the execution of a previous <see cref="IUnit"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Occurs if there are no previously executed units to undo.</exception>
		public UnitResult Undo(object ownerKey)
		{
			if (ownerKey == null)
			{
				return Undo();
			}

			if (!undoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> undoableUnits))
			{
				throw new InvalidOperationException("No undoable units for the specified owner key.");
			}

			IUndoableUnit undoableUnit = undoableUnits.Pop();

			if (!repeatableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IInternalUnit> repeatableUnits))
			{
				throw new InvalidOperationException("No repeatable units for the specified owner key.");
			}

			repeatableUnits.RemoveLast();

			var eventArgs = new CancellableUndoServiceEventArgs(undoableUnit) {OwnerKey = ownerKey};
			OnUndoing(eventArgs);
			if (eventArgs.Cancel)
			{
				undoableUnits.AddLast(undoableUnit);				
				return UnitResult.Cancelled;
			}

			if (!redoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> redoableUnits))
			{
				redoableUnits = new UnitCollection<IUndoableUnit>();
				redoableDictionary[ownerKey] = redoableUnits;
			}
			redoableUnits.AddLast(undoableUnit);

			var result = undoableUnit.Undo();

			TrimIfRequired(ownerKey);
                  
			OnUndone(new UndoServiceEventArgs(undoableUnit));
			return result;
		}

		UnitResult Undo()
		{
			if (globallyRepeatableUnits.Count < 1)
			{
				throw new InvalidOperationException("No unit to undo.");
			}

			IUndoableUnit undoableUnit = globallyUndoableUnits.Pop();
			IInternalUnit repeatableUnit = globallyRepeatableUnits.Pop();

			var eventArgs = new CancellableUndoServiceEventArgs(undoableUnit);
			OnUndoing(eventArgs);
			if (eventArgs.Cancel)
			{
				globallyUndoableUnits.AddLast(undoableUnit);
				globallyRepeatableUnits.AddLast(repeatableUnit);
				return UnitResult.Cancelled;
			}

			globallyRedoableUnits.AddLast(undoableUnit);

			UnitResult result = undoableUnit.Undo();
			
			OnUndone(new UndoServiceEventArgs(undoableUnit));
			return result;
		}

		/// <inheritdoc />
		public UnitResult Undo(int undoCount, object ownerKey)
		{
			if (undoCount <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(undoCount), 
					"undoCount must be greater than 0.");
			}
			//AssertArg.IsGreaterThan(0, undoCount, nameof(undoCount));

			if (ownerKey == null)
			{
				return Undo(undoCount);
			}

			for (int i = 0; i < undoCount; i++)
			{
				var iterationResult = Undo(ownerKey);
				if (iterationResult != UnitResult.Completed)
				{
					return iterationResult;
				}
			}
			return UnitResult.Completed;
		}

		UnitResult Undo(int undoCount)
		{
			for (int i = 0; i < undoCount; i++)
			{
				var iterationResult = Undo();
				if (iterationResult != UnitResult.Completed)
				{
					return iterationResult;
				}
			}
			return UnitResult.Completed;
		}

		/// <inheritdoc />
		public bool CanRedo(object ownerKey)
		{
			if (ownerKey == null)
			{
				return CanRedo();
			}

			if (redoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> redoableUnits))
			{
				return redoableUnits.Count > 0;
			}
			return false;
		}

		bool CanRedo()
		{
			return globallyRedoableUnits.Count > 0;
		}

		/// <summary>
		/// Performs the execution of a <see cref="IUnit"/>
		/// instance that has been undone, then places it back
		/// into the command stack.
		/// </summary>
		public UnitResult Redo(object ownerKey)
		{
			if (ownerKey == null)
			{
				return Redo();
			}

			if (!redoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> redoableUnits))
			{
				throw new InvalidOperationException("No units to be redone for the specified owner key.");
			}
			IUndoableUnit unit = redoableUnits.Pop();

			var eventArgs = new CancellableUndoServiceEventArgs(unit);
			OnRedoing(eventArgs);

			if (eventArgs.Cancel)
			{
				redoableUnits.AddLast(unit);
				return UnitResult.Cancelled;
			}

			var internalUnit = (IInternalUnit)unit;

			if (!repeatableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IInternalUnit> repeatableUnits))
			{
				repeatableUnits = new UnitCollection<IInternalUnit>();
			}
			repeatableUnits.AddLast(internalUnit);

			if (!undoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> undoableUnits))
			{
				undoableUnits = new UnitCollection<IUndoableUnit>();
			}
			undoableUnits.AddLast(unit);

			UnitResult result = internalUnit.PerformUnit(internalUnit.Argument, UnitMode.Redo);

			TrimIfRequired(ownerKey);

			OnRedone(new UndoServiceEventArgs(unit));
			return result;
		}

		UnitResult Redo()
		{
			if (globallyRedoableUnits.Count < 1)
			{
				throw new InvalidOperationException("No unit to redo."); /* TODO: Make localizable resource. */
			}

			var unit = globallyRedoableUnits.Pop();
			var eventArgs = new CancellableUndoServiceEventArgs(unit);
			OnRedoing(eventArgs);

			if (eventArgs.Cancel)
			{
				globallyRedoableUnits.AddLast(unit);
				return UnitResult.Cancelled;
			}

			var internalUnit = (IInternalUnit)unit;
			
			globallyRepeatableUnits.AddLast(internalUnit);
			globallyUndoableUnits.AddLast(unit);

			var result = internalUnit.PerformUnit(internalUnit.Argument, UnitMode.Redo);

			TrimIfRequired();

			OnRedone(new UndoServiceEventArgs(unit));
			return result;
		}

		/// <inheritdoc />
		public UnitResult Repeat(object ownerKey)
		{
			if (ownerKey == null)
			{
				return Repeat();
			}

			if (!repeatableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IInternalUnit> repeatableUnits))
			{
				throw new InvalidOperationException(
					"No units to be redone for the specified owner key.");
			}
			var unit = repeatableUnits.Peek();
			if (!unit.Repeatable)
			{
				return UnitResult.NoUnit;
			}

			var eventArgs = new CancellableUndoServiceEventArgs(unit) {OwnerKey = ownerKey};
			OnExecuting(eventArgs);
			if (eventArgs.Cancel)
			{
				return UnitResult.Cancelled;
			}

			repeatableUnits.AddLast(unit);

			if (!undoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> undoableUnits))
			{
				undoableUnits = new UnitCollection<IUndoableUnit>();
				undoableDictionary[ownerKey] = undoableUnits;
			}

			var undoableUnit = unit as IUndoableUnit;
			if (undoableUnit != null)
			{
				undoableUnits.AddLast(undoableUnit);
			}
			else
			{
				/* It's not undoable so we clear the list of undoable units. 
				 * This is because this unit may cause the previous 
				 * undo activities to be rendered invalid. */
				undoableDictionary[ownerKey] = null;
				redoableDictionary[ownerKey] = null;
			}

			UnitResult result = unit.PerformUnit(unit.Argument, UnitMode.Repeat);

			TrimIfRequired(ownerKey);

			OnExecuted(new UndoServiceEventArgs(unit));
			return result;
		}

		Dictionary<object, int> unitCountMaximums = new Dictionary<object, int>(); 

		void TrimIfRequired(object ownerKey = null)
		{
			UnitCollection<IUndoableUnit> undoableUnits;
			UnitCollection<IInternalUnit> repeatableUnits;
			UnitCollection<IUndoableUnit> redoableUnits;
			long maximumUnitCount = unitCountMax;

			if (ownerKey != null)
			{
				if (unitCountMaximums.TryGetValue(ownerKey, out int tempMaximum))
				{
					maximumUnitCount = tempMaximum;
				}

				if (maximumUnitCount == long.MaxValue)
				{
					return;
				}

				undoableDictionary.TryGetValue(ownerKey, out undoableUnits);
				repeatableDictionary.TryGetValue(ownerKey, out repeatableUnits);
				redoableDictionary.TryGetValue(ownerKey, out redoableUnits);
			}
			else
			{
				if (unitCountMax == long.MaxValue)
				{
					return; /* Nothing to do. */
				}

				undoableUnits = globallyUndoableUnits;
				repeatableUnits = globallyRepeatableUnits;
				redoableUnits = globallyRedoableUnits;
			}

			int undoableUnitCount = undoableUnits?.Count ?? 0;
			int repeatableUnitCount = repeatableUnits?.Count ?? 0;
			int redoableUnitCount = redoableUnits?.Count ?? 0;

			long undoableUnitExcess = undoableUnitCount - maximumUnitCount;
			long repeatableUnitExcess = repeatableUnitCount - maximumUnitCount;
			long redoableUnitExcess = redoableUnitCount - maximumUnitCount;

			for (long i = 0; i < undoableUnitExcess; i++)
			{
				undoableUnits.RemoveFirst();
			}

			for (long i = 0; i < repeatableUnitExcess; i++)
			{
				repeatableUnits.RemoveFirst();
			}

			for (long i = 0; i < redoableUnitExcess; i++)
			{
				redoableUnits.RemoveFirst();
			}
		}

		internal enum UnitType
		{
			Undoable,
			Redoable,
			Repeatable
		}

		UnitResult Repeat()
		{
			var unit = globallyRepeatableUnits.Peek();
			if (!unit.Repeatable)
			{
				return UnitResult.NoUnit;
			}
			var eventArgs = new CancellableUndoServiceEventArgs(unit);
			OnExecuting(eventArgs);
			if (eventArgs.Cancel)
			{
				return UnitResult.Cancelled;
			}

			globallyRedoableUnits.Clear();
			globallyRepeatableUnits.AddLast(unit);

			var undoableUnit = unit as IUndoableUnit;
			if (undoableUnit != null)
			{
				globallyUndoableUnits.AddLast(undoableUnit);
			}
			else
			{
				globallyUndoableUnits.Clear();
				globallyRedoableUnits.Clear();
			}

			var result = unit.PerformUnit(unit.Argument, UnitMode.Repeat);

			OnExecuted(new UndoServiceEventArgs(unit));
			return result;
		}

		/// <inheritdoc />
		public bool CanRepeat(object ownerKey = null)
		{
			UnitCollection<IInternalUnit> units;

			if (ownerKey == null)
			{
				units = globallyRepeatableUnits;
			}
			else
			{
				if (!repeatableDictionary.TryGetValue(ownerKey, out units))
				{
					return false;
				}
			}
			
			return units.Count > 0 && units.Peek().Repeatable;
		}

		/// <inheritdoc />
		public IEnumerable<IUnit> GetUndoableUnits(object ownerKey = null)
		{
			if (ownerKey == null)
			{
				return new List<IUnit>(globallyUndoableUnits.Cast<IUnit>());
			}

			if (!undoableDictionary.TryGetValue(ownerKey, 
				// ReSharper disable once CollectionNeverUpdated.Local
					out UnitCollection<IUndoableUnit> units))
			{
				return new List<IUnit>();
			}
			return new List<IUnit>(units.Cast<IUnit>());
		}

		/// <inheritdoc />
		public IEnumerable<IUnit> GetRedoableUnits(object ownerKey = null)
		{
			if (ownerKey == null)
			{
				return new List<IUnit>(globallyRedoableUnits.Cast<IUnit>());
			}

			if (!redoableDictionary.TryGetValue(ownerKey, 
				// ReSharper disable once CollectionNeverUpdated.Local
				out UnitCollection<IUndoableUnit> units))
			{
				return new List<IUnit>();
			}
			return new List<IUnit>(units.Cast<IUnit>());
		}

		/// <inheritdoc />
		public IEnumerable<IUnit> GetRepeatableUnits(object ownerKey = null)
		{
			List<IUnit> result;
			if (ownerKey == null)
			{
				result = globallyRepeatableUnits.Where(unit => unit.Repeatable).Cast<IUnit>().ToList();
				return result;
			}

			if (!repeatableDictionary.TryGetValue(ownerKey, 
				// ReSharper disable once CollectionNeverUpdated.Local
					out UnitCollection<IInternalUnit> units))
			{
				return new List<IUnit>();
			}
			result = units.Where(unit => unit.Repeatable).Cast<IUnit>().ToList();
			return result;
		}

		long unitCountMax = long.MaxValue;

		/// <inheritdoc />
		public void SetMaximumUndoCount(int count, object ownerKey = null)
		{
			//AssertArg.IsGreaterThan(0, count, nameof(count));
			if (count < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(count),
					"undoCount must be greater than 0.");
			}

			if (ownerKey == null)
			{
				unitCountMax = count;
			}
			else
			{
				unitCountMaximums[ownerKey] = count;
			}
		}

		/// <summary>
		/// Clears the undo and redo stacks.
		/// </summary>
		public void Clear(object ownerKey = null)
		{
			if (ownerKey == null)
			{
				globallyRepeatableUnits.Clear();
				globallyUndoableUnits.Clear();
				globallyRedoableUnits.Clear();
				OnCleared(EventArgs.Empty);
				return;
			}

			if (repeatableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IInternalUnit> repeatableUnits))
			{
				repeatableUnits.Clear();
			}

			if (undoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> undoableUnits))
			{
				undoableUnits.Clear();
			}

			if (redoableDictionary.TryGetValue(ownerKey, 
					out UnitCollection<IUndoableUnit> redoableUnits))
			{
				redoableUnits.Clear();
			}

			OnCleared(EventArgs.Empty);
		}

		#region event Executing

		/// <summary>
		/// This event occurs when a <see cref="IUnit"/> is about to be performed.
		/// </summary>
		public event EventHandler<CancellableUndoServiceEventArgs> Executing;

		void OnExecuting(CancellableUndoServiceEventArgs e)
		{
			Executing?.Invoke(this, e);
		}

		#endregion

		#region event Executed

		/// <summary>
		/// This event occurs when a <see cref="IUnit"/> has been performed.
		/// </summary>
		public event EventHandler<UndoServiceEventArgs> Executed;

		void OnExecuted(UndoServiceEventArgs e)
		{
			Executed?.Invoke(this, e);
		}

		#endregion

		#region event Undoing

		/// <summary>
		/// This event occurs when a <see cref="IUnit"/> is about to be undone.
		/// </summary>
		public event EventHandler<CancellableUndoServiceEventArgs> Undoing;

		void OnUndoing(CancellableUndoServiceEventArgs e)
		{
			Undoing?.Invoke(this, e);
		}

		#endregion

		#region event Undone

		/// <summary>
		/// This event occurs when a <see cref="IUnit"/> has been undone.
		/// </summary>
		public event EventHandler<UndoServiceEventArgs> Undone;

		void OnUndone(UndoServiceEventArgs e)
		{
			Undone?.Invoke(this, e);
		}

		#endregion

		#region event Redoing

		/// <summary>
		/// This event occurs when a <see cref="IUnit"/> is about to be performed again.
		/// </summary>
		public event EventHandler<CancellableUndoServiceEventArgs> Redoing;

		void OnRedoing(CancellableUndoServiceEventArgs e)
		{
			Redoing?.Invoke(this, e);
		}

		#endregion

		#region event Redone

		/// <summary>
		/// This event occurs when a <see cref="IUnit"/> has been performed again.
		/// </summary>
		public event EventHandler<UndoServiceEventArgs> Redone;

		void OnRedone(UndoServiceEventArgs e)
		{
			Redone?.Invoke(this, e);
		}

		#endregion

		#region event Cleared

		/// <summary>
		/// This event occurs when the service's list
		/// of undoable and re-doable units is being cleared.
		/// </summary>
		public event EventHandler<EventArgs> Cleared;

		void OnCleared(EventArgs e)
		{
			Cleared?.Invoke(this, e);
		}

		#endregion

		void IInternalUndoService.NotifyUnitRepeatableChanged(IInternalUnit unit)
		{
			/* Here we should notify listeners that a unit has changed its repeatable property. 
			 * This will prompt re-population of any Repeatable unit lists etc. */
		}

		class UnitCollection<T> : LinkedList<T>
		{
			public T Pop()
			{
				T result = Last.Value;
				RemoveLast();
				return result;
			}

			public T Peek()
			{
				var last = Last;
				return last != null ? last.Value : default;
			}
		}

		internal int GetUnitCount(UnitType unitType, object ownerKey = null)
		{
			if (unitType == UnitType.Undoable)
			{
				if (ownerKey == null)
				{
					return globallyUndoableUnits.Count;
				}

				if (!undoableDictionary.TryGetValue(ownerKey, 
						out UnitCollection<IUndoableUnit> units))
				{
					return 0;
				}
				return units.Count;
			}
			else if (unitType == UnitType.Repeatable)
			{
				if (ownerKey == null)
				{
					return globallyRepeatableUnits.Count;
				}

				if (!repeatableDictionary.TryGetValue(ownerKey, 
						out UnitCollection<IInternalUnit> units))
				{
					return 0;
				}
				return units.Count;
			}
			else if (unitType == UnitType.Redoable)
			{
				if (ownerKey == null)
				{
					return globallyRedoableUnits.Count;
				}

				if (!redoableDictionary.TryGetValue(ownerKey, 
								out UnitCollection<IUndoableUnit> units))
				{
					return 0;
				}
				return units.Count;
			}
			else
			{
				throw new InvalidOperationException(
					"Unknown unit type: " + unitType);
			}
		}
	}
}
