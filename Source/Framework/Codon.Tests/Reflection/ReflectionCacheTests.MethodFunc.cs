#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-11 14:40:40Z</CreationDate>
</File>
*/
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.Reflection
{
	public partial class ReflectionCacheTests
	{
		[TestMethod]
		public void GetMethodFuncWithNoArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));
			var func = reflectionCache.GetMethodInvoker(info);

			Assert.IsNotNull(func);

			var foo = new Foo();

			var funcResult = func(foo, new object[] { });
			
			Assert.IsTrue(foo.Method0ArgsCalled);
			Assert.AreEqual(Foo.Method0ArgsResult, funcResult, 
				"Func didn't return the expected value.");
		}

		[TestMethod]
		public void GetMethodFuncGenericWithNoArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));
			var func = reflectionCache.GetMethodInvoker<string>(info);

			Assert.IsNotNull(func);

			var foo = new Foo();

			var funcResult = func(foo, new object[] { });

			Assert.IsTrue(foo.Method0ArgsCalled);
			Assert.AreEqual(Foo.Method0ArgsResult, funcResult,
				"Func didn't return the expected value.");
		}

		[TestMethod]
		public void GetMethodFuncWithOneArgAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method1Arg));
			var func = reflectionCache.GetMethodInvoker(info);

			Assert.IsNotNull(func);

			var foo = new Foo();

			const string expectedValue1 = "Foo";
			var funcResult = func(foo, new object[] { expectedValue1 });

			Assert.IsTrue(foo.Method1ArgCalled);
			Assert.AreEqual(expectedValue1, foo.Arg1);

			Assert.AreEqual(expectedValue1, funcResult,
				"Func didn't return the expected value.");
		}

		[TestMethod]
		public void GetMethodFuncWithTwoArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method2Args));
			var func = reflectionCache.GetMethodInvoker(info);

			Assert.IsNotNull(func);

			var foo = new Foo();

			const string expectedValue1 = "One";
			const string expectedValue2 = "Two";

			var funcResult = func(foo, new object[]
			{
				expectedValue1, expectedValue2
			});

			Assert.IsTrue(foo.Method2ArgsCalled);
			Assert.AreEqual(expectedValue1, foo.Arg1);
			Assert.AreEqual(expectedValue2, foo.Arg2);

			Assert.AreEqual(expectedValue1, funcResult,
				"Func didn't return the expected value.");
		}

		[TestMethod]
		public void GetMethodFuncWithThreeArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method3Args));
			var func = reflectionCache.GetMethodInvoker(info);

			Assert.IsNotNull(func);

			var foo = new Foo();

			const string expectedValue1 = "One";
			const string expectedValue2 = "Two";
			const string expectedValue3 = "Two";

			var funcResult = func(foo, new object[]
			{
				expectedValue1, expectedValue2, expectedValue3
			});

			Assert.IsTrue(foo.Method3ArgsCalled);
			Assert.AreEqual(expectedValue1, foo.Arg1);
			Assert.AreEqual(expectedValue2, foo.Arg2);
			Assert.AreEqual(expectedValue3, foo.Arg3);

			Assert.AreEqual(expectedValue1, funcResult,
				"Func didn't return the expected value.");
		}

		[TestMethod]
		public void FuncZeroArgsCached()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));
			var func = reflectionCache.GetMethodInvoker(info);
			Assert.IsNotNull(func);
			var func2 = reflectionCache.GetMethodInvoker(info);
			Assert.IsNotNull(func2);

			Assert.AreEqual(func, func2);
		}

		[TestMethod]
		public void FuncOneArgCached()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method1Arg));
			var func = reflectionCache.GetMethodInvoker(info);
			Assert.IsNotNull(func);
			var func2 = reflectionCache.GetMethodInvoker(info);
			Assert.IsNotNull(func2);

			Assert.AreEqual(func, func2);
		}
	}
}
