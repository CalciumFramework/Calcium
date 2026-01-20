using Calcium.Networking.UrlParsing;

using FluentAssertions;

using Xunit;

namespace Calcium.Networking.UrlParsing
{
	public sealed class UriCreatorTests
	{
		readonly UriCreator creator = new();

		[Fact]
		public void TryCreate_WhenUrlIsNull_Throws()
		{
			Action act = () => creator.TryCreate(null!, out _);

			/* Adjust this to ThrowsAny<Exception>() if AssertArg throws a custom exception type. */
			act.Should().Throw<ArgumentNullException>();
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData("\t")]
		[InlineData("\r\n")]
		public void TryCreate_WhenUrlIsWhitespace_ReturnsFalseAndEmptyOrWhitespaceDiagnosticOnly(string url)
		{
			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();
			result.ParsedUri.Should().BeNull();

			result.Diagnostics.Should().ContainSingle(
				d =>
					d.Code     == UriCreationDiagnosticCode.EmptyOrWhitespace
					&& d.Index == null
					&& d.Message.Contains("empty or whitespace", StringComparison.OrdinalIgnoreCase));

			result.Diagnostics.Select(d => d.Code).Should().NotContain(UriCreationDiagnosticCode.DotNetParsingFailed);
		}

		[Fact]
		public void TryCreate_WhenUrlIsValid_ReturnsTrueWithParsedUriAndNoDiagnostics()
		{
			const string url = "https://example.com/path?x=1&y=2";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeTrue();
			result.ParsedUri.Should().NotBeNull();
			result.Diagnostics.Should().BeEmpty();
		}

		[Fact]
		public void TryCreate_WhenMissingScheme_ReturnsFalseWithMissingSchemeAndDotNetParsingFailed()
		{
			const string url = "example.com/path?x=1";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();
			result.ParsedUri.Should().BeNull();

			result.Diagnostics.Select(d => d.Code)
				  .Should().Contain(UriCreationDiagnosticCode.MissingScheme);

			result.Diagnostics.Select(d => d.Code)
				  .Should().Contain(UriCreationDiagnosticCode.DotNetParsingFailed);
		}

		[Fact]
		public void TryCreate_WhenDotNetCannotParse_ReturnsFalseAndIncludesDotNetParsingFailed()
		{
			/* Malformed scheme should fail Uri.TryCreate across runtimes. */
			const string url = "ht!tp://example.com/path";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();
			result.ParsedUri.Should().BeNull();

			result.Diagnostics.Select(d => d.Code)
				  .Should().Contain(UriCreationDiagnosticCode.DotNetParsingFailed);
		}

		[Fact]
		public void TryCreate_WhenUrlContainsMultipleWhitespace_ReportsAllWhitespacePositions()
		{
			/* Use a malformed scheme to guarantee .NET parsing fails, so diagnostics run. */
			const string url = "ht!tp://example.com/a b\tc";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			List<UriCreationDiagnostic> whitespaceDiagnostics
				= result.Diagnostics
						.Where(d => d.Code == UriCreationDiagnosticCode.UnescapedWhitespace)
						.OrderBy(d => d.Index)
						.ToList();

			whitespaceDiagnostics.Should().HaveCount(2);

			int spaceIndex = url.IndexOf(' ');
			int tabIndex = url.IndexOf('\t');

			whitespaceDiagnostics[0].Index.Should().Be(spaceIndex);
			whitespaceDiagnostics[1].Index.Should().Be(tabIndex);

			result.Diagnostics.Select(d => d.Code)
				  .Should().Contain(UriCreationDiagnosticCode.DotNetParsingFailed);
		}

		[Fact]
		public void TryCreate_WhenUrlContainsMultipleQuotes_ReportsAllQuotePositions()
		{
			const string url = "ht!tp://example.com/a\"b\"c";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			List<UriCreationDiagnostic> quoteDiagnostics
				= result.Diagnostics
						.Where(d => d.Code == UriCreationDiagnosticCode.UnescapedQuote)
						.OrderBy(d => d.Index)
						.ToList();

			quoteDiagnostics.Should().HaveCount(2);

			int[] expectedIndexes = new[]
			{
				url.IndexOf('"'),
				url.IndexOf('"', url.IndexOf('"') + 1)
			};

			quoteDiagnostics.Select(d => d.Index)
							.Should().BeEquivalentTo(expectedIndexes, options => options.WithStrictOrdering());
		}

