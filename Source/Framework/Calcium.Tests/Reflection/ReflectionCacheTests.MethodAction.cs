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
		public void GetMethodActionWithNoArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);

				Assert.IsNotNull(action, mode.ToString());

				var foo = new Foo();

				action(foo, new object[] { });

				Assert.IsTrue(foo.MethodVoid0ArgsCalled, mode.ToString());
			}
		}

		[TestMethod]
		public void GetMethodActionWithOneArgAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid1Arg));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);

				Assert.IsNotNull(action, mode.ToString());

				var foo = new Foo();

				const string expectedValue1 = "Foo";
				action(foo, new object[] {expectedValue1});

				Assert.IsTrue(foo.MethodVoid1ArgCalled, mode.ToString());
				Assert.AreEqual(expectedValue1, foo.Arg1, mode.ToString());
			}
		}

		[TestMethod]
		public void GetMethodActionWithTwoArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid2Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);

				Assert.IsNotNull(action, mode.ToString());

				var foo = new Foo();

				const string expectedValue1 = "One";
				const string expectedValue2 = "Two";

				action(foo, new object[] {expectedValue1, expectedValue2});

				Assert.IsTrue(foo.MethodVoid2ArgsCalled, mode.ToString());
				Assert.AreEqual(expectedValue1, foo.Arg1, mode.ToString());
				Assert.AreEqual(expectedValue2, foo.Arg2, mode.ToString());
			}
		}

		[TestMethod]
		public void GetMethodActionWithThreeArgsAndCallIt()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid3Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);

				Assert.IsNotNull(action);

				var foo = new Foo();

				const string expectedValue1 = "One";
				const string expectedValue2 = "Two";
				const string expectedValue3 = "Two";

				action(foo, new object[]
				{
					expectedValue1, expectedValue2, expectedValue3
				});

				Assert.IsTrue(foo.MethodVoid3ArgsCalled, mode.ToString());
				Assert.AreEqual(expectedValue1, foo.Arg1, mode.ToString());
				Assert.AreEqual(expectedValue2, foo.Arg2, mode.ToString());
				Assert.AreEqual(expectedValue3, foo.Arg3, mode.ToString());
			}
		}

		[TestMethod]
		public void ActionZeroArgsCached()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);
				Assert.IsNotNull(action, mode.ToString());
				var func2 = reflectionCache.GetVoidMethodInvoker(info, mode);
				Assert.IsNotNull(func2, mode.ToString());

				Assert.AreEqual(action, func2, mode.ToString());
			}
		}

		[TestMethod]
		public void ActionOneArgCached()
		{
			var reflectionCache = new ReflectionCache();

			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid1Arg));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);
				Assert.IsNotNull(action, mode.ToString());
				var func2 = reflectionCache.GetVoidMethodInvoker(info, mode);
				Assert.IsNotNull(func2, mode.ToString());

				Assert.AreEqual(action, func2, mode.ToString());
			}
		}
	}
}
