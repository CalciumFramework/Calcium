#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2020, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2020-01-02 23:25:23Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;
// ReSharper disable InconsistentNaming

namespace Calcium.InversionOfControl.Containers
{
	public class ContainerTests
	{
		internal void RegisterAndResolveTypes(IContainer container)
		{
			container.Register<IClass1, Class1a>();

			container.IsRegistered<IClass1>().Should().BeTrue();
			container.IsRegistered(typeof(IClass1)).Should().BeTrue();

			container.Register<IClass1, Class1b>(false, "Class1b");
			container.IsRegistered<IClass1>().Should().BeTrue();
			container.IsRegistered<IClass1>("Class1b").Should().BeTrue();

			var class1s = container.ResolveAll<IClass1>();
			class1s.Count().Should().Be(2);

			container.Register<IClass2, Class2a>(true);
			container.IsRegistered<IClass2>().Should().BeTrue();

			var class2Instance1 = container.Resolve<IClass2>();
			var class2Instance2 = container.Resolve<IClass2>();
			class2Instance1.Should().Be(class2Instance2);
		}

		internal void RegisterAndResolveTypesWithKeys(IContainer container)
		{
			container.Register<IClass2, Class2a>(true);
			var class2Instance1 = container.Resolve<IClass2>();

			container.Register<IClass2, Class2a>(true, "Class2a");
			var c2 = container.Resolve<IClass2>("Class2a");

			class2Instance1.Should().NotBe(c2);

			container.Register<IClass3>(new Class3a());
			var c3a = container.Resolve<IClass3>();
			var c3b = container.Resolve<IClass3>();
			c3a.Should().Be(c3b);
		}

		internal void RegisterAndResolveSingletons(IContainer container)
		{
			container.Register<IClass3>(new Class3a());
			var c3a = container.Resolve<IClass3>();
			var c3b = container.Resolve<IClass3>();
			c3a.Should().Be(c3b);
		}

		internal void RegisterAndResolveSingletonsWithKeys(IContainer container)
		{
			container.Register<IClass3>(new Class3a(), "Class3a");
			var c3a = container.Resolve<IClass3>("Class3a");
			var c3b = container.Resolve<IClass3>("Class3a");
			c3a.Should().Be(c3b);
		}

		internal void RegisterAndResolveFuncs(IContainer container)
		{
			container.Register<IClass1>(() => new Class1a());

			{
				container.IsRegistered<IClass1>().Should().BeTrue();
				container.IsRegistered(typeof(IClass1)).Should().BeTrue();
				var c1a = container.Resolve<IClass1>();
				var c1b = container.Resolve<IClass1>();
				c1a.Should().NotBe(c1b);
			}

			container.Register<IClass2>(() => new Class2a(), true);

			{
				container.IsRegistered<IClass2>().Should().BeTrue();
				container.IsRegistered(typeof(IClass2)).Should().BeTrue();
				var c2a = container.Resolve<IClass2>();
				var c2b = container.Resolve<IClass2>();
				c2a.Should().Be(c2b);
			}

			container.Register<Class2a>(() => new Class2a(), true);

			{
				container.IsRegistered<Class2a>().Should().BeTrue();
				container.IsRegistered(typeof(Class2a)).Should().BeTrue();
				var c2a = container.Resolve<Class2a>();
				var c2b = container.Resolve<Class2a>();
				c2a.Should().Be(c2b);
			}
		}

		internal void DetectCircularDependence(IContainer container)
		{
			container.Register<CrossDependentA>(() => new CrossDependentA(container), true);
			container.Register<CrossDependentB>(() => new CrossDependentB(container), true);

			{
				container.IsRegistered<CrossDependentA>().Should().BeTrue();
				container.IsRegistered(typeof(CrossDependentA)).Should().BeTrue();
				container.IsRegistered<CrossDependentB>().Should().BeTrue();
				container.IsRegistered(typeof(CrossDependentB)).Should().BeTrue();
				var a = container.Resolve<CrossDependentA>();
			}
		}

