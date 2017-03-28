using System.Collections.Generic;
using System.Threading.Tasks;
using Codon.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Codon.Concurrency;

namespace Codon.UIModel.Validation
{
	[TestClass]
	public class DataErrorNotifierTests
	{
		[TestInitialize]
		public void Initialize()
		{
			UIContext.SetTestContext(new SynchronizationContextForTests());
		}

		[TestMethod]
		public async Task ShouldValidateSynchronous()
		{
			var s1 = new SimpleValidatable1();
			var validator = new DataValidatorForTesting();
			var notifier = new DataErrorNotifier(s1, validator);
			notifier.AddValidationProperty(nameof(SimpleValidatable1.Text1), () => s1.Text1);

			validator.Result = new ValidationCompleteEventArgs(nameof(SimpleValidatable1.Text1));
			await notifier.ValidateAllAsync();
			Assert.IsFalse(notifier.HasErrors);

			var e1 = new List<DataValidationError> {new DataValidationError {ErrorMessage = "Failed Validation 1"}};
			validator.Result = new ValidationCompleteEventArgs(nameof(SimpleValidatable1.Text1), e1);
			await notifier.ValidateAllAsync();
			Assert.IsTrue(notifier.HasErrors);
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