		[Fact]
		public void TryCreate_WhenUrlContainsMultipleBadPercentEncodings_ReportsAllOfThem()
		{
			const string url = "ht!tp://example.com/a%2G/b%/c%2";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			List<UriCreationDiagnostic> percentDiagnostics
				= result.Diagnostics
						.Where(d => d.Code == UriCreationDiagnosticCode
										.BadPercentEncoding)
						.OrderBy(d => d.Index)
						.ToList();

			percentDiagnostics.Should().HaveCount(3);

			int first = url.IndexOf("%2G", StringComparison.Ordinal);
			int second = url.IndexOf("%/", StringComparison.Ordinal);
			int third = url.LastIndexOf("%2", StringComparison.Ordinal);

			percentDiagnostics.Select(d => d.Index).Should()
							  .BeEquivalentTo(new[] { first, second, third }, o => o.WithStrictOrdering());

			result.Diagnostics.Select(d => d.Code)
				  .Should().Contain(UriCreationDiagnosticCode.DotNetParsingFailed);
		}

		[Fact]
		public void TryCreate_WhenUrlContainsLegacyPercentUEncoding_ReportsBadPercentEncodingWithLegacyMessage()
		{
			const string url = "ht!tp://example.com/a%u00E9b";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			UriCreationDiagnostic? legacy = result.Diagnostics.SingleOrDefault(
				diagnostic =>
					diagnostic.Code == UriCreationDiagnosticCode.BadPercentEncoding
					&& diagnostic.Message.Contains("legacy %uXXXX", StringComparison.OrdinalIgnoreCase));

			legacy.Should().NotBeNull();
			legacy!.Index.Should().Be(url.IndexOf("%u00E9", StringComparison.Ordinal));
		}

		[Fact]
		public void TryCreate_WhenUrlEndsWithPercent_ReportsBadPercentEncodingAndDoesNotAlsoReportPossibleTruncation()
		{
			const string url = "ht!tp://example.com/a%";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			int index = url.LastIndexOf('%');

			result.Diagnostics.Should().Contain(
				diagnostic =>
					diagnostic.Code  == UriCreationDiagnosticCode.BadPercentEncoding 
					&& diagnostic.Index == index);

			/* Truncation hint is intentionally suppressed when a bad percent diagnostic exists at the same index. */
			result.Diagnostics.Select(d => d.Code).Should().NotContain(UriCreationDiagnosticCode.PossibleTruncation);
		}

		[Fact]
		public void
			TryCreate_WhenUrlEndsWithPercentAndSingleHexDigit_ReportsBadPercentEncodingAndDoesNotAlsoReportPossibleTruncation()
		{
			const string url = "ht!tp://example.com/a%F";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			int index = url.LastIndexOf('%');

			result.Diagnostics.Should().Contain(
				diagnostic =>
					diagnostic.Code  == UriCreationDiagnosticCode.BadPercentEncoding &&
					diagnostic.Index == index);

			result.Diagnostics.Select(d => d.Code).Should().NotContain(UriCreationDiagnosticCode.PossibleTruncation);
		}

		[Fact]
		public void TryCreate_WhenDotNetParsingFailsAndNoOtherIssuesExist_ReturnsGenericDotNetFailureMessage()
		{
			/* Invalid scheme that contains ://, no whitespace, no quotes, no percent. */
			const string url = "ht!tp://example.com";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			result.Diagnostics.Should().ContainSingle(
				d =>
					d.Code == UriCreationDiagnosticCode.DotNetParsingFailed
					&& d.Message.Contains("Check for invalid characters", StringComparison.OrdinalIgnoreCase));
		}

		[Fact]
		public void TryCreate_WhenDotNetParsingFailsAndOtherIssuesExist_ReturnsShortDotNetFailureMessage()
		{
			/* Includes whitespace so we get at least one other diagnostic. */
			const string url = "ht!tp://example.com/a b";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			result.Diagnostics.Select(d => d.Code).Should().Contain(
				UriCreationDiagnosticCode.UnescapedWhitespace);

			result.Diagnostics.Should().ContainSingle(
				diagnostic => diagnostic.Code == UriCreationDiagnosticCode.DotNetParsingFailed
							  && diagnostic.Message == "URI failed .NET parsing.");
		}

		[Fact]
		public void TryCreate_WhenParsingFails_DiagnosticsShouldNotContainDuplicatesForSameBadPercentPosition()
		{
			const string url = "ht!tp://example.com/a%2";

			bool success = creator.TryCreate(url, out UriCreationResult result);

			success.Should().BeFalse();

			int index = url.LastIndexOf('%');

			result.Diagnostics.Count(
					  diagnostic =>
						  diagnostic.Code  == UriCreationDiagnosticCode.BadPercentEncoding &&
						  diagnostic.Index == index)
				  .Should().Be(1);
		}
	}
}