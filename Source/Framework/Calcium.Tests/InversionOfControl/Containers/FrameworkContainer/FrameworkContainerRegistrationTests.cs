#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-07-16 18:08:34Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.InversionOfControl.Containers
{
	/// <summary>
	/// Unit tests for registration attributes used by
	/// <see cref="FrameworkContainer"/>.
	/// </summary>
	public class FrameworkContainerRegistrationTests
	{
		[Fact]
		public void ShouldReuseInstanceForSingletonRegistration()
		{
			FrameworkContainer container = new();

			SingletonDependency first
				= container.Resolve<SingletonDependency>();

			SingletonDependency second
				= container.Resolve<SingletonDependency>();

			Assert.Same(first, second);
		}

		[Fact]
		public void ShouldCreateNewInstanceForTransientRegistration()
		{
			FrameworkContainer container = new();

			TransientDependency first
				= container.Resolve<TransientDependency>();

			TransientDependency second
				= container.Resolve<TransientDependency>();

			Assert.NotSame(first, second);
		}

		[Fact]
		public void ShouldRegisterAttributedTransientTypeWhenFirstResolved()
		{
			FrameworkContainer container = new();

			Assert.False(
				container.IsRegistered<TransientDependency>());

			container.Resolve<TransientDependency>();

			Assert.True(
				container.IsRegistered<TransientDependency>());
		}

		[Fact]
		public void ShouldReuseSingletonWhenResolvedAsConstructorDependency()
		{
			FrameworkContainer container = new();

			SingletonDependency directDependency
				= container.Resolve<SingletonDependency>();

			DependencyConsumer firstConsumer
				= container.Resolve<DependencyConsumer>();

			DependencyConsumer secondConsumer
				= container.Resolve<DependencyConsumer>();

			Assert.NotSame(firstConsumer, secondConsumer);

			Assert.Same(
				directDependency,
				firstConsumer.Dependency);

			Assert.Same(
				firstConsumer.Dependency,
				secondConsumer.Dependency);
		}

		[Fact]
		public void ShouldPreferExplicitTransientRegistrationOverSingletonAttribute()
		{
			FrameworkContainer container = new();

			container.Register<SingletonDependency, SingletonDependency>(
				singleton: false);

			SingletonDependency first
				= container.Resolve<SingletonDependency>();

			SingletonDependency second
				= container.Resolve<SingletonDependency>();

			Assert.NotSame(first, second);
		}

		[Fact]
		public void ShouldPreferExplicitSingletonRegistrationOverTransientAttribute()
		{
			FrameworkContainer container = new();

			container.Register<TransientDependency, TransientDependency>(
				singleton: true);

			TransientDependency first
				= container.Resolve<TransientDependency>();

			TransientDependency second
				= container.Resolve<TransientDependency>();

			Assert.Same(first, second);
		}

		[Registration(Lifetime.Singleton)]
		sealed class SingletonDependency
		{
		}

		[Registration(Lifetime.Transient)]
		sealed class TransientDependency
		{
		}

		sealed class DependencyConsumer
		{
			public DependencyConsumer(SingletonDependency dependency)
			{
				Dependency = dependency;
			}

			public SingletonDependency Dependency { get; }
		}
	}
}