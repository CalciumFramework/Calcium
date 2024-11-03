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

using FluentAssertions;
using Calcium.ComponentModel;
using Calcium.ResourcesModel.Experimental;
using Calcium.Services;

namespace Calcium.ResourcesModel
{
	/// <summary>
	/// <see cref="StringParserService"/> Tests.
	/// </summary>
	public partial class StringParserServiceTests
	{
		public static IEnumerable<object[]> GetImplementations()
		{
			yield return [new StringParserService()];
			yield return [new AsyncStringParser()];
		}

		//[Theory]
		//[MemberData(nameof(GetImplementations))]
		//public void TestTagShouldReturnValue(IStringParserService service)
		//{
		//	var result = service.Parse("${" + StringParserService.TestTag + "}");
		//	result.Should().Be(StringParserService.TestTagResult);
		//}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ShouldReturnTagWhenUnknown(IStringParserService service)
		{
			string text = "${l:Enum_ThisIsADummyEnumThatDoesNotExist_Auto}";
			var result = service.Parse(text);
			result.Should().Be(text);
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ConverterShouldReturnValue(IStringParserService service)
		{
			string tagValue = "Foo";
			MockConverter converter = new(tagValue);

			const string tagName = nameof(StringParserServiceTests);
			service.RegisterConverter(tagName, converter);
			string tag = $"${{" + tagName + $"}}";
			string format = $"This '{{0}}' should be replaced by '" + tagValue + "'.";
			var stringToParse = string.Format(format, tag);
			var result = service.Parse(stringToParse);

			var expectedResult = string.Format(format, tagValue);

			result.Should().Be(expectedResult);
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ArgsShouldBePassedToConverter(IStringParserService service)
		{
			string tagValue = "Foo";
			MockConverter converter = new(tagValue);

			string arg = "Bah";

			const string tagName = nameof(StringParserServiceTests);
			service.RegisterConverter(tagName, converter);
			string tag = $"${{" + tagName + $":{arg}}}";
			string format = $"This '{{0}}' should be replaced by '" + tagValue + "'.";
			var stringToParse = string.Format(format, tag);
			string result = service.Parse(stringToParse);

			string expectedResult = string.Format(format, tagValue);

			result.Should().Be(expectedResult);

			converter.Args.Should().Be(arg, "Tag argument was not correctly passed to converter.");
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ParseWithSubstitutionsOnlyTag(IStringParserService service)
		{
			string content1 = "${Tag1}";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			result1.Should().Be("Replacement1");
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ParseMultipleSubstitutions(IStringParserService service)
		{
			string content1 = "${Tag1} ${Tag1}";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			result1.Should().Be("Replacement1 Replacement1");
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ParseMultipleSubstitutionsWithMultipleTags(IStringParserService service)
		{
			string content1 = "${Tag1} ${Tag2} ${Tag3}";
			var substitutions = new Dictionary<string, string>
			{
				{"Tag1", "Replacement1"},
				{ "Tag2", "Replacement2" },
				{ "Tag3", "Replacement3" }
			};
			var result1 = service.Parse(content1, substitutions);
			result1.Should().Be("Replacement1 Replacement2 Replacement3");
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ParseWithSubstitutionsSpaceAtStart(IStringParserService service)
		{
			string content1 = " ${Tag1}";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			result1.Should().Be(" Replacement1");
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ParseWithSubstitutionsSpaceAtEnd(IStringParserService service)
		{
			string content1 = "${Tag1} ";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			result1.Should().Be("Replacement1 ");
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ParseWithSubstitutionsOtherContent(IStringParserService service)
		{
			string content1 = "This is ${Tag1} some text.";
			var substitutions = new Dictionary<string, string> { { "Tag1", "Replacement1" } };
			var result1 = service.Parse(content1, substitutions);
			result1.Should().Be("This is Replacement1 some text.");
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ParseWithCodeInContent(IStringParserService service)
		{
			string input = @"
P ${currentPrice}\n""; 

if 
";
			var result = service.Parse(input);
			result.Should().Be(input);
		}

		[Theory]
		[MemberData(nameof(GetImplementations))]
		public void ParseWithDollarSignPresentInContent(IStringParserService service)
		{
			string content1 = "string getUrl = $\"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{filePath}\";";
			var result1 = service.Parse(content1);
			result1.Should().Be(content1);
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

			public object? Args { get; private set; }
		}
	}
}
