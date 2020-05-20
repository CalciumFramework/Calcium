#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:52Z</CreationDate>
</File>
*/
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.UIModel.Input
{
	[TestClass]
	public class CompositeCommandTests
	{
		[TestMethod]
		public void ShouldResolveTextFromCorrectCommand()
		{
			UICommand activeCommand;
			UICommand c1 = null, c2 = null, c3 = null;
			c1 = new UICommand(o =>
			{
				activeCommand = c1;
			}) { Text = "c1" };

			c2 = new UICommand(o =>
			{
				activeCommand = c2;
			}) { Text = "c2" };

			c3 = new UICommand(o =>
			{
				activeCommand = c3;
			}) { Text = "c3" };

			var command = new UICompositeCommand(c1, c2, c3);

			Assert.AreEqual("c1", command.Text);

			command.SelectedCommandIndex = 1;

			Assert.AreEqual("c2", command.Text);

			command.SelectedCommand = c3;

			Assert.AreEqual("c3", command.Text);
		}

		[TestMethod]
		public void ShouldRaisePropertyChangedEvents()
		{
			var c1 = new UICommand(o => { }) { Text = "c1" };

			var c2 = new UICommand(o => { }) { Text = "c2" };

			var c3 = new UICommand(o => { }) { Text = "c3" };

			string newText = null;

			var command = new UICompositeCommand(c1, c2, c3);
			command.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == nameof(UICompositeCommand.Text))
				{
					newText = command.Text;
				}
			};

			const string updatedText1 = "UpdatedText1";
			c1.Text = updatedText1;

			Assert.AreEqual(updatedText1, newText);

			command.SelectedCommandIndex = 1;

			const string updatedText2 = "UpdatedText2";
			c2.Text = updatedText2;
			Assert.AreEqual(updatedText2, newText);
		}
	}
}
