using FluentAssertions;

using Xunit;

namespace Calcium.UserOptionsModel.UserOptions
{
	public class BooleanUserOptionTests
	{
		[Fact]
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
			settingValue.Should().BeTrue();

			var rw = option.ReaderWriter;

		}

		class TestValues
		{
			public bool Bool1 { get; set; } = true;
		}
	}
}
