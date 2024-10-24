#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:56Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;

namespace Calcium.InversionOfControl.Containers
{
	public class WeakReferencingContainerDefaultTypeTests
	{
		readonly ContainerDefaultTypeTests sharedTests = new ContainerDefaultTypeTests();

		[DebuggerStepThrough]
		IContainer CreateContainer() => new WeakReferencingContainer();

		[Fact]
		public void ShouldResolveDefaultType()
		{
			sharedTests.ShouldResolveDefaultType(CreateContainer());
		}

		[Fact]
		public void ShouldNotResolveNonDefaultType()
		{
			sharedTests.ShouldNotResolveNonDefaultType(CreateContainer());
		}
	}
}

