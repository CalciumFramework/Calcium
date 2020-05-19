#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-12 22:37:51Z</CreationDate>
</File>
*/
#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium
{
	/// <summary>
	/// Test methods for the <see cref="AssertArg"/> class.
	/// </summary>
	[TestClass]
	public  class AssertArgTests
	{
		#region IsNotNull
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IsNotNullShouldRaiseExceptionIfNull()
		{
			object foo = null;
			AssertArg.IsNotNull(foo, nameof(IsNotNullShouldRaiseExceptionIfNull));
		}

		[TestMethod]
		public void IsNotNullShouldNotRaiseExceptionIfNotNull()
		{
			object foo = new object();
			var result = AssertArg.IsNotNull(foo, nameof(foo));
			
			Assert.AreEqual(foo, result);
		}

		#endregion

		#region IsNotNullOrEmpty
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IsNotNullOrEmptyShouldRaiseExceptionIfNull()
		{
			string foo = null;
			AssertArg.IsNotNullOrEmpty(foo, nameof(foo));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsNotNullOrEmptyShouldRaiseExceptionIfEmpty()
		{
			string foo = string.Empty;
			AssertArg.IsNotNullOrEmpty(foo, nameof(foo));
		}

		[TestMethod]
		public void IsNotNullOrEmptyShouldNotRaiseExceptionIfNotNullOrEmpty()
		{
			string foo = "foo";
			var result = AssertArg.IsNotNullOrEmpty(foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsNotNullOrWhiteSpace
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsNotNullOrWhiteSpaceShouldRaiseExceptionIfNull()
		{
			string foo = null;
			AssertArg.IsNotNullOrWhiteSpace(foo, nameof(foo));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsNotNullOrWhiteSpaceShouldRaiseExceptionIfWhiteSpace()
		{
			string foo = "  ";
			AssertArg.IsNotNullOrWhiteSpace(foo, nameof(foo));

			string foo2 = "";
			AssertArg.IsNotNullOrWhiteSpace(foo, nameof(foo2));
		}

		[TestMethod]
		public void IsNotNullOrWhiteSpaceShouldNotRaiseExceptionIfNotNullOrWhiteSpace()
		{
			string foo = "foo";
			var result = AssertArg.IsNotNullOrEmpty(foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsNotEmpty
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsNotEmptyShouldRaiseExceptionIfEmpty()
		{
			Guid foo = Guid.Empty;
			AssertArg.IsNotEmpty(foo, nameof(foo));
		}

		[TestMethod]
		public void IsNotEmptyShouldNotRaiseExceptionIfNotEmpty()
		{
			Guid foo = Guid.NewGuid();
			var result = AssertArg.IsNotEmpty(foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsGreaterThan Int
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsGreaterIntThanShouldRaiseExceptionIfLessThan()
		{
			AssertArg.IsGreaterThan(1, 0, "None");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsGreaterIntThanShouldRaiseExceptionIfEqual()
		{
			AssertArg.IsGreaterThan(1, 1, "None");
		}

		[TestMethod]
		public void IsGreaterIntThanShouldNotRaiseExceptionIfGreaterThan()
		{
			var foo = 5;
			var result = AssertArg.IsGreaterThan(1, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsGreaterThan Double
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsGreaterDoubleThanShouldRaiseExceptionIfLessThan()
		{
			AssertArg.IsGreaterThan(1.0, 0.0, "None");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsGreaterDoubleThanShouldRaiseExceptionIfEqual()
		{
			AssertArg.IsGreaterThan(1.0, 1.0, "None");
		}

		[TestMethod]
		public void IsGreaterDoubleThanShouldNotRaiseExceptionIfGreaterThan()
		{
			var foo = 5;
			var result = AssertArg.IsGreaterThan(1, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsGreaterThanOrEqual Int
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsGreaterThanOrEqualIntShouldRaiseExceptionIfLessThan()
		{
			AssertArg.IsGreaterThanOrEqual(1, 0, "None");
		}

		[TestMethod]
		public void IsGreaterThanOrEqualIntShouldNotRaiseExceptionIfGreaterThan()
		{
			var foo = 5;
			var result = AssertArg.IsGreaterThanOrEqual(1, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}

		[TestMethod]
		public void IsGreaterThanOrEqualIntShouldNotRaiseExceptionIfEqual()
		{
			var foo = 5;
			var result = AssertArg.IsGreaterThanOrEqual(5, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsGreaterThanOrEqual Double
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsGreaterThanOrEqualDoubleShouldRaiseExceptionIfLessThan()
		{
			AssertArg.IsGreaterThanOrEqual(1.0, 0.0, "None");
		}

		[TestMethod]
		public void IsGreaterThanOrEqualDoubleShouldNotRaiseExceptionIfGreaterThan()
		{
			var foo = 5.0;
			var result = AssertArg.IsGreaterThanOrEqual(1.0, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}

		[TestMethod]
		public void IsGreaterThanOrEqualDoubleShouldNotRaiseExceptionIfEqual()
		{
			var foo = 5.0;
			var result = AssertArg.IsGreaterThanOrEqual(5.0, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsLessThan Int
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsLessThanIntShouldRaiseExceptionIfNotLessThan()
		{
			AssertArg.IsLessThan(0, 1, "None");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsLessThanIntShouldRaiseExceptionIfEqual()
		{
			AssertArg.IsLessThan(1, 1, "None");
		}

		[TestMethod]
		public void IsLessThanIntShouldNotRaiseExceptionIfLessThan()
		{
			var foo = 5;
			var result = AssertArg.IsLessThan(6, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsLessThan Double
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsLessThanDoubleShouldRaiseExceptionIfNotLessThan()
		{
			AssertArg.IsLessThan(0.0, 1.0, "None");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsLessThanDoubleShouldRaiseExceptionIfEqual()
		{
			AssertArg.IsLessThan(1.0, 1.0, "None");
		}

		[TestMethod]
		public void IsLessThanDoubleShouldNotRaiseExceptionIfLessThan()
		{
			var foo = 5.0;
			var result = AssertArg.IsLessThan(6.0, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsLessThanOrEqual Int
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsLessThanOrEqualIntShouldRaiseExceptionIfNotLessThan()
		{
			AssertArg.IsLessThan(0, 1, "None");
		}

		[TestMethod]
		public void IsLessThanOrEqualIntShouldNotRaiseExceptionIfEqual()
		{
			var foo = 5;
			var result = AssertArg.IsLessThanOrEqual(5, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}

		[TestMethod]
		public void IsLessThanOrEqualIntShouldNotRaiseExceptionIfLessThanOrEqual()
		{
			var foo = 5;
			var result = AssertArg.IsLessThanOrEqual(6, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsLessThanOrEqual Double
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IsLessThanOrEqualDoubleShouldRaiseExceptionIfNotLessThan()
		{
			AssertArg.IsLessThan(0.0, 1.0, "None");
		}

		[TestMethod]
		public void IsLessThanOrEqualDoubleShouldNotRaiseExceptionIfEqual()
		{
			var foo = 5.0;
			var result = AssertArg.IsLessThanOrEqual(5.0, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}

		[TestMethod]
		public void IsLessThanOrEqualDoubleShouldNotRaiseExceptionIfLessThanOrEqual()
		{
			var foo = 5.0;
			var result = AssertArg.IsLessThanOrEqual(6.0, foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}
		#endregion

		#region IsOfType
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsOfTypeShouldRaiseExceptionIfNotOfType()
		{
			object foo = new object();
			AssertArg.IsOfType<string>(foo, nameof(foo));
		}

		[TestMethod]
		public void IsOfTypeShouldNotRaiseExceptionIfNotNull()
		{
			object foo = new object();
			var result = AssertArg.IsOfType<object>(foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}

		#endregion

		#region IsNotNullAndOfType
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IsNotNullAndOfTypeShouldRaiseExceptionIfNull()
		{
			object foo = null;
			AssertArg.IsNotNullAndOfType<object>(foo, nameof(foo));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsNotNullAndOfTypeShouldRaiseExceptionIfNotOfType()
		{
			object foo = new object();
			AssertArg.IsNotNullAndOfType<string>(foo, nameof(foo));
		}

		[TestMethod]
		public void IsNotNullAndOfTypeShouldNotRaiseExceptionIfNotNull()
		{
			object foo = new object();
			var result = AssertArg.IsNotNullAndOfType<object>(foo, nameof(foo));

			Assert.AreEqual(foo, result);
		}

		#endregion
	}
}
