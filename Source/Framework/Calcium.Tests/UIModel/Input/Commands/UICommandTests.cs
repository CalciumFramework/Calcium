#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:58Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.UIModel.Input
{
	public class UICommandTests
	{
		[Fact]
		public async Task ShouldSetTextSynchronous()
		{
			UICommand command = new(_ => { });

			command.Text.Should().BeNullOrEmpty();

			string newText = "NewText";

			command.TextFunc = _ => newText;

			await command.RefreshAsync();

			command.Text.Should().Be(newText);
		}

		[Fact]
		public async Task ShouldSetTextAsynchronous()
		{
			UICommand command = new(_ => { });

			command.Text.Should().BeNullOrEmpty();

			string[] newText = { "NewText" };

			command.TextFunc = _ => newText[0];

			await command.RefreshAsync();

			command.Text.Should().Be(newText[0]);

			const string newText2 = "NewText2";
			newText[0] = newText2;

			await command.RefreshAsync();

			command.Text.Should().Be(newText2);
		}

		[Fact]
		public async Task ShouldSetVisibleAsynchronous()
		{
			UICommand command = new(_ => { });
			bool[] newVisibility = { false };
			command.IsVisibleFunc = _ => newVisibility[0];

			await command.RefreshAsync();
			command.Visible.Should().BeFalse();

			newVisibility[0] = true;
			await command.RefreshAsync();
			command.Visible.Should().BeTrue();
		}

		[Fact]
		public async Task ShouldSetEnabledAsynchronous()
		{
			bool[] newEnabled = { false };

			UICommand command = new(
				_ => { },
				_ => newEnabled[0]);

			await command.RefreshAsync();

			command.Enabled.Should().BeFalse();

			newEnabled[0] = true;
			await command.RefreshAsync();

			command.Enabled.Should().BeTrue();
		}
	}
}
