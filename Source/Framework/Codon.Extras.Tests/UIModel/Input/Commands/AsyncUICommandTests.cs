#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:20:45Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.UIModel.Input
{
	[TestClass]
	public class AsyncUICommandTests
	{
		[TestMethod]
		public async Task ShouldSetTextAsynchronous()
		{
			var command = new AsyncUICommand(o => Task.FromResult((object)null));

			Assert.IsTrue(string.IsNullOrEmpty(command.Text));

			string[] newText = { "NewText" };

			command.TextFunc = o => Task.FromResult(newText[0]);

			await command.RefreshAsync();

			Assert.AreEqual(newText[0], command.Text);

			const string newText2 = "NewText2";
			newText[0] = newText2;

			await command.RefreshAsync();

			Assert.AreEqual(newText2, command.Text);
		}
	}
}
