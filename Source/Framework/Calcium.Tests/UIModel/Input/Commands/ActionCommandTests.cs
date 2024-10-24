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

using FluentAssertions;

namespace Calcium.UIModel.Input
{
	public class ActionCommandTests
	{
		[Fact]
		public void ShouldRaiseCanExecuteChangedSynchronous()
		{
			bool canExecuteCalled = false;
			ActionCommand command
				= new(_ => { }, _ =>
								{
									canExecuteCalled = true;
									return true;
								});

			command.Refresh();

			canExecuteCalled.Should().BeTrue();
		}

		[Fact]
		public void ShouldRaiseCanExecuteChangedGenericSynchronous()
		{
			bool canExecuteCalled = false;
			ActionCommand<int> command
				= new(_ => { },
					_ =>
					{
						canExecuteCalled = true;
						return true;
					});

			command.Refresh();

			canExecuteCalled.Should().BeTrue();
		}

		[Fact]
		public void ShouldExecuteSynchronous()
		{
			bool executeCalled = false;
			ActionCommand<int> command
				= new(_ =>
					  {
						  executeCalled = true;
					  });

			command.Execute();

			executeCalled.Should().BeTrue();
		}

		[Fact]
		public void ShouldSetEnabledToFalseSynchronous()
		{
			ActionCommand<int> command = new(_ => { }, _ => false);
			command.Refresh();

			command.Enabled.Should().BeFalse();
		}

		[Fact]
		public void ShouldInitializeEnabledToTrueSynchronous()
		{
			ActionCommand<int> command = new(_ => { }, _ => true);

			command.Enabled.Should().BeTrue();
		}

		[Fact]
		public void ShouldReceiveParameterSynchronous()
		{
			int receivedParameter = 0;
			ActionCommand<int> command 
				= new(o =>
					  {
						  receivedParameter = o;
					  });

			int parameterValue = 5;
			command.Execute(parameterValue);

			receivedParameter.Should().Be(parameterValue);
		}
	}
}
