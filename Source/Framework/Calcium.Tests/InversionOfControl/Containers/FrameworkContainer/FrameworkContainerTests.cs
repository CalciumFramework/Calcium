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

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.InversionOfControl.Containers
{
	/// <summary>
	/// Unit tests for <see cref="FrameworkContainer"/>.
	/// </summary>
	[TestClass]
	public class FrameworkContainerTests
	{
		readonly ContainerTests sharedTests = new ContainerTests();

		[DebuggerStepThrough]
		IContainer CreateContainer() => new FrameworkContainer();

		[TestMethod]
		public void RegisterAndResolveTypes()
		{
			sharedTests.RegisterAndResolveTypes(CreateContainer());
		}

		[TestMethod]
		public void RegisterAndResolveTypesWithKeys()
		{
			sharedTests.RegisterAndResolveTypesWithKeys(CreateContainer());
		}

		[TestMethod]
		public void RegisterAndResolveSingletons()
		{
			sharedTests.RegisterAndResolveSingletons(CreateContainer());
		}

		[TestMethod]
		public void RegisterAndResolveSingletonsWithKeys()
		{
			sharedTests.RegisterAndResolveSingletonsWithKeys(CreateContainer());
		}

		[TestMethod]
		public void RegisterAndResolveFuncs()
		{
			sharedTests.RegisterAndResolveFuncs(CreateContainer());
		}

		[TestMethod]
		[ExpectedException(typeof(ResolutionException), "Cycle detected.")]
		public void DetectCircularDependence()
		{
			sharedTests.DetectCircularDependence(CreateContainer());
		}

		[TestMethod]
		public void RegisterAndResolveFuncsWithKeys()
		{
			sharedTests.RegisterAndResolveFuncsWithKeys(CreateContainer());
		}

		[TestMethod]
		public void ReplaceTypes()
		{
			sharedTests.ReplaceTypes(CreateContainer());
		}

		[TestMethod]
		public void RegisterConcreteTypes()
		{
			sharedTests.RegisterConcreteTypes(CreateContainer());
		}

		[TestMethod]
		public void ShouldResolveDefaultTypes()
		{
			sharedTests.ShouldResolveDefaultTypes(CreateContainer());
		}
	}
}
