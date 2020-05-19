#if __ANDROID__
using Android.Views;
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Codon.UI.Elements.ValueConverters
{
	[TestClass]
	public partial class BooleanToVisibilityConverterTests
	{
		[TestMethod]
		public void ShouldReturnCorrectVibility()
		{
			var converter = new BooleanToVisibilityConverter();

			foreach (var valuePair in expectedResultsDictionary)
			{
				var key = valuePair.Key;
				var parameter = key.Parameter;
				var value = key.Value;
				var expectedResult = valuePair.Value;
				var result = (ViewStates)converter.Convert(value, null, parameter, null);
				Assert.AreEqual(expectedResult, result, $"value: {value}, parameter: '{parameter}'.");
			}
			
		}
	}
}
