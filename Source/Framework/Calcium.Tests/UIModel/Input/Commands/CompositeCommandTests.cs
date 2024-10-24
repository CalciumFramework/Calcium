#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:52Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.UIModel.Input
{
	public class CompositeCommandTests
	{
		[Fact]
		public void ShouldResolveTextFromCorrectCommand()
		{
			UICommand? activeCommand;
			UICommand? c1 = null, c2 = null, c3 = null;
			c1 = new UICommand(_ =>
			{
				activeCommand = c1;
			})
			{ Text = "c1" };

			c2 = new UICommand(_ =>
			{
				activeCommand = c2;
			})
			{ Text = "c2" };

			c3 = new UICommand(_ =>
			{
				activeCommand = c3;
			})
			{ Text = "c3" };

			var command = new UICompositeCommand(c1, c2, c3);

			command.Text.Should().Be("c1");

			command.SelectedCommandIndex = 1;

			command.Text.Should().Be("c2");

			command.SelectedCommand = c3;

			command.Text.Should().Be("c3");
		}

		[Fact]
		public void ShouldRaisePropertyChangedEvents()
		{
			var c1 = new UICommand(_ => { }) { Text = "c1" };
			var c2 = new UICommand(_ => { }) { Text = "c2" };
			var c3 = new UICommand(_ => { }) { Text = "c3" };

			string? newText = null;

			UICompositeCommand command = new(c1, c2, c3);

			command.PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(UICompositeCommand.Text))
				{
					newText = command.Text;
				}
			};

			const string updatedText1 = "UpdatedText1";
			c1.Text = updatedText1;

			newText.Should().Be(updatedText1);

			command.SelectedCommandIndex = 1;

			const string updatedText2 = "UpdatedText2";
			c2.Text = updatedText2;

			newText.Should().Be(updatedText2);
		}
	}
}
