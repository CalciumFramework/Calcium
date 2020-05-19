using System;
using System.Globalization;
using System.Threading.Tasks;
using Calcium.Concurrency;
using Calcium.MissingTypes.System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.UI.Data
{
	[TestClass]
	public class BindingApplicatorTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Dependency.Register<ISynchronizationContext>(new SynchronizationContextForTests());
		}

		[TestMethod]
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
			Assert.IsTrue(target.Bool1, "Changing binding value to true should update Bool1.");
			source.Bool1 = false;
			Assert.IsFalse(target.Bool1, "Changing binding value to false should update Bool1.");
		}

		[TestMethod]
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
			Assert.AreEqual(DummyVisibility.Visible, target.Visibility, "Changing binding value to true should update Visibility.");
			source.Bool1 = false;
			Assert.AreEqual(DummyVisibility.Invisible, target.Visibility, "Changing binding value to false should update Visibility.");
		}

		[TestMethod]
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
			Assert.IsTrue(target.Bool1, "Changing binding value to true should update Bool1.");
			source.Bool1 = false;
			Assert.IsFalse(target.Bool1, "Changing binding value to false should update Bool1.");

			Assert.IsNotNull(unbindAction);
			unbindAction();
			Assert.IsFalse(target.Bool1, "Unbinding should not change value.");
			source.Bool1 = true;
			Assert.IsFalse(target.Bool1, "Changing source value after unbind should have no effect.");
		}
	}
}
