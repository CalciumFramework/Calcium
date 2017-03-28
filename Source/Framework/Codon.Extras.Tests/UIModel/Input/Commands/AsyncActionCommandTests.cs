#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:00Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Codon.UIModel.Input;

namespace Codon.Input.CommandModel
{
	/// <summary>
	/// <see cref="AsyncActionCommand"/> tests.
	/// </summary>
	[TestClass]
	public class AsyncActionCommandTests
	{
		[TestMethod]
		public async Task ShouldExecuteAsynchronous()
		{
			bool executeCalled = false;
			var command = new AsyncActionCommand<int>(o =>
			{
				executeCalled = true;
				return Task.FromResult((object)null);
			});

			await command.ExecuteAsync();

			Assert.IsTrue(executeCalled);
		}

		[TestMethod]
		public async Task ShouldRaiseCanExecuteChangedGenericAsynchronous()
		{
			bool canExecuteCalled = false;
			var command = new AsyncActionCommand<int>(o => Task.FromResult((object)null),
				o =>
				{
					canExecuteCalled = true;
					return Task.FromResult(true);
				});

			await command.RefreshAsync();

			Assert.IsTrue(canExecuteCalled);
		}

		[TestMethod]
		public async Task ShouldRaiseCanExecuteChangedAsynchronous()
		{
			bool canExecuteCalled = false;
			var command = new AsyncActionCommand(o => Task.FromResult((object)null),
				o =>
				{
					canExecuteCalled = true;
					return Task.FromResult(true);
				});

			await command.RefreshAsync();

			Assert.IsTrue(canExecuteCalled);
		}

		[TestMethod]
		public async Task ShouldReceiveParameterAsynchronous()
		{
			int receivedParameter = 0;
			var command = new AsyncActionCommand<int>(o =>
			{
				receivedParameter = o;
				return Task.FromResult((object)null);
			});

			int parameterValue = 5;
			await command.ExecuteAsync(parameterValue);

			Assert.AreEqual(parameterValue, receivedParameter);
		}

		[TestMethod]
		public async Task ShouldSetEnabledToFalseAsynchronous()
		{
			bool[] canExecute = { true };
			var command = new AsyncActionCommand<int>(
				o => Task.FromResult((object)null),
				async o =>
				{
					await Task.Yield();
					return canExecute[0];
				});

			await command.RefreshAsync();
			Assert.IsTrue(command.Enabled);

			canExecute[0] = false;

			await command.RefreshAsync();
			Assert.IsFalse(command.Enabled);
		}
	}
}
