using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.ResourcesModel
{
	public partial class StringParserServiceTests
	{
		[TestMethod]
		public void ParseWithMissingEndDelimiter_ShouldReturnUnmodifiedText()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=T1";  // Missing the closing %>

			var substitutions = new Dictionary<string, string>
			{
				{ "T1", "Replacement1" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// Since the end delimiter is missing, it should return the content as-is.
			Assert.AreEqual(content, result);
		}

		[TestMethod]
		public void ParseWithEmptyString_ShouldReturnEmptyString()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = string.Empty;  // Empty input string
			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "Replacement1" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// The result should be empty since the input is empty.
			Assert.AreEqual(string.Empty, result);
		}

		[TestMethod]
		public void ParseWithoutDelimiters_ShouldReturnUnmodifiedText()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "This is just plain text without any tags.";  // No tags or delimiters

			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "Replacement1" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// Since no delimiters are present, the content should remain the same.
			Assert.AreEqual(content, result);
		}

		[TestMethod]
		public void ParseWithEmptyTags_ShouldReturnUnmodifiedTag()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=   %>";  // Tag with only spaces inside
			var substitutions = new Dictionary<string, string>();

			var result = service.Parse(content, substitutions, customDelimiters);

			// Since the tag is just spaces, the parser should return the tag itself.
			Assert.AreEqual(content, result);
		}

		[TestMethod]
		public void ParseWithSpecialCharacters_ShouldHandleCorrectly()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=Tag1%> \"Quoted Text\" \n Escape \\ Sequences & Symbols!";
			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "SpecialValue" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// The tag should be replaced but the special characters should remain unmodified.
			Assert.AreEqual("SpecialValue \"Quoted Text\" \n Escape \\ Sequences & Symbols!", result);
		}

		[TestMethod]
		public void ParseWithConsecutiveTags_ShouldReplaceCorrectly()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=Tag1%><%=Tag2%>";
			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "Replacement1" },
				{ "Tag2", "Replacement2" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			// Both tags should be replaced correctly even though they're consecutive.
			Assert.AreEqual("Replacement1Replacement2", result);
		}

		#region Recursive Tags (Not yet supported)

		//[TestMethod]
		//public void ParseWithNestedTags_ShouldReplaceNestedTagAndReturnUnmodifiedOuterText()
		//{
		//	var service = new StringParserService();
		//	var customDelimiters = new TagDelimiters("<%=", "%>");

		//	string content = "<%=Tag1<%=Tag2%>%>"; // A nested tag, which should be treated as raw text
		//	var substitutions = new Dictionary<string, string>
		//	{
		//		{ "Tag1", "Replacement1" },
		//		{ "Tag2", "Replacement2" }
		//	};

		//	var result = service.Parse(content, substitutions, customDelimiters);

		//	// Since there's no proper way to handle nested tags, it should return the input as is.
		//	Assert.AreEqual("<%=Tag1Replacement2%>", result);
		//}

		//[TestMethod]
		//public void ParseWithUnclosedTagFollowedByValidTag_ShouldHandleCorrectly()
		//{
		//	var service = new StringParserService();
		//	var customDelimiters = new TagDelimiters("<%=", "%>");

		//	string content = "<%=T1 <%=T2%>";
		//	var substitutions = new Dictionary<string, string>
		//	{
		//		{ "T1", "Replacement1" },
		//		{ "T2", "Replacement2" }
		//	};

		//	var result = service.Parse(content, substitutions, customDelimiters);

		//	// The unclosed tag should be left unmodified, but the valid tag should be replaced.
		//	Assert.AreEqual("<%=T1 Replacement2%>", result);
		//}

		//[TestMethod]
		//public void ParseWithNestedTags_ShouldHandleCorrectly()
		//{
		//	var service = new StringParserService();
		//	var customDelimiters = new TagDelimiters("<%=", "%>");

		//	string content = "<%=T<%=T2%>%>";
		//	var substitutions = new Dictionary<string, string>
		//	{
		//		{ "T1", "Replacement1" },
		//		{ "T2", "1" }
		//	};

		//	var result = service.Parse(content, substitutions, customDelimiters);

		//	// Tag2 should be replaced with Replacement2, and then Tag1 should be replaced with Replacement1.
		//	Assert.AreEqual("Replacement1", result);
		//}

		#endregion
	}
}