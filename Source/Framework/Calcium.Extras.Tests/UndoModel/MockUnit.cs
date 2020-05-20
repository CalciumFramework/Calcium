#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 12:00:53Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.UndoModel
{
	class MockUnit : UnitBase<string>
	{
		public MockUnit()
		{
			Execute += OnExecute;	
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
				return "Mock unit";
			}
		}

		public int ExecutionCount { get; private set; }

		public string LastArgument { get; private set; }

		public bool RepeatableTest
		{
			get => Repeatable;
			set => Repeatable = value;
		}
	}
}
