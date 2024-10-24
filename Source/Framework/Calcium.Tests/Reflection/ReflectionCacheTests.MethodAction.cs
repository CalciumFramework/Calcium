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
		[Fact]
		public void GetMethodActionWithNoArgsAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);
				action.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				action(foo, []);

				foo.MethodVoid0ArgsCalled.Should().BeTrue(mode.ToString());
			}
		}

		[Fact]
		public void GetMethodActionWithOneArgAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid1Arg));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);
				action.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "Foo";
				action(foo, [expectedValue1]);

				foo.MethodVoid1ArgCalled.Should().BeTrue(mode.ToString());
				foo.Arg1.Should().Be(expectedValue1, mode.ToString());
			}
		}

		[Fact]
		public void GetMethodActionWithTwoArgsAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid2Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);
				action.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "One";
				const string expectedValue2 = "Two";
				action(foo, [expectedValue1, expectedValue2]);

				foo.MethodVoid2ArgsCalled.Should().BeTrue(mode.ToString());
				foo.Arg1.Should().Be(expectedValue1, mode.ToString());
				foo.Arg2.Should().Be(expectedValue2, mode.ToString());
			}
		}

		[Fact]
		public void GetMethodActionWithThreeArgsAndCallIt()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid3Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);
				action.Should().NotBeNull(mode.ToString());

				var foo = new Foo();
				const string expectedValue1 = "One";
				const string expectedValue2 = "Two";
				const string expectedValue3 = "Two";

				action(foo, [expectedValue1, expectedValue2, expectedValue3]);

				foo.MethodVoid3ArgsCalled.Should().BeTrue(mode.ToString());
				foo.Arg1.Should().Be(expectedValue1, mode.ToString());
				foo.Arg2.Should().Be(expectedValue2, mode.ToString());
				foo.Arg3.Should().Be(expectedValue3, mode.ToString());
			}
		}

		[Fact]
		public void ActionZeroArgsCached()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid0Args));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);
				action.Should().NotBeNull(mode.ToString());
				var func2 = reflectionCache.GetVoidMethodInvoker(info, mode);
				func2.Should().NotBeNull(mode.ToString());

				action.Should().Be(func2, mode.ToString());
			}
		}

		[Fact]
		public void ActionOneArgCached()
		{
			ReflectionCache reflectionCache = new();
			var info = typeof(Foo).GetMethod(nameof(Foo.MethodVoid1Arg));

			foreach (DelegateCreationMode mode in delegateCreationModes)
			{
				InvokeUsingMode(mode);
			}

			void InvokeUsingMode(DelegateCreationMode mode)
			{
				var action = reflectionCache.GetVoidMethodInvoker(info, mode);
				action.Should().NotBeNull(mode.ToString());
				var func2 = reflectionCache.GetVoidMethodInvoker(info, mode);
				func2.Should().NotBeNull(mode.ToString());

				action.Should().Be(func2, mode.ToString());
			}
		}
	}
}
