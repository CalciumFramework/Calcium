#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-20 05:03:12Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.ResourcesModel
{
	public partial class StringParserServiceTests
	{
		[Fact]
		public void ParseWithMissingEndDelimiter_ShouldReturnUnmodifiedText()
		{
			StringParserService service = new();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=T1";  // Missing the closing %>

			var substitutions = new Dictionary<string, string>
			{
				{ "T1", "Replacement1" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// Since the end delimiter is missing, it should return the content as-is.
			result.Should().Be(content);
		}

		[Fact]
		public void ParseWithEmptyString_ShouldReturnEmptyString()
		{
			StringParserService service = new();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = string.Empty;  // Empty input string
			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "Replacement1" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// The result should be empty since the input is empty.
			result.Should().Be(string.Empty);
		}

		[Fact]
		public void ParseWithoutDelimiters_ShouldReturnUnmodifiedText()
		{
			StringParserService service = new();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "This is just plain text without any tags.";  // No tags or delimiters

			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "Replacement1" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// Since no delimiters are present, the content should remain the same.
			result.Should().Be(content);
		}

		[Fact]
		public void ParseWithEmptyTags_ShouldReturnUnmodifiedTag()
		{
			StringParserService service = new();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=   %>";  // Tag with only spaces inside
			var substitutions = new Dictionary<string, string>();

			var result = service.Parse(content, substitutions, customDelimiters);

			// Since the tag is just spaces, the parser should return the tag itself.
			result.Should().Be(content);
		}

		[Fact]
		public void ParseWithSpecialCharacters_ShouldHandleCorrectly()
		{
			StringParserService service = new();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=Tag1%> \"Quoted Text\" \n Escape \\ Sequences & Symbols!";
			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "SpecialValue" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// The tag should be replaced but the special characters should remain unmodified.
			result.Should().Be("SpecialValue \"Quoted Text\" \n Escape \\ Sequences & Symbols!");
		}

		[Fact]
		public void ParseWithConsecutiveTags_ShouldReplaceCorrectly()
		{
			StringParserService service = new();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=Tag1%><%=Tag2%>";
			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "Replacement1" },
				{ "Tag2", "Replacement2" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// Both tags should be replaced correctly even though they're consecutive.
			result.Should().Be("Replacement1Replacement2");
		}

		#region Recursive Tags (Not yet supported)

		// [Fact]
		// public void ParseWithNestedTags_ShouldReplaceNestedTagAndReturnUnmodifiedOuterText()
		// {
		//     var service = new StringParserService();
		//     var customDelimiters = new TagDelimiters("<%=", "%>");

		//     string content = "<%=Tag1<%=Tag2%>%>"; // A nested tag, which should be treated as raw text
		//     var substitutions = new Dictionary<string, string>
		//     {
		//         { "Tag1", "Replacement1" },
		//         { "Tag2", "Replacement2" }
		//     };

		//     var result = service.Parse(content, substitutions, customDelimiters);

		//     // Since there's no proper way to handle nested tags, it should return the input as is.
		//     result.Should().Be("<%=Tag1Replacement2%>");
		// }

		// [Fact]
		// public void ParseWithUnclosedTagFollowedByValidTag_ShouldHandleCorrectly()
		// {
		//     var service = new StringParserService();
		//     var customDelimiters = new TagDelimiters("<%=", "%>");

		//     string content = "<%=T1 <%=T2%>";
		//     var substitutions = new Dictionary<string, string>
		//     {
		//         { "T1", "Replacement1" },
		//         { "T2", "Replacement2" }
		//     };

		//     var result = service.Parse(content, substitutions, customDelimiters);

		//     // The unclosed tag should be left unmodified, but the valid tag should be replaced.
		//     result.Should().Be("<%=T1 Replacement2%>");
		// }

		// [Fact]
		// public void ParseWithNestedTags_ShouldHandleCorrectly()
		// {
		//     var service = new StringParserService();
		//     var customDelimiters = new TagDelimiters("<%=", "%>");

		//     string content = "<%=T<%=T2%>%>";
		//     var substitutions = new Dictionary<string, string>
		//     {
		//         { "T1", "Replacement1" },
		//         { "T2", "1" }
		//     };

		//     var result = service.Parse(content, substitutions, customDelimiters);

		//     // Tag2 should be replaced with Replacement2, and then Tag1 should be replaced with Replacement1.
		//     result.Should().Be("Replacement1");
		// }

		#endregion
	}
}
