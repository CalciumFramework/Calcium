#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2020, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2020-01-02 23:24:07Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.InversionOfControl.Containers
{
	public class ContainerDefaultTypeNameTests
	{
		/* These are raising an InvalidCastException in Release builds
         * when using the MS Test runner.
         * It would appear that the interface type is being loaded 
         * in a different App domain, causing the type mismatch. It's rather strange.
         * That's just a guess. */
#if DEBUG
		internal void ShouldResolveDefaultTypeByName(IContainer container)
		{
			var r1 = container.Resolve<IHaveDefaultTypeAndName>();

			r1.Should().BeOfType<Class2>();
		}

		internal void ShouldFallbackToDefaultType(IContainer container)
		{
			var r1 = container.Resolve<IHaveDefaultTypeName>();

			r1.Should().BeOfType<ClassForResolvingViaDefaultTypeName>();
		}
#endif
		internal void ShouldFallbackToType(IContainer container)
		{
			var r1 = container.Resolve<IHaveDefaultTypeAndWrongName>();

			r1.Should().BeOfType<Class3>();
		}

		internal void ShouldNotResolveNonDefaultType(IContainer container)
		{
			//string? typeName = typeof(ClassForResolvingViaDefaultTypeName).AssemblyQualifiedName;

			Assert.Throws<ResolutionException>(() => container.Resolve<IDontHaveDefaultTypeName>());
		}

		//[DefaultTypeName(nameof(ContainerDefaultTypeNameTests) + "+" + nameof(ClassForResolvingViaDefaultTypeName))]
		[DefaultTypeName("Calcium.InversionOfControl.Containers.ContainerDefaultTypeNameTests+ClassForResolvingViaDefaultTypeName, Calcium.Tests")]
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
		[DefaultTypeName(nameof(Calcium) + "." + nameof(InversionOfControl) + "." + nameof(Containers) + "." + nameof(ContainerDefaultTypeNameTests) + "+" + nameof(Class2) + ", Calcium.Tests")]
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
