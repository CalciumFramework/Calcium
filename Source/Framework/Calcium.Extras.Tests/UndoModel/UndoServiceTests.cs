#region File and License Information
/* 
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 11:59:01Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

using Xunit;

namespace Calcium.UndoModel
{
	/// <summary>
	///This is a test class for PropertyChangedNotifierTest and is intended
	///to contain all PropertyChangedNotifierTest Unit Tests
	///</summary>
	public class UndoServiceTests
	{
		[Fact]
		public void AGlobalTaskShouldBePerformed()
		{
			var mockTask = new MockUnit();
			string expectedValue1 = "1";
			var target = new UndoService();

			target.PerformUnit(mockTask, expectedValue1, null);

			mockTask.LastArgument.Should().Be(expectedValue1);
			mockTask.ExecutionCount.Should().Be(1);
		}

		[Fact]
		public void TaskServiceShouldUndoGlobalTasks()
		{
			TaskServiceShouldUndoTasks(null);
		}

		[Fact]
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

			target.CanUndo(key).Should().BeFalse();
			target.CanRedo(key).Should().BeFalse();
			target.CanRepeat(key).Should().BeFalse();

			target.PerformUnit(mockTask1, expectedValue1, key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(1);

			target.CanUndo(key).Should().BeTrue();
			target.CanRedo(key).Should().BeFalse();
			target.CanRepeat(key).Should().BeTrue();

			target.Undo(key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(0);

			target.CanUndo(key).Should().BeFalse();
			target.CanRedo(key).Should().BeTrue();
			target.CanRepeat(key).Should().BeFalse();

			target.Redo(key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(1);

			target.CanUndo(key).Should().BeTrue();
			target.CanRedo(key).Should().BeFalse();
			target.CanRepeat(key).Should().BeTrue();

			target.Repeat(key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(2);

			target.CanUndo(key).Should().BeTrue();
			target.CanRedo(key).Should().BeFalse();
			target.CanRepeat(key).Should().BeTrue();

			target.Repeat(key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(3);

			target.CanUndo(key).Should().BeTrue();
			target.CanRedo(key).Should().BeFalse();
			target.CanRepeat(key).Should().BeTrue();

			target.Undo(key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(2);

			target.CanUndo(key).Should().BeTrue();
			target.CanRedo(key).Should().BeTrue();
			target.CanRepeat(key).Should().BeTrue();

			target.Undo(key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(1);

			target.CanUndo(key).Should().BeTrue();
			target.CanRedo(key).Should().BeTrue();
			target.CanRepeat(key).Should().BeTrue();

			target.Undo(key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(0);

			target.CanUndo(key).Should().BeFalse();
			target.CanRedo(key).Should().BeTrue();
			target.CanRepeat(key).Should().BeFalse();

			var mockTask2 = new MockUndoableUnit { RepeatableTest = true };
			target.PerformUnit(mockTask1, expectedValue1, key);
			target.PerformUnit(mockTask2, expectedValue2, key);

			mockTask1.LastArgument.Should().Be(expectedValue1);
			mockTask1.ExecutionCount.Should().Be(1);
			mockTask2.LastArgument.Should().Be(expectedValue2);
			mockTask2.ExecutionCount.Should().Be(1);

			target.CanUndo(key).Should().BeTrue();
			target.CanRedo(key).Should().BeFalse();
			target.CanRepeat(key).Should().BeTrue();

			target.Undo(key);

			mockTask1.ExecutionCount.Should().Be(1);
			mockTask2.ExecutionCount.Should().Be(0);

			target.CanUndo(key).Should().BeTrue();
			target.CanRedo(key).Should().BeTrue();
			target.CanRepeat(key).Should().BeTrue();

			var list = target.GetRepeatableUnits(key);
			list.Should().NotBeNull();
			list.ToList().Count.Should().Be(1);
		}

		[Fact]
		public void GetRepeatableTasksShouldReturnRepeatableTasks()
		{
			GetRepeatableTasksShouldReturnRepeatableTasks2(null);
		}

		[Fact]
		public void GetRepeatableGlobalTasksShouldReturnRepeatableTasks()
		{
			GetRepeatableTasksShouldReturnRepeatableTasks2(new object());
		}

		void GetRepeatableTasksShouldReturnRepeatableTasks2(object key)
		{
			var task1 = new MockUnit { RepeatableTest = true };
			var target = new UndoService();
			var list = target.GetRepeatableUnits(key);
			list.Count().Should().BeLessThan(1);
			target.PerformUnit(task1, string.Empty, key);
			list = target.GetRepeatableUnits(key);
			list.Count().Should().BeGreaterThan(0);
		}

		[Fact]
		public void NonRepeatableGlobalTaskShouldNotBeRepeatable()
		{
			var task1 = new MockUnit();
			var target = new UndoService();
			string arg1 = "1";
			target.CanRepeat(null).Should().BeFalse();
			target.PerformUnit(task1, arg1, null);
			target.CanRepeat(null).Should().BeFalse();
		}

		[Fact]
		public void RepeatableGlobalTaskShouldBeRepeatable()
		{
			var task1 = new MockUnit {RepeatableTest = true};
			var target = new UndoService();
			string arg1 = "1";
			target.CanRepeat(null).Should().BeFalse();
			target.PerformUnit(task1, arg1, null);
			target.CanRepeat(null).Should().BeTrue();
			target.Repeat(null);
			task1.ExecutionCount.Should().Be(2);
		}

		[Fact]
		public void NonRepeatableTaskShouldNotBeRepeatable()
		{
			var task1 = new MockUnit();
			var target = new UndoService();
			object key = new object();
			target.CanRepeat(key).Should().BeFalse();
			target.PerformUnit(task1, "1", key);
			target.CanRepeat(key).Should().BeFalse();
		}

		[Fact]
		public void RepeatableTaskShouldBeRepeatable()
		{
			var task1 = new MockUnit { RepeatableTest = true };
			var target = new UndoService();
			object key = new object();
			target.CanRepeat(key).Should().BeFalse();
			target.PerformUnit(task1, "1", key);
			target.CanRepeat(key).Should().BeTrue();
			target.Repeat(key);
			task1.ExecutionCount.Should().Be(2);
		}

		[Fact]
		public void UndoableTaskShouldBeRedoable()
		{
			UndoableTaskShouldBeRedoable2(null);
		}

		[Fact]
		public void UndoableGlobalTaskShouldBeRedoable()
		{
			UndoableTaskShouldBeRedoable2(new object());
		}

		void UndoableTaskShouldBeRedoable2(object key)
		{
			var task1 = new MockUndoableUnit();
			var target = new UndoService();
			target.CanRedo(key).Should().BeFalse();
			target.PerformUnit(task1, "1", key);
			target.Undo(key);
			target.CanRedo(key).Should().BeTrue();
		}

		[Fact]
		public void NonUndoableTasksShouldNotBeUndoable()
		{
			NonUndoableTasksShouldNotBeUndoable2(null);
		}

		[Fact]
		public void NonUndoableGlobalTasksShouldNotBeUndoable()
		{
			NonUndoableTasksShouldNotBeUndoable2(new object());
		}

		[Fact]
		public void PerformingGlobalTaskShouldClearUndoableList()
		{
			PerformingTaskShouldClearUndoableList2(null);
		}

		[Fact]
		public void PerformingTaskShouldClearUndoableList()
		{
			PerformingTaskShouldClearUndoableList2(new object());
		}
		
		void PerformingTaskShouldClearUndoableList2(object key)
		{
			/* First set up some undoable tasks. */
			var task1 = new MockUndoableUnit();
			var target = new UndoService();
			target.PerformUnit(task1, "1", key);			
			target.CanUndo(key).Should().BeTrue();

			/* Perform a non-undoable unit. This must clear the undoable list. */
			var task2 = new MockUnit { RepeatableTest = true };
			target.PerformUnit(task2, "1", key);
			target.CanUndo(key).Should().BeFalse();
		}

		void NonUndoableTasksShouldNotBeUndoable2(object key)
		{
			var task1 = new MockUnit();
			var target = new UndoService();
			target.CanUndo(key).Should().BeFalse();
			target.PerformUnit(task1, "1", key);
			target.CanUndo(key).Should().BeFalse();
		}

		[Fact]
		public async Task CompositeTasksShouldbePerformedInParallel()
		{
			await CompositeTasksShouldBePerformedInParallel(new object());
		}

		[Fact]
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
				mockTask.ExecutionCount.Should().Be(1);
			}
		}

		[Fact]
		public void CompositeUndoableTasksShouldbePerformedInParallel()
		{
			CompositeUndoableTasksShouldBePerformedInParallel(new object());
		}

		[Fact]
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
				mockTask.ExecutionCount.Should().Be(1);
			}

			/* Test undo. */
			target.Undo(contextKey);

			foreach (KeyValuePair<UndoableUnitBase<string>, string> keyValuePair in tasks)
			{
				var mockTask = (MockUndoableUnit)keyValuePair.Key;
				mockTask.ExecutionCount.Should().Be(0);
			}
		}

		[Fact]
		public void AGlobalUndoLimitShouldBeEnforced()
		{
			string key = "key";
			var mockTask = new MockUnit();
			string expectedValue1 = "1";
			var target = new UndoService();
			target.SetMaximumUndoCount(2, key);

			target.GetUnitCount(UndoService.UnitType.Repeatable, key).Should()
				.Be(0, "Repeatable unit count should be 0.");

			target.GetUnitCount(UndoService.UnitType.Undoable, key).Should().Be(0, "Undoable unit count should be 0.");
			target.GetUnitCount(UndoService.UnitType.Redoable, key).Should().Be(0, "Redoable unit count should be 0.");

			for (int i = 0; i < 5; i++)
			{
				target.PerformUnit(mockTask, expectedValue1, key);
			}

			target.GetUnitCount(UndoService.UnitType.Repeatable, key).Should().Be(2, "Repeatable tasks.");
			target.GetUnitCount(UndoService.UnitType.Undoable, key).Should().Be(0, "Undoable tasks.");
			target.GetUnitCount(UndoService.UnitType.Redoable, key).Should().Be(0, "Redoable tasks.");

			MockUndoableUnit mockUndoableUnit = new MockUndoableUnit();

			for (int i = 0; i < 5; i++)
			{
				target.PerformUnit(mockUndoableUnit, i.ToString(), key);
			}

			target.GetUnitCount(UndoService.UnitType.Repeatable, key).Should().Be(2, "Repeatable tasks.");
			target.GetUnitCount(UndoService.UnitType.Undoable, key).Should().Be(2, "Undoable tasks.");
		}
	}
}
