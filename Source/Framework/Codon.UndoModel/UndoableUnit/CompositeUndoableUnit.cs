#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-02-06 18:16:24Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if NETFX_CORE
using Console = System.Diagnostics.Debug;
using Windows.System.Threading;
#endif

namespace Codon.UndoModel
{
	/// <summary>
	/// Provides the ability to group units for sequential or parallel execution 
	/// with undo and redo capabilities.
	/// </summary>
	public class CompositeUndoableUnit<T> : UndoableUnitBase<T>
	{
		readonly string descriptionForUser;
		readonly Dictionary<UndoableUnitBase<T>, T> unitDictionary;

		public CompositeUndoableUnit(IDictionary<UndoableUnitBase<T>, T> units, string descriptionForUser)
		{
			this.descriptionForUser = descriptionForUser ?? throw new ArgumentNullException(nameof(descriptionForUser)); //AssertArg.IsNotNull(descriptionForUser, nameof(descriptionForUser));
			//AssertArg.IsNotNull(units, nameof(units));

			unitDictionary = new Dictionary<UndoableUnitBase<T>, T>(units ?? throw new ArgumentNullException(nameof(units)));
			Execute += OnExecute;
			Undo += OnUndo;

			/* Determine repeatable status. */
			bool repeatable = unitDictionary.Keys.Count > 0;
			foreach (IInternalUnit key in units.Keys)
			{
				if (!key.Repeatable)
				{
					repeatable = false;
				}
			}
			Repeatable = repeatable;
		}
        
		void OnExecute(object sender, UnitEventArgs<T> e)
		{
			ExecuteInternal(unitDictionary, e.UnitMode);
		}

		protected internal virtual void ExecuteInternal(
			Dictionary<UndoableUnitBase<T>, T> unitDictionary, UnitMode unitMode)
		{
			if (Parallel)
			{
				ExecuteInParallel(unitDictionary, unitMode);
			}
			else
			{
				ExecuteSequentially(unitDictionary, unitMode);
			}
		}

		static void ExecuteSequentially(Dictionary<UndoableUnitBase<T>, T> unitDictionary, UnitMode unitMode)
		{
			var performedUnits = new List<UndoableUnitBase<T>>();
			foreach (KeyValuePair<UndoableUnitBase<T>, T> pair in unitDictionary)
			{
				var unit = (IInternalUnit)pair.Key;
				try
				{
					unit.PerformUnit(pair.Value, unitMode);
					performedUnits.Add(pair.Key);
				}
				catch (Exception)
				{
					/* TODO: improve this to capture undone unit errors. */
					SafelyUndoUnits(performedUnits);
					throw;
				}
			}
		}

		static void SafelyUndoUnits(IEnumerable<IUndoableUnit> undoableUnits)
		{
			try
			{
				foreach (var undoableUnit in undoableUnits)
				{
					try
					{
						undoableUnit.Undo();
					}
					catch (Exception ex)
					{
						/* Ignore for now. TODO: implement internal log. */
						try
						{
							Console.WriteLine("Exception thrown undoing units " + ex);
						}
						catch (Exception)
						{
							/* WPF in .NET Standard may raise exception here. */
						}
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					Console.WriteLine("Exception thrown undoing units " + ex);
				}
				catch (Exception)
				{	
					/* WPF in .NET Standard may raise exception here. */
				}
			}
		}

		void OnUndo(object sender, UnitEventArgs<T> e)
		{
			UndoInternal(unitDictionary);
		}

		protected internal virtual void UndoInternal(
			Dictionary<UndoableUnitBase<T>, T> unitDictionary)
		{
			if (Parallel)
			{
				UndoInParallel(unitDictionary);
			}
			else
			{
				UndoSequentially(unitDictionary);
			}
		}

		static void UndoSequentially(Dictionary<UndoableUnitBase<T>, T> undoDictionary)
		{
			/* Undo sequentially. */
			foreach (KeyValuePair<UndoableUnitBase<T>, T> pair in undoDictionary)
			{
				var undoableUnit = (IUndoableUnit)pair.Key;
				undoableUnit.Undo();
			}
		}

		public override string DescriptionForUser => descriptionForUser;

		#region Parallel Execution
		public bool Parallel { get; set; }

		static void ExecuteInParallel(Dictionary<UndoableUnitBase<T>, T> unitDictionary, UnitMode unitMode)
		{
			/* When we move to .NET 4 we may use System.Threading.Parallel for the Desktop CLR. */
			var performedUnits = new List<UndoableUnitBase<T>>();
			object performedUnitsLock = new object();
			var exceptions = new List<Exception>();
			object exceptionsLock = new object();
			var events = unitDictionary.ToDictionary(x => x, x => new AutoResetEvent(false));

			foreach (KeyValuePair<UndoableUnitBase<T>, T> pair in unitDictionary)
			{
				var autoResetEvent = events[pair];
				var unit = (IInternalUnit)pair.Key;
				var undoableUnit = pair.Key;
				var arg = pair.Value;

				Task.Run(
					() =>
					{
						try
						{
							unit.PerformUnit(arg, unitMode);
							lock (performedUnitsLock)
							{
								performedUnits.Add(undoableUnit);
							}
						}
						catch (Exception ex)
						{
							/* TODO: improve this to capture undone unit errors. */
							lock (exceptionsLock)
							{
								exceptions.Add(ex);
							}
						}
						autoResetEvent.Set();
					});

			}

			foreach (var autoResetEvent in events.Values)
			{
				autoResetEvent.WaitOne();
			}

			if (exceptions.Count > 0)
			{
				SafelyUndoUnits(performedUnits);
				throw new AggregateException("Unable to undo units", exceptions);
			}
		}

		static void UndoInParallel(Dictionary<UndoableUnitBase<T>, T> unitDictionary)
		{
			/* When we move to .NET 4 we may use System.Threading.Parallel for the Desktop CLR. */
			var performedUnits = new List<UndoableUnitBase<T>>();
			object performedUnitsLock = new object();
			var exceptions = new List<Exception>();
			object exceptionsLock = new object();
			var events = unitDictionary.ToDictionary(x => x, x => new AutoResetEvent(false));

			foreach (KeyValuePair<UndoableUnitBase<T>, T> pair in unitDictionary)
			{
				var autoResetEvent = events[pair];
				var undoableUnit = pair.Key;

				Task.Run(
					delegate
					{
						try
						{
							((IUndoableUnit)undoableUnit).Undo();
							lock (performedUnitsLock)
							{
								performedUnits.Add(undoableUnit);
							}
						}
						catch (Exception ex)
						{
							/* TODO: improve this to capture undone unit errors. */
							lock (exceptionsLock)
							{
								exceptions.Add(ex);
							}
						}
						autoResetEvent.Set();
					});

			}

			foreach (var autoResetEvent in events.Values)
			{
				autoResetEvent.WaitOne();
			}

			if (exceptions.Count > 0)
			{
				SafelyUndoUnits(performedUnits);
				throw new AggregateException("Unable to undo units", exceptions);
			}
		}
		#endregion
	}
}
