#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-07-16 18:22:17Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.InversionOfControl.Containers
{
	public class FrameworkContainerGenericRegistrationTests
	{
		[Fact]
		public void ShouldReuseSingletonForClosedGenericType()
		{
			FrameworkContainer container = new();

			GenericSingleton<FirstContext> first
				= container.Resolve<GenericSingleton<FirstContext>>();

			GenericSingleton<FirstContext> second
				= container.Resolve<GenericSingleton<FirstContext>>();

			Assert.Same(first, second);
		}

		[Fact]
		public void ShouldCreateSeparateSingletonForEachClosedGenericType()
		{
			FrameworkContainer container = new();

			GenericSingleton<FirstContext> first
				= container.Resolve<GenericSingleton<FirstContext>>();

			GenericSingleton<SecondContext> second
				= container.Resolve<GenericSingleton<SecondContext>>();

			Assert.NotSame(first, second);
		}

		[Fact]
		public void ShouldRegisterOnlyResolvedClosedGenericType()
		{
			FrameworkContainer container = new();

			container.Resolve<GenericSingleton<FirstContext>>();

			Assert.True(
				container.IsRegistered<GenericSingleton<FirstContext>>());

			Assert.False(
				container.IsRegistered<GenericSingleton<SecondContext>>());
		}

		[Fact]
		public void ShouldPreferExplicitRegistrationForClosedGenericType()
		{
			FrameworkContainer container = new();

			container.Register<
				GenericSingleton<FirstContext>,
				GenericSingleton<FirstContext>>(singleton: false);

			GenericSingleton<FirstContext> first
				= container.Resolve<GenericSingleton<FirstContext>>();

			GenericSingleton<FirstContext> second
				= container.Resolve<GenericSingleton<FirstContext>>();

			GenericSingleton<SecondContext> otherFirst
				= container.Resolve<GenericSingleton<SecondContext>>();

			GenericSingleton<SecondContext> otherSecond
				= container.Resolve<GenericSingleton<SecondContext>>();

			Assert.NotSame(first, second);
			Assert.Same(otherFirst, otherSecond);
		}

		[Registration(Lifetime.Singleton)]
		sealed class GenericSingleton<TContext>
		{
		}

		sealed class FirstContext
		{
		}

		sealed class SecondContext
		{
		}
	}
}
