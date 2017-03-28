#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 12:00:47Z</CreationDate>
</File>
*/
#endregion

namespace Codon.UndoModel
{
	class MockUndoableUnit : UndoableUnitBase<string>
	{
		public MockUndoableUnit()
		{
			Execute += OnExecute;
			Undo += OnUndo;
		}

		public int ExecutionCount { get; private set; }
		public string LastArgument { get; private set; }

		void OnUndo(object sender, UnitEventArgs<string> e)
		{
			ExecutionCount--;
			e.UnitResult = UnitResult.Completed;
		}
       
		void OnExecute(object sender, UnitEventArgs<string> e)
		{
			ExecutionCount++;
			LastArgument = e.Argument;
		}

		public override string DescriptionForUser
		{
			get
			{
				return "Mock Undoable unit";
			}
		}

		public bool RepeatableTest
		{
			get => Repeatable;
			set => Repeatable = value;
		}
        
	}
}
