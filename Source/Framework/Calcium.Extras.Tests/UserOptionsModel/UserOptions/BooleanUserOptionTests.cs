using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.UserOptionsModel.UserOptions
{
	[TestClass]
	public class BooleanUserOptionTests
	{
		[TestMethod]
		public async Task SetterShouldBeInvoked()
		{
			var values = new TestValues();
			var option = new BooleanUserOption(() => "Test", 
				() => Task.FromResult(values.Bool1), 
				value =>
				{
					values.Bool1 = value;
					return Task.FromResult(new SaveOptionResult());
				}, 
				() => false);

			var settingValue = await option.GetSetting();
			Assert.IsTrue(settingValue);

			var rw = option.ReaderWriter;

		}

		class TestValues
		{
			public bool Bool1 { get; set; } = true;
		}
	}
}
