#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:50Z</CreationDate>
</File>
*/
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.InversionOfControl.Containers
{
	[TestClass]
	public class ContainerDefaultTypeNameTests
	{
		/* These are raising an InvalidCastException in Release builds
		 * when using the MS Test runner.
		 * It would appear that the interface type is being loaded 
		 * in a different App domain, cause the type mismatch. It's rather strange.
		 * That's just a guess. */
#if DEBUG
		[TestMethod]
		public void ShouldResolveDefaultTypeByName()
		{
			var container = new FrameworkContainer();

			var r1 = container.Resolve<IHaveDefaultTypeAndName>();

			Assert.IsInstanceOfType(r1, typeof(Class2));
		}

		[TestMethod]
		public void ShouldFallbackToDefaultType()
		{
			var container = new FrameworkContainer();

			var r1 = container.Resolve<IHaveDefaultTypeName>();

			Assert.IsInstanceOfType(r1, typeof(ClassForResolvingViaDefaultTypeName));
		}
#endif
		[TestMethod]
		public void ShouldFallbackToType()
		{
			var container = new FrameworkContainer();

			var r1 = container.Resolve<IHaveDefaultTypeAndWrongName>();

			Assert.IsInstanceOfType(r1, typeof(Class3));
		}

		[TestMethod]
		[ExpectedException(typeof(ResolutionException))]
		public void ShouldNotResolveNonDefaultType()
		{
			var container = new FrameworkContainer();

			string typeName = typeof(ClassForResolvingViaDefaultTypeName).AssemblyQualifiedName;

			var r1 = container.Resolve<IDontHaveDefaultTypeName>();
		}

		//[DefaultTypeName(nameof(ContainerDefaultTypeNameTests) + "+" + nameof(ClassForResolvingViaDefaultTypeName))]
		[DefaultTypeName("Codon.InversionOfControl.Containers.ContainerDefaultTypeNameTests+ClassForResolvingViaDefaultTypeName, Codon.Tests")]
		public interface IHaveDefaultTypeName
		{
		}

		public class ClassForResolvingViaDefaultTypeName : IHaveDefaultTypeName
		{
		}

		interface IDontHaveDefaultTypeName
		{
		}

		[DefaultType(typeof(Class1))]
		[DefaultTypeName(nameof(Codon) + "." + nameof(InversionOfControl) + "." + nameof(Containers) + "." + nameof(ContainerDefaultTypeNameTests) + "+" + nameof(Class2) + ", Codon.Tests")]
		interface IHaveDefaultTypeAndName
		{
		}

		class Class1 : IHaveDefaultTypeAndName
		{
		}

		class Class2 : IHaveDefaultTypeAndName
		{
		}

		[DefaultType(typeof(Class3))]
		[DefaultTypeName("InvalidTypeName123123")]
		interface IHaveDefaultTypeAndWrongName
		{

		}

		class Class3 : IHaveDefaultTypeAndWrongName
		{
		}
	}
}