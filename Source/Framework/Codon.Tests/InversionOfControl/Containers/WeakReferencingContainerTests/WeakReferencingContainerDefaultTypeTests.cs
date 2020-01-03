#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:56Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.InversionOfControl.Containers
{
	[TestClass]
	public class WeakReferencingContainerDefaultTypeTests
	{
		readonly ContainerDefaultTypeTests sharedTests 
						= new ContainerDefaultTypeTests();

		[DebuggerStepThrough]
		IContainer CreateContainer() => new WeakReferencingContainer();

		[TestMethod]
		public void ShouldResolveDefaultType()
		{
			sharedTests.ShouldResolveDefaultType(CreateContainer());
		}

		[TestMethod]
		[ExpectedException(typeof(ResolutionException))]
		public void ShouldNotResolveNonDefaultType()
		{
			sharedTests.ShouldNotResolveNonDefaultType(CreateContainer());
		}
	}
}
