#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:00Z</CreationDate>
</File>
*/
#endregion

using Calcium.UIModel.Input;

using FluentAssertions;

using Xunit;

namespace Calcium.Input.CommandModel
{
	/// <summary>
	/// <see cref="AsyncActionCommand"/> tests.
	/// </summary>
	public class AsyncActionCommandTests
	{
		[Fact]
		public async Task ShouldExecuteAsynchronous()
		{
			bool executeCalled = false;
			var command = new AsyncActionCommand<int>(o =>
			{
				executeCalled = true;
				return Task.CompletedTask;
			});

			await command.ExecuteAsync();

			executeCalled.Should().BeTrue();
		}

		[Fact]
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

			canExecuteCalled.Should().BeTrue();
		}

		[Fact]
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

			canExecuteCalled.Should().BeTrue();
		}

		[Fact]
		public async Task ShouldReceiveParameterAsynchronous()
		{
			int receivedParameter = 0;
			var command = new AsyncActionCommand<int>(o =>
			{
				receivedParameter = o;
				return Task.CompletedTask;
			});

			int parameterValue = 5;
			await command.ExecuteAsync(parameterValue);

			receivedParameter.Should().Be(parameterValue);
		}

		[Fact]
		public async Task ShouldSetEnabledToFalseAsynchronous()
		{
			bool[] canExecute = { true };
			var command = new AsyncActionCommand<int>(
				o => Task.CompletedTask,
				async o =>
				{
					await Task.Yield();
					return canExecute[0];
				});

			await command.RefreshAsync();
			command.Enabled.Should().BeTrue();

			canExecute[0] = false;

			await command.RefreshAsync();
			command.Enabled.Should().BeFalse();
		}
	}
}
