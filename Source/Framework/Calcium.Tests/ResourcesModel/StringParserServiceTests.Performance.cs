using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.ResourcesModel
{
	partial class StringParserServiceTests
	{
		[TestMethod]
		public void ParsePerformanceTest_LargeInput()
		{
			var service = new StringParserService();

			// Generate a large input string
			StringBuilder largeInputBuilder = new StringBuilder();
			var substitutions = new Dictionary<string, string>();

			for (int i = 1; i <= 10000; i++)
			{
				largeInputBuilder.Append($"<%=Tag{i}%> ");
				substitutions[$"Tag{i}"] = $"Replacement{i}";
			}

			string largeInput = largeInputBuilder.ToString();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			// Measure the performance
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

			var result = service.Parse(largeInput, substitutions, customDelimiters);

			stopwatch.Stop();

			// Check time and result validity
			Console.WriteLine($"Performance Test Execution Time: {stopwatch.ElapsedMilliseconds} ms");

			// Ensure the result contains expected replacements
			Assert.IsTrue(result.Contains("Replacement1"));
			Assert.IsTrue(result.Contains("Replacement10000"));
		}
	}
}