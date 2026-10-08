#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-10-08 18:37:02Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

using FluentAssertions;

using Xunit;

namespace Calcium.ResourcesModel
{
	/// <summary>
	/// <see cref="TagArgumentParser"/> tests.
	/// </summary>
	public class TagArgumentParserTests
	{
		[Fact]
		public void ShouldParseSingleArgument()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse("Provider=Google");

			result.Should().ContainSingle();
			result["Provider"].Should().Be("Google");
		}

		[Fact]
		public void ShouldParseMultipleArguments()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse("Provider=Google;PermissionSet=Calendar");

			result.Should().HaveCount(2);
			result["Provider"].Should().Be("Google");
			result["PermissionSet"].Should().Be("Calendar");
		}

		[Theory]
		[InlineData("")]
		[InlineData(";")]
		[InlineData(";;;")]
		[InlineData("   ")]
		[InlineData(" ; ; ")]
		public void ShouldReturnEmptyDictionaryWhenNoArguments(
			string arguments)
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse(arguments);

			result.Should().BeEmpty();
		}

		[Fact]
		public void ShouldIgnoreEmptyArguments()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse(
					";;Provider=Google;;;PermissionSet=Calendar;;");

			result.Should().HaveCount(2);
			result["Provider"].Should().Be("Google");
			result["PermissionSet"].Should().Be("Calendar");
		}

		[Fact]
		public void ShouldTrimArgumentNamesAndValues()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse(
					"  Provider  =  Google  ;  PermissionSet = Calendar  ");

			result.Should().HaveCount(2);
			result["Provider"].Should().Be("Google");
			result["PermissionSet"].Should().Be("Calendar");
		}

		[Fact]
		public void ShouldTreatArgumentNamesAsCaseInsensitive()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse("Provider=Google");

			result["provider"].Should().Be("Google");
			result["PROVIDER"].Should().Be("Google");
			result["PrOvIdEr"].Should().Be("Google");
		}

		[Fact]
		public void ShouldPreserveArgumentValueCase()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse("Provider=GoOgLe");

			result["Provider"].Should().Be("GoOgLe");
		}

		[Fact]
		public void ShouldPreserveEqualsSignsWithinValues()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse("Expression=One=Two=Three");

			result["Expression"].Should().Be("One=Two=Three");
		}

		[Fact]
		public void ShouldPreserveWhitespaceWithinValues()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse("Description=Some value with spaces");

			result["Description"].Should().Be("Some value with spaces");
		}

		[Fact]
		public void ShouldSupportCustomDelimiter()
		{
			TagArgumentParser parser = new(",");

			Dictionary<string, string> result
				= parser.Parse("Provider=Google,PermissionSet=Calendar");

			result.Should().HaveCount(2);
			result["Provider"].Should().Be("Google");
			result["PermissionSet"].Should().Be("Calendar");
		}

		[Fact]
		public void ShouldSupportMultipleDelimiters()
		{
			TagArgumentParser parser = new(";,|");

			Dictionary<string, string> result
				= parser.Parse(
					"Provider=Google;PermissionSet=Calendar,Scope=Read|Mode=Full");

			result.Should().HaveCount(4);
			result["Provider"].Should().Be("Google");
			result["PermissionSet"].Should().Be("Calendar");
			result["Scope"].Should().Be("Read");
			result["Mode"].Should().Be("Full");
		}

		[Fact]
		public void ShouldSupportRepeatedDelimiterCharacters()
		{
			TagArgumentParser parser = new(";;,,");

			Dictionary<string, string> result
				= parser.Parse("Provider=Google;,PermissionSet=Calendar");

			result.Should().HaveCount(2);
			result["Provider"].Should().Be("Google");
			result["PermissionSet"].Should().Be("Calendar");
		}

		[Fact]
		public void ShouldNotTreatDefaultDelimiterAsSpecialWhenOverridden()
		{
			TagArgumentParser parser = new(",");

			Dictionary<string, string> result
				= parser.Parse("Expression=One;Two,Provider=Google");

			result.Should().HaveCount(2);
			result["Expression"].Should().Be("One;Two");
			result["Provider"].Should().Be("Google");
		}

		[Fact]
		public void ShouldThrowWhenArgumentsAreNull()
		{
			TagArgumentParser parser = new();

			Action action = () => parser.Parse(null!);

			action.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("arguments");
		}

		[Theory]
		[InlineData("Provider")]
		[InlineData("ProviderGoogle")]
		[InlineData("=Google")]
		[InlineData("Provider=")]
		[InlineData("= ")]
		[InlineData(" =Google")]
		[InlineData("Provider= ")]
		[InlineData(" = ")]
		[InlineData("Provider=Google;Invalid")]
		[InlineData("Provider=Google;=Calendar")]
		[InlineData("Provider=Google;PermissionSet=")]
		public void ShouldThrowWhenArgumentIsMalformed(
			string arguments)
		{
			TagArgumentParser parser = new();

			Action action = () => parser.Parse(arguments);

			action.Should().Throw<FormatException>();
		}

		[Theory]
		[InlineData("Provider=Google;Provider=Microsoft")]
		[InlineData("Provider=Google;provider=Microsoft")]
		[InlineData("Provider=Google;PROVIDER=Microsoft")]
		[InlineData(" Provider = Google ; provider = Microsoft ")]
		public void ShouldThrowWhenArgumentNameIsDuplicated(
			string arguments)
		{
			TagArgumentParser parser = new();

			Action action = () => parser.Parse(arguments);

			action.Should()
				.Throw<FormatException>()
				.WithMessage("*specified more than once*");
		}

		[Fact]
		public void ShouldAllowSameValueForDifferentArgumentNames()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> result
				= parser.Parse("Provider=Google;Alternative=Google");

			result.Should().HaveCount(2);
			result["Provider"].Should().Be("Google");
			result["Alternative"].Should().Be("Google");
		}

		[Fact]
		public void ShouldReturnIndependentDictionariesForEachParse()
		{
			TagArgumentParser parser = new();

			Dictionary<string, string> firstResult
				= parser.Parse("Provider=Google");

			Dictionary<string, string> secondResult
				= parser.Parse("PermissionSet=Calendar");

			firstResult.Should().ContainKey("Provider");
			firstResult.Should().NotContainKey("PermissionSet");

			secondResult.Should().ContainKey("PermissionSet");
			secondResult.Should().NotContainKey("Provider");
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData("\t")]
		[InlineData("\r\n")]
		public void ShouldThrowWhenDelimiterIsNullOrWhitespace(
			string? delimiters)
		{
			Action action = () => new TagArgumentParser(delimiters!);

			action.Should()
				.Throw<ArgumentException>()
				.WithParameterName("argumentDelimiters");
		}

		[Theory]
		[InlineData("=")]
		[InlineData(";=")]
		[InlineData("=,")]
		[InlineData(";,=|")]
		public void ShouldThrowWhenDelimiterContainsEqualsSign(
			string delimiters)
		{
			Action action = () => new TagArgumentParser(delimiters);

			action.Should()
				.Throw<ArgumentException>()
				.WithParameterName("argumentDelimiters");
		}
	}
}