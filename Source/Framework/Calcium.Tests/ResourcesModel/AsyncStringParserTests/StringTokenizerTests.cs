#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-03 23:38:49Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;

using Calcium.ResourcesModel.Experimental;

using FluentAssertions;

namespace Calcium.ResourcesModel
{
	public class StringTokenizerTests
	{
		#region Basic Tag Extraction Tests

		[Fact]
		public void Tokenize_SingleTag_ReturnsSingleSegment()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "The ${Tag}.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Tag");
			result[0].Index.Should().Be(4);
			result[0].Length.Should().Be(6);
		}

		[Fact]
		public void Tokenize_SingleTag_AtEndOfString_ReturnsSingleSegment()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "The ${Tag}";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Tag");
			result[0].Index.Should().Be(4);
			result[0].Length.Should().Be(6);
		}

		[Fact]
		public void Tokenize_SingleTag_AtStartOfString_ReturnsSingleSegment()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${Tag} is at the start.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Tag");
			result[0].Index.Should().Be(0);
			result[0].Length.Should().Be(6);
		}

		#endregion

		#region Handling of Multiple Tags Tests

		[Fact]
		public void Tokenize_MultipleTags_ReturnsMultipleSegments()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "This ${First} has multiple ${Second} tags.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(2);
			result[0].TagName.Should().Be("First");
			result[0].Index.Should().Be(5);
			result[0].Length.Should().Be(8);
			result[1].TagName.Should().Be("Second");
			result[1].Index.Should().Be(27);
			result[1].Length.Should().Be(9);
		}

		[Fact]
		public void Tokenize_AdjacentTags_ReturnsMultipleSegments()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${First}${Second}";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(2);
			result[0].TagName.Should().Be("First");
			result[0].Index.Should().Be(0);
			result[0].Length.Should().Be(8);
			result[1].TagName.Should().Be("Second");
			result[1].Index.Should().Be(8);
			result[1].Length.Should().Be(9);
		}

		[Fact]
		public void Tokenize_TagsWithTextInBetween_ReturnsMultipleSegments()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "Text ${First} in between ${Second} and more.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(2);
			result[0].TagName.Should().Be("First");
			result[0].Index.Should().Be(5);
			result[0].Length.Should().Be(8);
			result[1].TagName.Should().Be("Second");
			result[1].Index.Should().Be(25);
			result[1].Length.Should().Be(9);
		}

		#endregion

		#region Tags with Arguments Tests

		[Fact]
		public void Tokenize_TagWithArgument_SplitsNameAndArgument()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "Display ${Tag:Argument} here.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Tag");
			result[0].TagArg.Should().Be("Argument");
			result[0].Index.Should().Be(8);
			result[0].Length.Should().Be(15);
		}

		[Fact]
		public void Tokenize_MultipleTagsWithArguments_ReturnsSegmentsWithArguments()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${Tag1:Arg1} and ${Tag2:Arg2}";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(2);
			result[0].TagName.Should().Be("Tag1");
			result[0].TagArg.Should().Be("Arg1");
			result[0].Index.Should().Be(0);
			result[0].Length.Should().Be(12);
			result[1].TagName.Should().Be("Tag2");
			result[1].TagArg.Should().Be("Arg2");
			result[1].Index.Should().Be(17);
			result[1].Length.Should().Be(12);
		}

		[Fact]
		public void Tokenize_TagWithEmptyArgument_ReturnsSegmentWithEmptyArg()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "Test ${Tag:} for empty argument.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Tag");
			result[0].TagArg.Should().BeEmpty();
			result[0].Index.Should().Be(5);
			result[0].Length.Should().Be(7);
		}

		#endregion

		#region No Tags Present Tests

		[Fact]
		public void Tokenize_InputWithNoTags_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "No tags here.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public void Tokenize_EmptyString_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public void Tokenize_TextWithDelimitersOnly_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${}";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		#endregion

		#region Incomplete Tags Tests

		[Fact]
		public void Tokenize_IncompleteTag_NoClosingDelimiter_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "This is an incomplete ${Tag.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public void Tokenize_IncompleteTagWithTextAfter_NoClosingDelimiter_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "Prefix ${Tag and more text after.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public void Tokenize_MultipleIncompleteTags_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${Tag1 ${Tag2 ${Tag3";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		#endregion

		#region Nested or Overlapping Tags Tests

		//[Fact]
		//public void Tokenize_NestedTags_ReturnsOuterTagOnly()
		//{
		//	// Arrange
		//	var tokenizer = new StringTokenizer();
		//	TagDelimiters delimiters = new("${", "}");
		//	string input = "${Outer ${Inner} Tag}";

		//	// Act
		//	var result = tokenizer.Tokenize(input, delimiters);

		//	// Assert
		//	result.Should().HaveCount(1);
		//	result[0].TagName.Should().Be("Outer ${Inner} Tag");
		//	result[0].Index.Should().Be(0);
		//	result[0].Length.Should().Be(20);
		//}

		[Fact]
		public void Tokenize_OverlappingTags_ReturnsFirstTagOnly()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${Outer ${Inner}";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Outer ${Inner");
			result[0].Index.Should().Be(0);
			result[0].Length.Should().Be(16);
		}

		#endregion

		#region Edge Cases Tests

		[Fact]
		public void Tokenize_DelimitersAtStart_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${StartOnly";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public void Tokenize_DelimitersAtEnd_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "Ends with delimiter ${";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public void Tokenize_EmptyString_ReturnsEmptyList2()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public void Tokenize_StringWithOnlyDelimiters_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${}";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		#endregion

		#region Delimiter Edge Cases Tests

		[Fact]
		public void Tokenize_SimilarDelimitersAtEnd_NoClosingDelimiter_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${Start} but ends with ${End";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Start");
			result[0].Index.Should().Be(0);
			result[0].Length.Should().Be(8);
		}

		[Fact]
		public void Tokenize_MismatchedDelimiters_NoClosing_ReturnsEmptyList()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "Text with ${Tag but no closing";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public void Tokenize_SimilarDelimitersAtStartAndEnd_OnlyFirstTagParsed()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = "${Tag1} with similar delimiter ${Tag2";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Tag1");
			result[0].Index.Should().Be(0);
			result[0].Length.Should().Be(7);
		}

		#endregion

		#region Whitespace Handling Tests

		[Fact]
		public void Tokenize_TagWithWhitespace_IgnoresLeadingAndTrailingWhitespace()
		{
			// Arrange
			StringTokenizer tokenizer = new StringTokenizer();
			TagDelimiters delimiters = new("${", "}");
			string input = "Text with ${   Tag   } and more text.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Tag");
			result[0].Index.Should().Be(10);
			result[0].Length.Should().Be(12);
		}

		[Fact]
		public void Tokenize_TagWithArgAndWhitespace_IgnoresWhitespaceAroundTagAndArg()
		{
			// Arrange
			StringTokenizer tokenizer = new StringTokenizer();
			TagDelimiters delimiters = new("${", "}");
			string input = "Text with ${ Tag :   Arg   } and more text.";

			// Act
			var result = tokenizer.Tokenize(input, delimiters);

			// Assert
			result.Should().HaveCount(1);
			result[0].TagName.Should().Be("Tag");
			result[0].TagArg.Should().Be("Arg");
			result[0].Index.Should().Be(10);
			result[0].Length.Should().Be(18);
		}

		#endregion

		#region Performance Tests

		[Fact]
		public void Tokenize_LargeInputWithManyTags_PerformanceTest()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input
				= string.Concat(Enumerable.Repeat("This is a test ${Tag} ", 100000)); // Large input with repeated tags

			// Act
			Stopwatch watch = Stopwatch.StartNew();
			var result = tokenizer.Tokenize(input, delimiters);
			watch.Stop();

			// Assert
			result.Count.Should().Be(100000);
			watch.ElapsedMilliseconds.Should().BeLessThan(5000); // Arbitrary threshold for performance
		}

		[Fact]
		public void Tokenize_HighDensityOfTags_PerformanceTest()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = string.Concat(Enumerable.Repeat("${Tag}", 100000)); // High-density tags with minimal spacing

			// Act
			Stopwatch watch = Stopwatch.StartNew();
			var result = tokenizer.Tokenize(input, delimiters);
			watch.Stop();

			// Assert
			result.Count.Should().Be(100000);
			watch.ElapsedMilliseconds.Should().BeLessThan(3000); // Performance threshold
		}

		[Fact]
		public void Tokenize_ManyIncompleteTags_PerformanceTest()
		{
			// Arrange
			StringTokenizer tokenizer = new();
			TagDelimiters delimiters = new("${", "}");
			string input = string.Concat(Enumerable.Repeat("This is incomplete ${Tag", 100000)); // Many incomplete tags

			// Act
			Stopwatch watch = Stopwatch.StartNew();
			var result = tokenizer.Tokenize(input, delimiters);
			watch.Stop();

			// Assert
			result.Should().BeEmpty();
			watch.ElapsedMilliseconds.Should().BeLessThan(5000); // Performance threshold
		}

		#endregion

	}
}