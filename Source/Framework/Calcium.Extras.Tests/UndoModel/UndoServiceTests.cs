#region File and License Information
/* 
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 11:59:01Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.UndoModel
{
    /// <summary>
    ///This is a test class for PropertyChangedNotifierTest and is intended
    ///to contain all PropertyChangedNotifierTest Unit Tests
    ///</summary>
	[TestClass]
	public class UndoServiceTests
	{
		[TestMethod]
		public void AGlobalTaskShouldBePerformed()
		{
			var mockTask = new MockUnit();
			string expectedValue1 = "1";
			var target = new UndoService();

			target.PerformUnit(mockTask, expectedValue1, null);

			Assert.AreEqual(mockTask.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask.ExecutionCount, 1);
		}

		[TestMethod]
		public void TaskServiceShouldUndoGlobalTasks()
		{
			TaskServiceShouldUndoTasks(null);
		}

		[TestMethod]
		public void TaskServiceShouldUndoNonGlobalTasks()
		{
			TaskServiceShouldUndoTasks(new object());
		}

		void TaskServiceShouldUndoTasks(object key)
		{
			var mockTask1 = new MockUndoableUnit { RepeatableTest = true };
			string expectedValue1 = "1";
			string expectedValue2 = "2";
			var target = new UndoService();

			Assert.IsFalse(target.CanUndo(key));
			Assert.IsFalse(target.CanRedo(key));
			Assert.IsFalse(target.CanRepeat(key));

			target.PerformUnit(mockTask1, expectedValue1, key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 1);

			Assert.IsTrue(target.CanUndo(key));
			Assert.IsFalse(target.CanRedo(key));
			Assert.IsTrue(target.CanRepeat(key));

			target.Undo(key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 0);

			Assert.IsFalse(target.CanUndo(key));
			Assert.IsTrue(target.CanRedo(key));
			Assert.IsFalse(target.CanRepeat(key));

			target.Redo(key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 1);

			Assert.IsTrue(target.CanUndo(key));
			Assert.IsFalse(target.CanRedo(key));
			Assert.IsTrue(target.CanRepeat(key));

			target.Repeat(key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 2);

			Assert.IsTrue(target.CanUndo(key));
			Assert.IsFalse(target.CanRedo(key));
			Assert.IsTrue(target.CanRepeat(key));

			target.Repeat(key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 3);

			Assert.IsTrue(target.CanUndo(key));
			Assert.IsFalse(target.CanRedo(key));
			Assert.IsTrue(target.CanRepeat(key));

			target.Undo(key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 2);

			Assert.IsTrue(target.CanUndo(key));
			Assert.IsTrue(target.CanRedo(key));
			Assert.IsTrue(target.CanRepeat(key));

			target.Undo(key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 1);

			Assert.IsTrue(target.CanUndo(key));
			Assert.IsTrue(target.CanRedo(key));
			Assert.IsTrue(target.CanRepeat(key));

			target.Undo(key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 0);

			Assert.IsFalse(target.CanUndo(key));
			Assert.IsTrue(target.CanRedo(key));
			Assert.IsFalse(target.CanRepeat(key));

			var mockTask2 = new MockUndoableUnit { RepeatableTest = true };
			target.PerformUnit(mockTask1, expectedValue1, key);
			target.PerformUnit(mockTask2, expectedValue2, key);

			Assert.AreEqual(mockTask1.LastArgument, expectedValue1);
			Assert.AreEqual(mockTask1.ExecutionCount, 1);
			Assert.AreEqual(mockTask2.LastArgument, expectedValue2);
			Assert.AreEqual(mockTask2.ExecutionCount, 1);

			Assert.IsTrue(target.CanUndo(key));
			Assert.IsFalse(target.CanRedo(key));
			Assert.IsTrue(target.CanRepeat(key));

			target.Undo(key);

			Assert.AreEqual(mockTask1.ExecutionCount, 1);
			Assert.AreEqual(mockTask2.ExecutionCount, 0);

			Assert.IsTrue(target.CanUndo(key));
			Assert.IsTrue(target.CanRedo(key));
			Assert.IsTrue(target.CanRepeat(key));

			var list = target.GetRepeatableUnits(key);
			Assert.IsNotNull(list);
			Assert.IsTrue(list.ToList().Count == 1);
		}

		[TestMethod]
		public void GetRepeatableTasksShouldReturnRepeatableTasks()
		{
			GetRepeatableTasksShouldReturnRepeatableTasks(null);
		}

		[TestMethod]
		public void GetRepeatableGlobalTasksShouldReturnRepeatableTasks()
		{
			GetRepeatableTasksShouldReturnRepeatableTasks(new object());
		}

		void GetRepeatableTasksShouldReturnRepeatableTasks(object key)
		{
			var task1 = new MockUnit { RepeatableTest = true };
			var target = new UndoService();
			var list = target.GetRepeatableUnits(key);
			Assert.IsTrue(list.Count() < 1);
			target.PerformUnit(task1, string.Empty, key);
			list = target.GetRepeatableUnits(key);
			Assert.IsTrue(list.Count() > 0);
		}

		[TestMethod]
		public void NonRepeatableGlobalTaskShouldNotBeRepeatable()
		{
			var task1 = new MockUnit();
			var target = new UndoService();
			string arg1 = "1";
			Assert.IsFalse(target.CanRepeat(null));
			target.PerformUnit(task1, arg1, null);
			Assert.IsFalse(target.CanRepeat(null));
		}

		[TestMethod]
		public void RepeatableGlobalTaskShouldBeRepeatable()
		{
			var task1 = new MockUnit {RepeatableTest = true};
			var target = new UndoService();
			string arg1 = "1";
			Assert.IsFalse(target.CanRepeat(null));
			target.PerformUnit(task1, arg1, null);
			Assert.IsTrue(target.CanRepeat(null));
			target.Repeat(null);
			Assert.IsTrue(task1.ExecutionCount == 2);
		}

		[TestMethod]
		public void NonRepeatableTaskShouldNotBeRepeatable()
		{
			var task1 = new MockUnit();
			var target = new UndoService();
			object key = new object();
			Assert.IsFalse(target.CanRepeat(key));
			target.PerformUnit(task1, "1", key);
			Assert.IsFalse(target.CanRepeat(key));
		}

		[TestMethod]
		public void RepeatableTaskShouldBeRepeatable()
		{
			var task1 = new MockUnit { RepeatableTest = true };
			var target = new UndoService();
			object key = new object();
			Assert.IsFalse(target.CanRepeat(key));
			target.PerformUnit(task1, "1", key);
			Assert.IsTrue(target.CanRepeat(key));
			target.Repeat(key);
			Assert.IsTrue(task1.ExecutionCount == 2);
		}

		[TestMethod]
		public void UndoableTaskShouldBeRedoable()
		{
			UndoableTaskShouldBeRedoable(null);
		}

		[TestMethod]
		public void UndoableGlobalTaskShouldBeRedoable()
		{
			UndoableTaskShouldBeRedoable(new object());
		}

		void UndoableTaskShouldBeRedoable(object key)
		{
			var task1 = new MockUndoableUnit();
			var target = new UndoService();
			Assert.IsFalse(target.CanRedo(key));
			target.PerformUnit(task1, "1", key);
			target.Undo(key);
			Assert.IsTrue(target.CanRedo(key));
		}

		[TestMethod]
		public void NonUndoableTasksShouldNotBeUndoable()
		{
			NonUndoableTasksShouldNotBeUndoable(null);
		}

		[TestMethod]
		public void NonUndoableGlobalTasksShouldNotBeUndoable()
		{
			NonUndoableTasksShouldNotBeUndoable(new object());
		}

		[TestMethod]
		public void PerformingGlobalTaskShouldClearUndoableList()
		{
			PerformingTaskShouldClearUndoableList(null);
		}

		[TestMethod]
		public void PerformingTaskShouldClearUndoableList()
		{
			PerformingTaskShouldClearUndoableList(new object());
		}
		
		void PerformingTaskShouldClearUndoableList(object key)
		{
			/* First set up some undoable tasks. */
			var task1 = new MockUndoableUnit();
			var target = new UndoService();
			target.PerformUnit(task1, "1", key);			
			Assert.IsTrue(target.CanUndo(key));

			/* Perform a non-undoable unit. This must clear the undoable list. */
			var task2 = new MockUnit { RepeatableTest = true };
			target.PerformUnit(task2, "1", key);
			Assert.IsFalse(target.CanUndo(key));
		}

		void NonUndoableTasksShouldNotBeUndoable(object key)
		{
			var task1 = new MockUnit();
			var target = new UndoService();
			Assert.IsFalse(target.CanUndo(key));
			target.PerformUnit(task1, "1", key);
			Assert.IsFalse(target.CanUndo(key));
		}

		[TestMethod]
		public async Task CompositeTasksShouldbePerformedInParallel()
		{
			await CompositeTasksShouldBePerformedInParallel(new object());
		}

		[TestMethod]
		public async Task GlobalCompositeTasksShouldBePerformedInParallel()
		{
			await CompositeTasksShouldBePerformedInParallel(null);
		}


		/* This method must be marked async so that the tasks run to completion. */
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		async Task CompositeTasksShouldBePerformedInParallel(object contextKey)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			var tasks = new Dictionary<UnitBase<string>, string>();
			for (int i = 0; i < 100; i++)
			{
				tasks.Add(new MockUnit(), i.ToString());
			}
			var compositeTask = new CompositeUnit<string>(tasks, "1") { Parallel = true };
			var target = new UndoService();
			target.PerformUnit(compositeTask, null, contextKey);
			foreach (KeyValuePair<UnitBase<string>, string> keyValuePair in tasks)
			{
				var mockTask = (MockUnit)keyValuePair.Key;
				Assert.AreEqual(1, mockTask.ExecutionCount);
			}
		}

		[TestMethod]
		public void CompositeUndoableTasksShouldbePerformedInParallel()
		{
			CompositeUndoableTasksShouldBePerformedInParallel(new object());
		}

		[TestMethod]
		public void GlobalCompositeUndoableTasksShouldBePerformedInParallel()
		{
			CompositeUndoableTasksShouldBePerformedInParallel(null);
		}

		void CompositeUndoableTasksShouldBePerformedInParallel(object contextKey)
		{
			var tasks = new Dictionary<UndoableUnitBase<string>, string>();
			for (int i = 0; i < 100; i++)
			{
				tasks.Add(new MockUndoableUnit(), i.ToString());
			}
			var compositeTask = new CompositeUndoableUnit<string>(tasks, "1") { Parallel = true };
			var target = new UndoService();
			target.PerformUnit(compositeTask, null, contextKey);
			foreach (KeyValuePair<UndoableUnitBase<string>, string> keyValuePair in tasks)
			{
				var mockTask = (MockUndoableUnit)keyValuePair.Key;
				Assert.AreEqual(1, mockTask.ExecutionCount);
			}

			/* Test undo. */
			target.Undo(contextKey);

			foreach (KeyValuePair<UndoableUnitBase<string>, string> keyValuePair in tasks)
			{
				var mockTask = (MockUndoableUnit)keyValuePair.Key;
				Assert.AreEqual(0, mockTask.ExecutionCount);
			}
		}

		[TestMethod]
		public void AGlobalUndoLimitShouldBeEnforced()
		{
			string key = "key";
			var mockTask = new MockUnit();
			string expectedValue1 = "1";
			var target = new UndoService();
			target.SetMaximumUndoCount(2, key);

			Assert.AreEqual(0, target.GetUnitCount(UndoService.UnitType.Repeatable, key), "Repeatable unit count should be 0.");
			Assert.AreEqual(0, target.GetUnitCount(UndoService.UnitType.Undoable, key), "Undoable unit count should be 0.");
			Assert.AreEqual(0, target.GetUnitCount(UndoService.UnitType.Redoable, key), "Redoable unit count should be 0.");

			for (int i = 0; i < 5; i++)
			{
				target.PerformUnit(mockTask, expectedValue1, key);
			}

			Assert.AreEqual(2, target.GetUnitCount(UndoService.UnitType.Repeatable, key), "Repeatable tasks.");
			Assert.AreEqual(0, target.GetUnitCount(UndoService.UnitType.Undoable, key), "Undoable tasks.");
			Assert.AreEqual(0, target.GetUnitCount(UndoService.UnitType.Redoable, key), "Redoable tasks.");

			MockUndoableUnit mockUndoableUnit = new MockUndoableUnit();

			for (int i = 0; i < 5; i++)
			{
				target.PerformUnit(mockUndoableUnit, i.ToString(), key);
			}

			Assert.AreEqual(2, target.GetUnitCount(UndoService.UnitType.Repeatable, key), "Repeatable tasks.");
			Assert.AreEqual(2, target.GetUnitCount(UndoService.UnitType.Undoable, key), "Undoable tasks.");
		}
	}
}
