#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:50Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;

namespace Calcium.InversionOfControl.Containers
{
	public class FrameworkContainerDefaultTypeNameTests
	{
		readonly ContainerDefaultTypeNameTests sharedTests = new();

		[DebuggerStepThrough]
		IContainer CreateContainer() => new FrameworkContainer();

		/* These are raising an InvalidCastException in Release builds
		 * when using the MS Test runner.
		 * It would appear that the interface type is being loaded
		 * in a different App domain, causing the type mismatch. It's rather strange.
		 * That's just a guess. */
#if DEBUG
		[Fact]
		public void ShouldResolveDefaultTypeByName()
		{
			sharedTests.ShouldResolveDefaultTypeByName(CreateContainer());
		}

		[Fact]
		public void ShouldFallbackToDefaultType()
		{
			sharedTests.ShouldFallbackToDefaultType(CreateContainer());
		}
#endif

		[Fact]
		public void ShouldFallbackToType()
		{
			sharedTests.ShouldFallbackToType(CreateContainer());
		}

		[Fact]
		public void ShouldNotResolveNonDefaultType()
		{
			// This replaces the ExpectedException attribute
			Assert.Throws<ResolutionException>(() => sharedTests.ShouldNotResolveNonDefaultType(CreateContainer()));
		}
	}
}
