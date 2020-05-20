#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:39Z</CreationDate>
</File>
*/
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.UIModel.Input
{
	[TestClass]
	public class ActionCommandTests
	{
		[TestMethod]
		public void ShouldRaiseCanExecuteChangedSynchronous()
		{
			bool canExecuteCalled = false;
			ActionCommand command = new ActionCommand(o => {}, o =>
			{
				canExecuteCalled = true;
				return true;
			});

			command.Refresh();

			Assert.IsTrue(canExecuteCalled);
		}

		[TestMethod]
		public void ShouldRaiseCanExecuteChangedGenericSynchronous()
		{
			bool canExecuteCalled = false;
			var command = new ActionCommand<int>(o => { }, o =>
			{
				canExecuteCalled = true;
				return true;
			});

			command.Refresh();

			Assert.IsTrue(canExecuteCalled);
		}

		[TestMethod]
		public void ShouldExecuteSynchronous()
		{
			bool executeCalled = false;
			var command = new ActionCommand<int>(o =>
			{
				executeCalled = true;
			});

			command.Execute();

			Assert.IsTrue(executeCalled);
		}

		[TestMethod]
		public void ShouldSetEnabledToFalseSynchronous()
		{
			var command = new ActionCommand<int>(o => {}, o => false);
			command.Refresh();

			Assert.IsFalse(command.Enabled);
		}

		[TestMethod]
		public void ShouldInitializeEnabledToTrueSynchronous()
		{
			var command = new ActionCommand<int>(o => { }, o => true);

			Assert.IsTrue(command.Enabled);
		}

		[TestMethod]
		public void ShouldReceiveParameterSynchronous()
		{
			int receivedParameter = 0;
			var command = new ActionCommand<int>(o =>
			{
				receivedParameter = o;
			});

			int parameterValue = 5;
			command.Execute(parameterValue);

			Assert.AreEqual(parameterValue, receivedParameter);
		}
	}
}
