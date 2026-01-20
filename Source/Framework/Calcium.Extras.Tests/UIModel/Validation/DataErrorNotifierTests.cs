using Calcium.ComponentModel;

using Calcium.Concurrency;

using FluentAssertions;

using Xunit;

namespace Calcium.UIModel.Validation
{
	public class DataErrorNotifierTests
	{
		public DataErrorNotifierTests()
		{
			UIContext.SetTestContext(new SynchronizationContextForTests());
		}

		[Fact]
		public async Task ShouldValidateSynchronous()
		{
			var s1 = new SimpleValidatable1();
			var validator = new DataValidatorForTesting();
			var notifier = new DataErrorNotifier(s1, validator);
			notifier.AddValidationProperty(nameof(SimpleValidatable1.Text1), () => s1.Text1);

			validator.Result = new ValidationCompleteEventArgs(nameof(SimpleValidatable1.Text1));
			await notifier.ValidateAllAsync();
			notifier.HasErrors.Should().BeFalse();
			
			var e1 = new List<DataValidationError> {new DataValidationError(Guid.NewGuid(), "Failed Validation 1")};
			validator.Result = new ValidationCompleteEventArgs(nameof(SimpleValidatable1.Text1), e1);
			await notifier.ValidateAllAsync();
			notifier.HasErrors.Should().BeTrue();
		}

		class SimpleValidatable1 : ObservableBase
		{
			public string Text1 { get; set; }
		}

		class DataValidatorForTesting : IValidateData
		{
			internal ValidationCompleteEventArgs Result { get; set; }

			public Task<ValidationCompleteEventArgs> ValidateAsync(string propertyName, object value)
			{
				return Task.FromResult(Result);
			}
		}
	}
}
