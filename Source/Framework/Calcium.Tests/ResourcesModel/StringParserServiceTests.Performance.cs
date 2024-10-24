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

using System.Text;
using System.Diagnostics;

using FluentAssertions;

using Xunit.Abstractions;

namespace Calcium.ResourcesModel
{
	public partial class StringParserServiceTests
	{
		readonly ITestOutputHelper testOutputHelper;

		public StringParserServiceTests(ITestOutputHelper testOutputHelper)
		{
			this.testOutputHelper = testOutputHelper 
									?? throw new ArgumentNullException(nameof(testOutputHelper));
		}

		[Fact]
		public void ParsePerformanceTest_LargeInput()
		{
			StringParserService service = new();

			// Generate a large input string
			StringBuilder largeInputBuilder = new();
			var substitutions = new Dictionary<string, string>();

			for (int i = 1; i <= 10000; i++)
			{
				largeInputBuilder.Append($"<%=Tag{i}%> ");
				substitutions[$"Tag{i}"] = $"Replacement{i}";
			}

			string largeInput = largeInputBuilder.ToString();
			var customDelimiters = new TagDelimiters("<%=", "%>");

			// Measure the performance
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var result = service.Parse(largeInput, substitutions, customDelimiters);

			stopwatch.Stop();

			// Log execution time
			testOutputHelper.WriteLine($"Performance Test Execution Time: {stopwatch.ElapsedMilliseconds} ms");

			// Ensure the result contains expected replacements
			result.Should().Contain("Replacement1");
			result.Should().Contain("Replacement10000");
		}
	}
}