		internal void RegisterAndResolveFuncsWithKeys(IContainer container)
		{
			container.Register<IClass1>(() => new Class1a());

			{
				var c1 = container.Resolve<IClass1>();
				var c2 = container.Resolve<IClass1>();
				c1.Should().NotBe(c2);
			}

			container.Register<IClass3>(() => new Class3a(), true);

			{
				var c1 = container.Resolve<IClass3>();
				var c2 = container.Resolve<IClass3>();
				c1.Should().Be(c2);
			}

			container.Register<IClass3>(() => new Class3a(), true, "Class2a");

			{
				var types = container.ResolveAll<IClass3>();
				types.Count().Should().Be(2);
			}
		}

		internal void ReplaceTypes(IContainer container)
		{
			container.Register<IClass1, Class1a>();

			{
				var c1 = container.Resolve<IClass1>();
				c1.Should().NotBeNull();
				container.Register<IClass1, Class1b>();

				var c2 = container.Resolve<IClass1>();

				c2.Should().BeOfType<Class1b>();
			}

			container.Register<IClass1, Class1a>(true);

			{
				var c1 = container.Resolve<IClass1>();
				c1.Should().NotBeNull();
				container.Register<IClass1, Class1b>(true);

				var c2 = container.Resolve<IClass1>();

				c2.Should().BeOfType<Class1b>();
			}

			IClass1 c4 = new Class1a();
			container.Register<IClass1>(c4);

			{
				var c1 = container.Resolve<IClass1>();
				c1.Should().Be(c4);
				container.Register<IClass1, Class1b>(true);

				var c2 = container.Resolve<IClass1>();

				c2.Should().BeOfType<Class1b>();
			}

			{
				IClass1 c5 = new Class1a();
				container.Register<IClass1>(() => c5, false, "Class5");

				var c1 = container.Resolve<IClass1>("Class5");
				c1.Should().Be(c5);
				container.Register<IClass1, Class1b>(true);

				var c2 = container.Resolve<IClass1>();

				c2.Should().BeOfType<Class1b>();
			}
		}

		internal void RegisterConcreteTypes(IContainer container)
		{
			container.Register<Class1a, Class1a>();

			container.IsRegistered<Class1a>().Should().BeTrue();
			container.IsRegistered(typeof(Class1a)).Should().BeTrue();

			var c1 = container.Resolve<Class1a>();
			var c2 = container.Resolve<Class1a>();
			c1.Should().NotBe(c2);
			container.Register<Class1b, Class1b>(false, "Class1b");
			container.IsRegistered<Class1b>().Should().BeFalse();
			container.Register<Class1b, Class1b>(false);
			container.IsRegistered<Class1b>().Should().BeTrue();
			container.IsRegistered<Class1b>("Class1b").Should().BeTrue();

			var class1s = container.ResolveAll<Class1b>();
			class1s.Count().Should().Be(2);
		}

		internal void ShouldResolveDefaultTypes(IContainer container)
		{
			// Implementation or call to resolve default types would go here
		}

		class Class1a : IClass1
		{
		}

		class Class1b : IClass1
		{
		}

		interface IClass1
		{
		}

		interface IClass2
		{
		}

		class Class2a : IClass2
		{
		}

		class Class2b : IClass2
		{
		}

		interface IClass3
		{
		}

		class Class3a : IClass3
		{
		}

		class Class3b : IClass3
		{
		}

		class CrossDependentA
		{
			public CrossDependentB CrossDependentB { get; }

			public CrossDependentA(IContainer container)
			{
				CrossDependentB = container.Resolve<CrossDependentB>();
			}
		}

		class CrossDependentB
		{
			public CrossDependentA CrossDependentA { get; }

			public CrossDependentB(IContainer container)
			{
				CrossDependentA = container.Resolve<CrossDependentA>();
			}
		}
	}
}
