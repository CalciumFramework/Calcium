#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:23:03Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;

namespace Calcium.InversionOfControl.Containers
{
	/// <summary>
	/// Unit tests for <see cref="FrameworkContainer"/>.
	/// </summary>
	public class FrameworkContainerTests
	{
		readonly ContainerTests sharedTests = new ContainerTests();

		[DebuggerStepThrough]
		IContainer CreateContainer() => new FrameworkContainer();

		[Fact]
		public void RegisterAndResolveTypes()
		{
			sharedTests.RegisterAndResolveTypes(CreateContainer());
		}

		[Fact]
		public void RegisterAndResolveTypesWithKeys()
		{
			sharedTests.RegisterAndResolveTypesWithKeys(CreateContainer());
		}

		[Fact]
		public void RegisterAndResolveSingletons()
		{
			sharedTests.RegisterAndResolveSingletons(CreateContainer());
		}

		[Fact]
		public void RegisterAndResolveSingletonsWithKeys()
		{
			sharedTests.RegisterAndResolveSingletonsWithKeys(CreateContainer());
		}

		[Fact]
		public void RegisterAndResolveFuncs()
		{
			sharedTests.RegisterAndResolveFuncs(CreateContainer());
		}

		[Fact]
		public void DetectCircularDependence()
		{
			// Replaces ExpectedException attribute
			Assert.Throws<ResolutionException>(() => sharedTests.DetectCircularDependence(CreateContainer()));
		}

		[Fact]
		public void RegisterAndResolveFuncsWithKeys()
		{
			sharedTests.RegisterAndResolveFuncsWithKeys(CreateContainer());
		}

		[Fact]
		public void ReplaceTypes()
		{
			sharedTests.ReplaceTypes(CreateContainer());
		}

		[Fact]
		public void RegisterConcreteTypes()
		{
			sharedTests.RegisterConcreteTypes(CreateContainer());
		}

		[Fact]
		public void ShouldResolveDefaultTypes()
		{
			sharedTests.ShouldResolveDefaultTypes(CreateContainer());
		}
	}
}