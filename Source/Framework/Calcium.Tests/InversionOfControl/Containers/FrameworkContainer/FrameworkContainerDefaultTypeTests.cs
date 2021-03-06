#region File and License Information
/*
<File>
	<License>
		Copyright � 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:56Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.InversionOfControl.Containers
{
	[TestClass]
	public class FrameworkContainerDefaultTypeTests
	{
		readonly ContainerDefaultTypeTests sharedTests 
						= new ContainerDefaultTypeTests();

		[DebuggerStepThrough]
		IContainer CreateContainer() => new FrameworkContainer();

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
