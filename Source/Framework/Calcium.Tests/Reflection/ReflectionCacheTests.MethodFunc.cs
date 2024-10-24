#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-11 14:40:40Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.Reflection
{
	public partial class ReflectionCacheTests
	{
		static readonly IList<DelegateCreationMode> delegateCreationModes
			= EnumToListConverter.CreateEnumValueList<DelegateCreationMode>();

		[Fact]
		public void GetMethodFuncWithNoArgsAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);
				func.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				var funcResult = func(foo, []);

				foo.Method0ArgsCalled.Should().BeTrue(mode.ToString());
				funcResult.Should().Be(Foo.Method0ArgsResult, $"Func didn't return the expected value. {mode}");
			}
		}

		[Fact]
		public void GetMethodFuncGenericWithNoArgsAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker<string>(info, mode);
				func.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				var funcResult = func(foo, []);

				foo.Method0ArgsCalled.Should().BeTrue(mode.ToString());
				funcResult.Should().Be(Foo.Method0ArgsResult, $"Func didn't return the expected value. {mode}");
			}
		}

		[Fact]
		public void GetMethodFuncWithOneArgAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.Method1Arg));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);
				func.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "Foo";
				var funcResult = func(foo, [expectedValue1]);

				foo.Method1ArgCalled.Should().BeTrue(mode.ToString());
				foo.Arg1.Should().Be(expectedValue1, mode.ToString());
				funcResult.Should().Be(expectedValue1, $"Func didn't return the expected value. {mode}");
			}
		}

		[Fact]
		public void GetMethodFuncWithTwoArgsAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.Method2Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);
				func.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "One";
				const string expectedValue2 = "Two";
				var funcResult = func(foo, [expectedValue1, expectedValue2]);

				foo.Method2ArgsCalled.Should().BeTrue(mode.ToString());
				foo.Arg1.Should().Be(expectedValue1, mode.ToString());
				foo.Arg2.Should().Be(expectedValue2, mode.ToString());
				funcResult.Should().Be(expectedValue1, $"Func didn't return the expected value. {mode}");
			}
		}

		[Fact]
		public void GetMethodFuncWithThreeArgsAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.Method3Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);
				func.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "One";
				const string expectedValue2 = "Two";
				const string expectedValue3 = "Two";
				var funcResult = func(foo, [expectedValue1, expectedValue2, expectedValue3]);

				foo.Method3ArgsCalled.Should().BeTrue(mode.ToString());
				foo.Arg1.Should().Be(expectedValue1, mode.ToString());
				foo.Arg2.Should().Be(expectedValue2, mode.ToString());
				foo.Arg3.Should().Be(expectedValue3, mode.ToString());
				funcResult.Should().Be(expectedValue1, $"Func didn't return the expected value. {mode}");
			}
		}

		[Fact]
		public void FuncZeroArgsCached()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.Method0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);
				func.Should().NotBeNull(mode.ToString());
				var func2 = reflectionCache.GetMethodInvoker(info, mode);
				func2.Should().NotBeNull(mode.ToString());

				func.Should().Be(func2, mode.ToString());
			}
		}

		[Fact]
		public void FuncOneArgCached()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.Method1Arg));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var func = reflectionCache.GetMethodInvoker(info, mode);
				func.Should().NotBeNull(mode.ToString());
				var func2 = reflectionCache.GetMethodInvoker(info, mode);
				func2.Should().NotBeNull(mode.ToString());

				func.Should().Be(func2, mode.ToString());
			}
		}
	}
}
