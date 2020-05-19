using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
				var result = (Visibility)converter.Convert(value, null, parameter, null);
				Assert.AreEqual(expectedResult, result, $"value: {value}, parameter: '{parameter}'.");
			}
			
		}
	}
}
