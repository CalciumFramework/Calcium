using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.ResourcesModel
{
	partial class StringParserServiceTests
	{
		[TestMethod]
		public void ParseWithCustomDelimiters_ShouldReturnValue()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=Tag1%>";
			var substitutions = new Dictionary<string, string> { { "Tag1", "CustomReplacement" } };

			var result = service.Parse(content, substitutions, customDelimiters);

			Assert.AreEqual("CustomReplacement", result);
		}

		[TestMethod]
		public void ParseWithCustomDelimiters_MultipleSubstitutions()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=Tag1%> <%=Tag2%>";
			var substitutions = new Dictionary<string, string>
			{
				{ "Tag1", "CustomReplacement1" },
				{ "Tag2", "CustomReplacement2" }
			};

			var result = service.Parse(content, substitutions, customDelimiters);

			Assert.AreEqual("CustomReplacement1 CustomReplacement2", result);
		}

		[TestMethod]
		public void ParseWithCustomDelimiters_ShouldReturnOriginalTextForUnknownTags()
		{
			var service = new StringParserService();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			string content = "<%=UnknownTag%>";

			var result = service.Parse(content, new Dictionary<string, string>(), customDelimiters);

			Assert.AreEqual(content, result); // Should return the original text if the tag is unknown
		}
	}
}