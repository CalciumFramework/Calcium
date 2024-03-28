#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:45Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

using Xunit;

namespace Calcium.UIModel.Input
{
	public class AsyncUICommandTests
	{
		[Fact]
		public async Task ShouldSetTextAsynchronous()
		{
			var command = new AsyncUICommand(o => Task.CompletedTask);

			string.IsNullOrEmpty(command.Text).Should().BeTrue();

			string[] newText = { "NewText" };

			command.TextFunc = o => Task.FromResult(newText[0]);

			await command.RefreshAsync();

			command.Text.Should().Be(newText[0]);

			const string newText2 = "NewText2";
			newText[0] = newText2;

			await command.RefreshAsync();

			command.Text.Should().Be(newText2);
		}
	}
}
