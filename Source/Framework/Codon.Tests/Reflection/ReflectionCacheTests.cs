#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 18:37:57Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.Reflection
{
	[TestClass]
	public partial class ReflectionCacheTests
	{
		[TestMethod]
		public void GetPropertyFuncGenericAndRetrieveValue()
		{
			var reflectionCache = new ReflectionCache();

			var propertyInfo = typeof(Foo).GetProperty(nameof(Foo.Property1));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetPropertyGetter<string>(propertyInfo, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "Foo";
				foo.Property1 = expectedValue1;

				var property1 = func(foo);

				Assert.AreEqual(expectedValue1, property1, mode.ToString());
			}
		}

		[TestMethod]
		public void GetPropertyFuncAndRetrieveValue()
		{
			var reflectionCache = new ReflectionCache();

			var propertyInfo = typeof(Foo).GetProperty(nameof(Foo.Property1));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetPropertyGetter(propertyInfo, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "Foo";
				foo.Property1 = expectedValue1;

				var property1 = func(foo);

				Assert.AreEqual(expectedValue1, property1, mode.ToString());
			}
		}

		[TestMethod]
		public void GetterFuncCached()
		{
			var reflectionCache = new ReflectionCache();

			var propertyInfo = typeof(Foo).GetProperty(nameof(Foo.Property1));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetPropertyGetter(propertyInfo, mode);
				Assert.IsNotNull(func, mode.ToString());
				var func2 = reflectionCache.GetPropertyGetter(propertyInfo, mode);
				Assert.IsNotNull(func2, mode.ToString());

				Assert.AreEqual(func, func2, mode.ToString());
			}
		}

		[TestMethod]
		public void GetterGenericFuncCached()
		{
			var reflectionCache = new ReflectionCache();

			var propertyInfo = typeof(Foo).GetProperty(nameof(Foo.Property1));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetPropertyGetter<string>(propertyInfo, mode);
				Assert.IsNotNull(func, mode.ToString());
				var func2 = reflectionCache.GetPropertyGetter<string>(propertyInfo, mode);
				Assert.IsNotNull(func2, mode.ToString());

				Assert.AreEqual(func, func2, mode.ToString());
			}
		}

		[TestMethod]
		public void SetterFuncCached()
		{
			var reflectionCache = new ReflectionCache();

			var propertyInfo = typeof(Foo).GetProperty(nameof(Foo.Property1));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetPropertySetter(propertyInfo, mode);
				Assert.IsNotNull(func, mode.ToString());
				var func2 = reflectionCache.GetPropertySetter(propertyInfo, mode);
				Assert.IsNotNull(func2, mode.ToString());

				Assert.AreEqual(func, func2, mode.ToString());
			}
		}

		[TestMethod]
		public void SetterFuncCachedGeneric()
		{
			var reflectionCache = new ReflectionCache();

			var propertyInfo = typeof(Foo).GetProperty(nameof(Foo.Property1));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetPropertySetter<string>(propertyInfo, mode);
				Assert.IsNotNull(func, mode.ToString());
				var func2 = reflectionCache.GetPropertySetter<string>(propertyInfo, mode);
				Assert.IsNotNull(func2, mode.ToString());

				Assert.AreEqual(func, func2, mode.ToString());
			}
		}

		[TestMethod]
		public void GetPropertyFuncAndSetValue()
		{
			var reflectionCache = new ReflectionCache();

			var propertyInfo = typeof(Foo).GetProperty(nameof(Foo.Property1));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetPropertySetter(propertyInfo, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "Foo";
				func(foo, expectedValue1);

				Assert.AreEqual(expectedValue1, foo.Property1, mode.ToString());
			}
		}

		[TestMethod]
		public void GetPropertyFuncGenericAndSetValue()
		{
			var reflectionCache = new ReflectionCache();

			var propertyInfo = typeof(Foo).GetProperty(nameof(Foo.Property1));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetPropertySetter<string>(propertyInfo, mode);

				Assert.IsNotNull(func, mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "Foo";
				func(foo, expectedValue1);

				Assert.AreEqual(expectedValue1, foo.Property1, mode.ToString());
			}
		}
		

		class Foo
		{
			public string Property1 { get; set; }

			internal bool MethodVoid0ArgsCalled;
			internal bool MethodVoid1ArgCalled;
			internal bool MethodVoid2ArgsCalled;
			internal bool MethodVoid3ArgsCalled;

			internal bool Method0ArgsCalled;
			internal bool Method1ArgCalled;
			internal bool Method2ArgsCalled;
			internal bool Method3ArgsCalled;

			internal string Arg1;
			internal string Arg2;
			internal string Arg3;

			public void MethodVoid0Args()
			{
				MethodVoid0ArgsCalled = true;
			}

			public void MethodVoid1Arg(string arg1)
			{
				MethodVoid1ArgCalled = true;
				Arg1 = arg1;
			}

			public void MethodVoid2Args(string arg1, string arg2)
			{
				MethodVoid2ArgsCalled = true;
				Arg1 = arg1;
				Arg2 = arg2;
			}

			public void MethodVoid3Args(string arg1, string arg2, string arg3)
			{
				MethodVoid3ArgsCalled = true;
				Arg1 = arg1;
				Arg2 = arg2;
				Arg3 = arg3;
			}

			public const string Method0ArgsResult = "Foo";

			public string Method0Args()
			{
				Method0ArgsCalled = true;

				return Method0ArgsResult;
			}

			public string Method1Arg(string arg1)
			{
				Method1ArgCalled = true;
				Arg1 = arg1;

				return arg1;
			}

			public string Method2Args(string arg1, string arg2)
			{
				Method2ArgsCalled = true;
				Arg1 = arg1;
				Arg2 = arg2;

				return arg1;
			}

			public string Method3Args(string arg1, string arg2, string arg3)
			{
				Method3ArgsCalled = true;
				Arg1 = arg1;
				Arg2 = arg2;
				Arg3 = arg3;

				return arg1;
			}
		}
	}
}
