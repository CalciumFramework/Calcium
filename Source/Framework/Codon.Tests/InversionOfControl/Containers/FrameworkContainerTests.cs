#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:23:03Z</CreationDate>
</File>
*/
#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.InversionOfControl.Containers
{
	/// <summary>
	/// Unit tests for FrameworkContainer
	/// </summary>
	[TestClass]
	public class FrameworkContainerTests
	{

		TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get => testContextInstance;
			set => testContextInstance = value;
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void RegisterAndResolveTypes()
		{
			var container = new FrameworkContainer();

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

		[TestMethod]
		public void RegisterAndResolveTypesWithKeys()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();

			frameworkContainer.Register<IClass2, Class2a>(true);
			var class2Instance1 = frameworkContainer.Resolve<IClass2>();

			frameworkContainer.Register<IClass2, Class2a>(true, "Class2a");
			var c2 = frameworkContainer.Resolve<IClass2>("Class2a");

			Assert.AreNotEqual(class2Instance1, c2);

			frameworkContainer.Register<IClass3>(new Class3a());
			var c3a = frameworkContainer.Resolve<IClass3>();
			var c3b = frameworkContainer.Resolve<IClass3>();
			Assert.AreEqual(c3a, c3b);
		}

		[TestMethod]
		public void RegisterAndResolveSingletons()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();

			frameworkContainer.Register<IClass3>(new Class3a());
			var c3a = frameworkContainer.Resolve<IClass3>();
			var c3b = frameworkContainer.Resolve<IClass3>();
			Assert.AreEqual(c3a, c3b);
		}

		[TestMethod]
		public void RegisterAndResolveSingletonsWithKeys()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();

			frameworkContainer.Register<IClass3>(new Class3a(), "Class3a");
			var c3a = frameworkContainer.Resolve<IClass3>("Class3a");
			var c3b = frameworkContainer.Resolve<IClass3>("Class3a");
			Assert.AreEqual(c3a, c3b);
		}

		[TestMethod]
		public void RegisterAndResolveFuncs()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();

			frameworkContainer.Register<IClass1>(() => new Class1a());
			{
				Assert.IsTrue(frameworkContainer.IsRegistered<IClass1>());
				Assert.IsTrue(frameworkContainer.IsRegistered(typeof(IClass1)));
				var c1a = frameworkContainer.Resolve<IClass1>();
				var c1b = frameworkContainer.Resolve<IClass1>();
				Assert.AreNotEqual(c1a, c1b);
			}

			frameworkContainer.Register<IClass2>(() => new Class2a(), true);

			{
				Assert.IsTrue(frameworkContainer.IsRegistered<IClass2>());
				Assert.IsTrue(frameworkContainer.IsRegistered(typeof(IClass2)));
				var c2a = frameworkContainer.Resolve<IClass2>();
				var c2b = frameworkContainer.Resolve<IClass2>();
				Assert.AreEqual(c2a, c2b);
			}

			frameworkContainer.Register<Class2a>(() => new Class2a(), true);

			{
				Assert.IsTrue(frameworkContainer.IsRegistered<Class2a>());
				Assert.IsTrue(frameworkContainer.IsRegistered(typeof(Class2a)));
				var c2a = frameworkContainer.Resolve<Class2a>();
				var c2b = frameworkContainer.Resolve<Class2a>();
				Assert.AreEqual(c2a, c2b);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ResolutionException), "Cycle detected.")]
		public void DetectCircularDependence()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();

			frameworkContainer.Register<CrossDependentA>(() => new CrossDependentA(frameworkContainer), true);
			frameworkContainer.Register<CrossDependentB>(() => new CrossDependentB(frameworkContainer), true);

			{
				Assert.IsTrue(frameworkContainer.IsRegistered<CrossDependentA>());
				Assert.IsTrue(frameworkContainer.IsRegistered(typeof(CrossDependentA)));
				Assert.IsTrue(frameworkContainer.IsRegistered<CrossDependentB>());
				Assert.IsTrue(frameworkContainer.IsRegistered(typeof(CrossDependentB)));
				var a = frameworkContainer.Resolve<CrossDependentA>();
			}
		}

		[TestMethod]
		public void RegisterAndResolveFuncsWithKeys()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();

			frameworkContainer.Register<IClass1>(() => new Class1a());

			{
				var c1 = frameworkContainer.Resolve<IClass1>();
				var c2 = frameworkContainer.Resolve<IClass1>();
				Assert.AreNotEqual(c1, c2);
			}

			frameworkContainer.Register<IClass3>(() => new Class3a(), true);

			{
				var c1 = frameworkContainer.Resolve<IClass3>();
				var c2 = frameworkContainer.Resolve<IClass3>();
				Assert.AreEqual(c1, c2);
			}

			frameworkContainer.Register<IClass3>(() => new Class3a(), true, "Class2a");

			{
				var types = frameworkContainer.ResolveAll<IClass3>();
				Assert.AreEqual(2, types.Count());
			}
		}

		[TestMethod]
		public void ReplaceTypes()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();

			frameworkContainer.Register<IClass1, Class1a>();

			{
				var c1 = frameworkContainer.Resolve<IClass1>();
				Assert.IsNotNull(c1);
				frameworkContainer.Register<IClass1, Class1b>();

				var c2 = frameworkContainer.Resolve<IClass1>();

				Assert.IsTrue(c2 is Class1b);
			}

			frameworkContainer.Register<IClass1, Class1a>(true);

			{
				var c1 = frameworkContainer.Resolve<IClass1>();
				Assert.IsNotNull(c1);
				frameworkContainer.Register<IClass1, Class1b>(true);

				var c2 = frameworkContainer.Resolve<IClass1>();

				Assert.IsTrue(c2 is Class1b);
			}

			IClass1 c4 = new Class1a();
			frameworkContainer.Register<IClass1>(c4);

			{
				var c1 = frameworkContainer.Resolve<IClass1>();
				Assert.AreEqual(c4, c1);
				frameworkContainer.Register<IClass1, Class1b>(true);

				var c2 = frameworkContainer.Resolve<IClass1>();

				Assert.IsTrue(c2 is Class1b);
			}

			{
				IClass1 c5 = new Class1a();
				frameworkContainer.Register<IClass1>(() => c5, false, "Class5");

				var c1 = frameworkContainer.Resolve<IClass1>("Class5");
				Assert.AreEqual(c5, c1);
				frameworkContainer.Register<IClass1, Class1b>(true);

				var c2 = frameworkContainer.Resolve<IClass1>();

				Assert.IsTrue(c2 is Class1b);
			}
		}

		[TestMethod]
		public void RegisterConcreteTypes()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();

			{
				frameworkContainer.Register<Class1a, Class1a>();

				Assert.IsTrue(frameworkContainer.IsRegistered<Class1a>());
				Assert.IsTrue(frameworkContainer.IsRegistered(typeof(Class1a)));

				var c1 = frameworkContainer.Resolve<Class1a>();
				var c2 = frameworkContainer.Resolve<Class1a>();
				Assert.AreNotEqual(c1, c2);
				frameworkContainer.Register<Class1b, Class1b>(false, "Class1b");
				Assert.IsFalse(frameworkContainer.IsRegistered<Class1b>());
				frameworkContainer.Register<Class1b, Class1b>(false);
				Assert.IsTrue(frameworkContainer.IsRegistered<Class1b>());
				Assert.IsTrue(frameworkContainer.IsRegistered<Class1b>("Class1b"));

				var class1s = frameworkContainer.ResolveAll<Class1b>();
				Assert.AreEqual(2, class1s.Count());
			}
		}

		[TestMethod]
		public void ShouldResolveDefaultTypes()
		{
			FrameworkContainer frameworkContainer = new FrameworkContainer();
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

			public CrossDependentA(FrameworkContainer frameworkContainer)
			{
				CrossDependentB = frameworkContainer.Resolve<CrossDependentB>();
			}
		}

		class CrossDependentB
		{
			public CrossDependentA CrossDependentA { get; }

			public CrossDependentB(FrameworkContainer frameworkContainer)
			{
				CrossDependentA = frameworkContainer.Resolve<CrossDependentA>();
			}
		}
	}
}
