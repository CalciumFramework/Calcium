#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:23:27Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using Calcium.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.ResourcesModel
{
	/// <summary>
	/// <see cref="StringParserService"/> Tests.
	/// </summary>
	[TestClass]
	public class StringParserServiceTests
	{
		[TestMethod]
		public void TestTagShouldReturnValue()
		{
			var service = new StringParserService();
			var result = service.Parse("${" + StringParserService.TestTag + "}");
			Assert.AreEqual(StringParserService.TestTagResult, result);
		}

		[TestMethod]
		public void ShouldReturnTagWhenUnknown()
		{
			var service = new StringParserService();
			string text = "${l:Enum_ThisIsADummyEnumThatDoesntExist_Auto}";
			var result = service.Parse(text);
			Assert.AreEqual(text, result);
		}

		[TestMethod]
		public void ConverterShouldReturnValue()
		{
			var service = new StringParserService();
			string tagValue = "Foo";
			var converter = new MockConverter(tagValue);

			const string tagName = nameof(StringParserServiceTests);
			service.RegisterConverter(tagName, converter);
			string tag = $"${{" + tagName + $"}}";
			string format = $"This '{{0}}' should be replaced by '" + tagValue + "'.";
			var stringToParse = string.Format(format, tag);
			var result = service.Parse(stringToParse);

			var expectedResult = string.Format(format, tagValue);

			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void ArgsShouldBePassedToConverter()
		{
			var service = new StringParserService();
			string tagValue = "Foo";
			var converter = new MockConverter(tagValue);

			string arg = "Bah";

			const string tagName = nameof(StringParserServiceTests);
			service.RegisterConverter(tagName, converter);
			string tag = $"${{" + tagName + $":{arg}}}";
			string format = $"This '{{0}}' should be replaced by '" + tagValue + "'.";
			var stringToParse = string.Format(format, tag);
			var result = service.Parse(stringToParse);

			var expectedResult = string.Format(format, tagValue);

			Assert.AreEqual(expectedResult, result);

			Assert.AreEqual(arg, converter.Args, 
				"Tag argument was not correctly passed to converter.");
		}

		[TestMethod]
		public void ParseWithSubstitutionsOnlyTag()
		{
			var service = new StringParserService();
			string content1 = "${Tag1}";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			Assert.AreEqual("Replacement1", result1);
		}

		[TestMethod]
		public void ParseMultipleSubstitutions()
		{
			var service = new StringParserService();
			string content1 = "${Tag1} ${Tag1}";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			Assert.AreEqual("Replacement1 Replacement1", result1);
		}

		[TestMethod]
		public void ParseMultipleSubstitutionsWithMultipleTags()
		{
			var service = new StringParserService();
			string content1 = "${Tag1} ${Tag2} ${Tag3}";
			var substitutions = new Dictionary<string, string>
			{
				{"Tag1", "Replacement1"},
				{ "Tag2", "Replacement2" },
				{ "Tag3", "Replacement3" }
			};
			var result1 = service.Parse(content1, substitutions);
			Assert.AreEqual("Replacement1 Replacement2 Replacement3", result1);
		}

		[TestMethod]
		public void ParseWithSubstitutionsSpaceAtStart()
		{
			var service = new StringParserService();
			string content1 = " ${Tag1}";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			Assert.AreEqual(" Replacement1", result1);
		}

		[TestMethod]
		public void ParseWithSubstitutionsSpaceAtEnd()
		{
			var service = new StringParserService();
			string content1 = "${Tag1} ";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			Assert.AreEqual("Replacement1 ", result1);
		}

		[TestMethod]
		public void ParseWithSubstitutionsOtherContent()
		{
			var service = new StringParserService();
			string content1 = "This is ${Tag1} some text.";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			Assert.AreEqual("This is Replacement1 some text.", result1);
		}

		public class MockConverter : IConverter
		{
			readonly string conversionResult;

			public MockConverter(string conversionResult)
			{
				this.conversionResult = conversionResult;
			}

			public object Convert(object args)
			{
				Args = args;

				return conversionResult;
			}

			public object Args { get; private set; }
		}
	}
}
