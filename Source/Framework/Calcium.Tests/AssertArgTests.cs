#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-12 22:37:51Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium
{
	public class AssertArgTests
	{
		#region IsNotNull
		[Fact]
		public void IsNotNullShouldRaiseExceptionIfNull()
		{
			object? foo = null;
			Action action = () => AssertArg.IsNotNull(foo, nameof(IsNotNullShouldRaiseExceptionIfNull));

			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void IsNotNullShouldNotRaiseExceptionIfNotNull()
		{
			object foo = new();
			object result = AssertArg.IsNotNull(foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsNotNullOrEmpty
		[Fact]
		public void IsNotNullOrEmptyShouldRaiseExceptionIfNull()
		{
			string? foo = null;
			Action action = () => AssertArg.IsNotNullOrEmpty(foo, nameof(foo));

			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void IsNotNullOrEmptyShouldRaiseExceptionIfEmpty()
		{
			string foo = string.Empty;
			Action action = () => AssertArg.IsNotNullOrEmpty(foo, nameof(foo));

			action.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void IsNotNullOrEmptyShouldNotRaiseExceptionIfNotNullOrEmpty()
		{
			string foo = "foo";
			string result = AssertArg.IsNotNullOrEmpty(foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsNotNullOrWhiteSpace
		[Fact]
		public void IsNotNullOrWhiteSpaceShouldRaiseExceptionIfNull()
		{
			string? foo = null;
			Action action = () => AssertArg.IsNotNullOrWhiteSpace(foo, nameof(foo));

			action.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void IsNotNullOrWhiteSpaceShouldRaiseExceptionIfWhiteSpace()
		{
			string foo = "  ";
			Action action = () => AssertArg.IsNotNullOrWhiteSpace(foo, nameof(foo));

			action.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void IsNotNullOrWhiteSpaceShouldNotRaiseExceptionIfNotNullOrWhiteSpace()
		{
			string foo = "foo";
			var result = AssertArg.IsNotNullOrWhiteSpace(foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsNotEmpty
		[Fact]
		public void IsNotEmptyShouldRaiseExceptionIfEmpty()
		{
			Guid foo = Guid.Empty;
			Action action = () => AssertArg.IsNotEmpty(foo, nameof(foo));

			action.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void IsNotEmptyShouldNotRaiseExceptionIfNotEmpty()
		{
			Guid foo = Guid.NewGuid();
			var result = AssertArg.IsNotEmpty(foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsGreaterThan Int
		[Fact]
		public void IsGreaterIntThanShouldRaiseExceptionIfLessThan()
		{
			Action action = () => AssertArg.IsGreaterThan(1, 0, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsGreaterIntThanShouldRaiseExceptionIfEqual()
		{
			Action action = () => AssertArg.IsGreaterThan(1, 1, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsGreaterIntThanShouldNotRaiseExceptionIfGreaterThan()
		{
			int foo = 5;
			int result = AssertArg.IsGreaterThan(1, foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsGreaterThan Double
		[Fact]
		public void IsGreaterDoubleThanShouldRaiseExceptionIfLessThan()
		{
			Action action = () => AssertArg.IsGreaterThan(1.0, 0.0, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsGreaterDoubleThanShouldRaiseExceptionIfEqual()
		{
			Action action = () => AssertArg.IsGreaterThan(1.0, 1.0, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsGreaterDoubleThanShouldNotRaiseExceptionIfGreaterThan()
		{
			int foo = 5;
			int result = AssertArg.IsGreaterThan(1, foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsGreaterThanOrEqual Int
		[Fact]
		public void IsGreaterThanOrEqualIntShouldRaiseExceptionIfLessThan()
		{
			Action action = () => AssertArg.IsGreaterThanOrEqual(1, 0, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsGreaterThanOrEqualIntShouldNotRaiseExceptionIfGreaterThan()
		{
			int foo = 5;
			int result = AssertArg.IsGreaterThanOrEqual(1, foo, nameof(foo));

			result.Should().Be(foo);
		}

		[Fact]
		public void IsGreaterThanOrEqualIntShouldNotRaiseExceptionIfEqual()
		{
			int foo = 5;
			int result = AssertArg.IsGreaterThanOrEqual(5, foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsGreaterThanOrEqual Double
		[Fact]
		public void IsGreaterThanOrEqualDoubleShouldRaiseExceptionIfLessThan()
		{
			Action action = () => AssertArg.IsGreaterThanOrEqual(1.0, 0.0, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsGreaterThanOrEqualDoubleShouldNotRaiseExceptionIfGreaterThan()
		{
			double foo = 5.0;
			double result = AssertArg.IsGreaterThanOrEqual(1.0, foo, nameof(foo));

			result.Should().Be(foo);
		}

		[Fact]
		public void IsGreaterThanOrEqualDoubleShouldNotRaiseExceptionIfEqual()
		{
			double foo = 5.0;
			double result = AssertArg.IsGreaterThanOrEqual(5.0, foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsLessThan Int
		[Fact]
		public void IsLessThanIntShouldRaiseExceptionIfNotLessThan()
		{
			Action action = () => AssertArg.IsLessThan(0, 1, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsLessThanIntShouldRaiseExceptionIfEqual()
		{
			Action action = () => AssertArg.IsLessThan(1, 1, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsLessThanIntShouldNotRaiseExceptionIfLessThan()
		{
			double foo = 5;
			double result = AssertArg.IsLessThan(6, foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsLessThan Double
		[Fact]
		public void IsLessThanDoubleShouldRaiseExceptionIfNotLessThan()
		{
			Action action = () => AssertArg.IsLessThan(0.0, 1.0, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsLessThanDoubleShouldRaiseExceptionIfEqual()
		{
			Action action = () => AssertArg.IsLessThan(1.0, 1.0, "None");

			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void IsLessThanDoubleShouldNotRaiseExceptionIfLessThan()
		{
			double foo = 5.0;
			double result = AssertArg.IsLessThan(6.0, foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsOfType
		[Fact]
		public void IsOfTypeShouldRaiseExceptionIfNotOfType()
		{
			object foo = new();
			Action action = () => AssertArg.IsOfType<string>(foo, nameof(foo));

			action.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void IsOfTypeShouldNotRaiseExceptionIfNotNull()
		{
			object foo = new();
			object result = AssertArg.IsOfType<object>(foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion

		#region IsNotNullAndOfType
		[Fact]
		public void IsNotNullAndOfTypeShouldRaiseExceptionIfNull()
		{
			object? foo = null;
			Action action = () => AssertArg.IsNotNullAndOfType<object>(foo, nameof(foo));

			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void IsNotNullAndOfTypeShouldRaiseExceptionIfNotOfType()
		{
			object foo = new();
			Action action = () => AssertArg.IsNotNullAndOfType<string>(foo, nameof(foo));

			action.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void IsNotNullAndOfTypeShouldNotRaiseExceptionIfNotNull()
		{
			object foo = new();
			object result = AssertArg.IsNotNullAndOfType<object>(foo, nameof(foo));

			result.Should().Be(foo);
		}
		#endregion
	}
}
