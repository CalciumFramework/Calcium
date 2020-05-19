using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.InversionOfControl.Containers
{
	class ContainerTests
	{
		internal void RegisterAndResolveTypes(IContainer container)
		{
			container.Register<IClass1, Class1a>();

			Assert.IsTrue(container.IsRegistered<IClass1>());
			Assert.IsTrue(container.IsRegistered(typeof(IClass1)));

			container.Register<IClass1, Class1b>(false, "Class1b");
			Assert.IsTrue(container.IsRegistered<IClass1>());
			Assert.IsTrue(container.IsRegistered<IClass1>("Class1b"));

			var class1s = container.ResolveAll<IClass1>();
			Assert.AreEqual(2, class1s.Count());

			container.Register<IClass2, Class2a>(true);
			Assert.IsTrue(container.IsRegistered<IClass2>());

			var class2Instance1 = container.Resolve<IClass2>();
			var class2Instance2 = container.Resolve<IClass2>();
			Assert.AreEqual(class2Instance1, class2Instance2);
		}

		internal void RegisterAndResolveTypesWithKeys(IContainer container)
		{
			container.Register<IClass2, Class2a>(true);
			var class2Instance1 = container.Resolve<IClass2>();

			container.Register<IClass2, Class2a>(true, "Class2a");
			var c2 = container.Resolve<IClass2>("Class2a");

			Assert.AreNotEqual(class2Instance1, c2);

			container.Register<IClass3>(new Class3a());
			var c3a = container.Resolve<IClass3>();
			var c3b = container.Resolve<IClass3>();
			Assert.AreEqual(c3a, c3b);
		}

		internal void RegisterAndResolveSingletons(IContainer container)
		{
			container.Register<IClass3>(new Class3a());
			var c3a = container.Resolve<IClass3>();
			var c3b = container.Resolve<IClass3>();
			Assert.AreEqual(c3a, c3b);
		}

		internal void RegisterAndResolveSingletonsWithKeys(IContainer container)
		{
			container.Register<IClass3>(new Class3a(), "Class3a");
			var c3a = container.Resolve<IClass3>("Class3a");
			var c3b = container.Resolve<IClass3>("Class3a");
			Assert.AreEqual(c3a, c3b);
		}

		internal void RegisterAndResolveFuncs(IContainer container)
		{
			container.Register<IClass1>(() => new Class1a());

			{
				Assert.IsTrue(container.IsRegistered<IClass1>());
				Assert.IsTrue(container.IsRegistered(typeof(IClass1)));
				var c1a = container.Resolve<IClass1>();
				var c1b = container.Resolve<IClass1>();
				Assert.AreNotEqual(c1a, c1b);
			}

			container.Register<IClass2>(() => new Class2a(), true);

			{
				Assert.IsTrue(container.IsRegistered<IClass2>());
				Assert.IsTrue(container.IsRegistered(typeof(IClass2)));
				var c2a = container.Resolve<IClass2>();
				var c2b = container.Resolve<IClass2>();
				Assert.AreEqual(c2a, c2b);
			}

			container.Register<Class2a>(() => new Class2a(), true);

			{
				Assert.IsTrue(container.IsRegistered<Class2a>());
				Assert.IsTrue(container.IsRegistered(typeof(Class2a)));
				var c2a = container.Resolve<Class2a>();
				var c2b = container.Resolve<Class2a>();
				Assert.AreEqual(c2a, c2b);
			}
		}

		internal void DetectCircularDependence(IContainer container)
		{
			container.Register<CrossDependentA>(() => new CrossDependentA(container), true);
			container.Register<CrossDependentB>(() => new CrossDependentB(container), true);

			{
				Assert.IsTrue(container.IsRegistered<CrossDependentA>());
				Assert.IsTrue(container.IsRegistered(typeof(CrossDependentA)));
				Assert.IsTrue(container.IsRegistered<CrossDependentB>());
				Assert.IsTrue(container.IsRegistered(typeof(CrossDependentB)));
				var a = container.Resolve<CrossDependentA>();
			}
		}

		internal void RegisterAndResolveFuncsWithKeys(IContainer container)
		{
			container.Register<IClass1>(() => new Class1a());

			{
				var c1 = container.Resolve<IClass1>();
				var c2 = container.Resolve<IClass1>();
				Assert.AreNotEqual(c1, c2);
			}

			container.Register<IClass3>(() => new Class3a(), true);

			{
				var c1 = container.Resolve<IClass3>();
				var c2 = container.Resolve<IClass3>();
				Assert.AreEqual(c1, c2);
			}

			container.Register<IClass3>(() => new Class3a(), true, "Class2a");

			{
				var types = container.ResolveAll<IClass3>();
				Assert.AreEqual(2, types.Count());
			}
		}

		internal void ReplaceTypes(IContainer container)
		{
			container.Register<IClass1, Class1a>();

			{
				var c1 = container.Resolve<IClass1>();
				Assert.IsNotNull(c1);
				container.Register<IClass1, Class1b>();

				var c2 = container.Resolve<IClass1>();

				Assert.IsTrue(c2 is Class1b);
			}

			container.Register<IClass1, Class1a>(true);

			{
				var c1 = container.Resolve<IClass1>();
				Assert.IsNotNull(c1);
				container.Register<IClass1, Class1b>(true);

				var c2 = container.Resolve<IClass1>();

				Assert.IsTrue(c2 is Class1b);
			}

			IClass1 c4 = new Class1a();
			container.Register<IClass1>(c4);

			{
				var c1 = container.Resolve<IClass1>();
				Assert.AreEqual(c4, c1);
				container.Register<IClass1, Class1b>(true);

				var c2 = container.Resolve<IClass1>();

				Assert.IsTrue(c2 is Class1b);
			}

			{
				IClass1 c5 = new Class1a();
				container.Register<IClass1>(() => c5, false, "Class5");

				var c1 = container.Resolve<IClass1>("Class5");
				Assert.AreEqual(c5, c1);
				container.Register<IClass1, Class1b>(true);

				var c2 = container.Resolve<IClass1>();

				Assert.IsTrue(c2 is Class1b);
			}
		}

		internal void RegisterConcreteTypes(IContainer container)
		{
			container.Register<Class1a, Class1a>();

			Assert.IsTrue(container.IsRegistered<Class1a>());
			Assert.IsTrue(container.IsRegistered(typeof(Class1a)));

			var c1 = container.Resolve<Class1a>();
			var c2 = container.Resolve<Class1a>();
			Assert.AreNotEqual(c1, c2);
			container.Register<Class1b, Class1b>(false, "Class1b");
			Assert.IsFalse(container.IsRegistered<Class1b>());
			container.Register<Class1b, Class1b>(false);
			Assert.IsTrue(container.IsRegistered<Class1b>());
			Assert.IsTrue(container.IsRegistered<Class1b>("Class1b"));

			var class1s = container.ResolveAll<Class1b>();
			Assert.AreEqual(2, class1s.Count());
		}

		internal void ShouldResolveDefaultTypes(IContainer container)
		{
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
