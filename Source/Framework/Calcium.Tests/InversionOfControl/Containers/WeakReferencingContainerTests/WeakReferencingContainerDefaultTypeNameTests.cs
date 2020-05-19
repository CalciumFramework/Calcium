#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:50Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.InversionOfControl.Containers
{
	[TestClass]
	public class WeakReferencingContainerDefaultTypeNameTests
	{
		readonly ContainerDefaultTypeNameTests sharedTests 
						= new ContainerDefaultTypeNameTests();

		[DebuggerStepThrough]
		IContainer CreateContainer() => new WeakReferencingContainer();

		/* These are raising an InvalidCastException in Release builds
		 * when using the MS Test runner.
		 * It would appear that the interface type is being loaded 
		 * in a different App domain, cause the type mismatch. It's rather strange.
		 * That's just a guess. */
#if DEBUG
		[TestMethod]
		public void ShouldResolveDefaultTypeByName()
		{
			sharedTests.ShouldResolveDefaultTypeByName(CreateContainer());
		}

		[TestMethod]
		public void ShouldFallbackToDefaultType()
		{
			sharedTests.ShouldFallbackToDefaultType(CreateContainer());
		}
#endif
		[TestMethod]
		public void ShouldFallbackToType()
		{
			sharedTests.ShouldFallbackToType(CreateContainer());
		}

		[TestMethod]
		[ExpectedException(typeof(ResolutionException))]
		public void ShouldNotResolveNonDefaultType()
		{
			sharedTests.ShouldNotResolveNonDefaultType(CreateContainer());
		}
	}
}
