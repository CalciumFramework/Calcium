#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:58Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.UIModel.Input
{
	[TestClass]
	public class UICommandTests
	{
		[TestMethod]
		public async Task ShouldSetTextSynchronous()
		{
			var command = new UICommand(o => { });

			Assert.IsTrue(string.IsNullOrEmpty(command.Text));

			string newText = "NewText";

			command.TextFunc = o => newText;

			await command.RefreshAsync();

			Assert.AreEqual(newText, command.Text);
		}

		[TestMethod]
		public async Task ShouldSetTextAsynchronous()
		{
			var command = new UICommand(o => {});

			Assert.IsTrue(string.IsNullOrEmpty(command.Text));

			string[] newText = {"NewText"};

			command.TextFunc = o => newText[0];

			await command.RefreshAsync();

			Assert.AreEqual(newText[0], command.Text);

			const string newText2 = "NewText2";
			newText[0] = newText2;

			await command.RefreshAsync();

			Assert.AreEqual(newText2, command.Text);
		}

		[TestMethod]
		public async Task ShouldSetVisibleAsynchronous()
		{
			var command = new UICommand(o => {});
			bool[] newVisibility = {false};
			command.IsVisibleFunc = o => newVisibility[0];

			await command.RefreshAsync();
			Assert.AreEqual(false, command.Visible);

			newVisibility[0] = true;
			await command.RefreshAsync();
			Assert.AreEqual(true, command.Visible);
		}

		[TestMethod]
		public async Task ShouldSetEnabledAsynchronous()
		{
			bool[] newEnabled = {false};
			var command = new UICommand( 
				o => {},
				o => newEnabled[0]);

			await command.RefreshAsync();

			Assert.AreEqual(false, command.Enabled);

			newEnabled[0] = true;
			await command.RefreshAsync();

			Assert.AreEqual(true, command.Enabled);
		}
	}
}
