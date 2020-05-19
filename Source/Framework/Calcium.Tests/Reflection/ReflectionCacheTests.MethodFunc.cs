#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-11 14:40:40Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.Reflection
{
	public partial class ReflectionCacheTests
	{
		static readonly IList<DelegateCreationMode> delegateCreationModes 
			= EnumToListConverter.CreateEnumValueList<DelegateCreationMode>();

		[TestMethod]
		public void GetMethodFuncWithNoArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();

				var funcResult = func(foo, new object[] { });
			
				Assert.IsTrue(foo.Method0ArgsCalled, mode.ToString());
				Assert.AreEqual(Foo.Method0ArgsResult, funcResult, 
					"Func didn't return the expected value." + mode);
			}
		}

		[TestMethod]
		public void GetMethodFuncGenericWithNoArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker<string>(info, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();

				var funcResult = func(foo, new object[] { });

				Assert.IsTrue(foo.Method0ArgsCalled, mode.ToString());
				Assert.AreEqual(Foo.Method0ArgsResult, funcResult,
					"Func didn't return the expected value. " + mode);
			}
		}

		[TestMethod]
		public void GetMethodFuncWithOneArgAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method1Arg));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();

				const string expectedValue1 = "Foo";
				var funcResult = func(foo, new object[] {expectedValue1});

				Assert.IsTrue(foo.Method1ArgCalled, mode.ToString());
				Assert.AreEqual(expectedValue1, foo.Arg1, mode.ToString());

				Assert.AreEqual(expectedValue1, funcResult,
					"Func didn't return the expected value. " + mode);
			}
		}

		[TestMethod]
		public void GetMethodFuncWithTwoArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method2Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();

				const string expectedValue1 = "One";
				const string expectedValue2 = "Two";

				var funcResult = func(foo, new object[]
				{
					expectedValue1, expectedValue2
				});

				Assert.IsTrue(foo.Method2ArgsCalled, mode.ToString());
				Assert.AreEqual(expectedValue1, foo.Arg1, mode.ToString());
				Assert.AreEqual(expectedValue2, foo.Arg2, mode.ToString());

				Assert.AreEqual(expectedValue1, funcResult,
					"Func didn't return the expected value. " + mode);
			}
		}

		[TestMethod]
		public void GetMethodFuncWithThreeArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method3Args));
			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();

				const string expectedValue1 = "One";
				const string expectedValue2 = "Two";
				const string expectedValue3 = "Two";

				var funcResult = func(foo, new object[]
				{
					expectedValue1, expectedValue2, expectedValue3
				});

				Assert.IsTrue(foo.Method3ArgsCalled, mode.ToString());
				Assert.AreEqual(expectedValue1, foo.Arg1, mode.ToString());
				Assert.AreEqual(expectedValue2, foo.Arg2, mode.ToString());
				Assert.AreEqual(expectedValue3, foo.Arg3, mode.ToString());

				Assert.AreEqual(expectedValue1, funcResult,
					"Func didn't return the expected value." + mode);
			}
		}

		[TestMethod]
		public void FuncZeroArgsCached()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);
				Assert.IsNotNull(func, mode.ToString());
				var func2 = reflectionCache.GetMethodInvoker(info, mode);
				Assert.IsNotNull(func2, mode.ToString());

				Assert.AreEqual(func, func2, mode.ToString());
			}
		}

		[TestMethod]
		public void FuncOneArgCached()
		{
			var reflectionCache = new ReflectionCache();
			var info = typeof(Foo).GetMethod(nameof(Foo.Method1Arg));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);
				Assert.IsNotNull(func, mode.ToString());
				var func2 = reflectionCache.GetMethodInvoker(info, mode);
				Assert.IsNotNull(func2, mode.ToString());

				Assert.AreEqual(func, func2, mode.ToString());
			}
		}
	}
}
