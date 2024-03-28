using FluentAssertions;
using Xunit;

using Calcium.Concurrency;

namespace Calcium.UI.Data
{
	public class BindingApplicatorTests
	{
		public BindingApplicatorTests()
		{
			Dependency.Register<ISynchronizationContext>(new SynchronizationContextForTests());
		}

		[Fact]
		public void BindingShouldAllowUpdate()
		{
			var applicator = new BindingApplicator();

			var bindingExpression = new BindingExpression
			{
				//ConverterParameter = "Visible,Hidden,Hidden",
				Path = nameof(ViewModelTestClass.Bool1),
				Target = nameof(ViewTestClass.Bool1)
			};

			var target = new ViewTestClass();
			var source = new ViewModelTestClass();

			applicator.ApplyBinding(bindingExpression, target, source);

			source.Bool1 = true;
			target.Bool1.Should().BeTrue("Changing binding value to true should update Bool1.");

			source.Bool1 = false;
			target.Bool1.Should().BeFalse("Changing binding value to false should update Bool1.");
		}

		[Fact]
		public void ValueConverterShouldApplyValue()
		{
			var applicator = new BindingApplicator();

			var bindingExpression = new BindingExpression
			{
				Path = nameof(ViewModelTestClass.Bool1),
				Target = nameof(ViewTestClass.Visibility)
			};

			var target = new ViewTestClass();
			var source = new ViewModelTestClass();

			var converter = new DummyBooleanToVisibilityConverter();

			applicator.ApplyBinding(bindingExpression, target, source, converter);

			source.Bool1 = true;
			target.Visibility.Should().Be(DummyVisibility.Visible,
				"Changing binding value to true should update Visibility.");

			source.Bool1 = false;
			target.Visibility.Should().Be(DummyVisibility.Invisible,
				"Changing binding value to false should update Visibility.");
		}

		[Fact]
		public void ShouldUnbind()
		{
			var applicator = new BindingApplicator();

			var bindingExpression = new BindingExpression
			{
				Path = nameof(ViewModelTestClass.Bool1),
				Target = nameof(ViewTestClass.Bool1)
			};

			var target = new ViewTestClass();
			var source = new ViewModelTestClass();

			var unbindAction = applicator.ApplyBinding(bindingExpression, target, source);

			source.Bool1 = true;
			target.Bool1.Should().BeTrue("Changing binding value to true should update Bool1.");

			source.Bool1 = false;
			target.Bool1.Should().BeFalse("Changing binding value to false should update Bool1.");

			unbindAction.Should().NotBeNull();

			unbindAction();
			target.Bool1.Should().BeFalse("Unbinding should not change value.");

			source.Bool1 = true;
			target.Bool1.Should().BeFalse("Changing source value after unbind should have no effect.");
		}
	}
}
