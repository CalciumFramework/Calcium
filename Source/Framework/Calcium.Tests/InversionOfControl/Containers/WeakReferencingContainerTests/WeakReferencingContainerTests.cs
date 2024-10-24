#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2020, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2020-01-02 23:42:37Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;

namespace Calcium.InversionOfControl.Containers
{
	/// <summary>
	/// Unit tests for <see cref="WeakReferencingContainer"/>.
	/// </summary>
	public class WeakReferencingContainerTests
	{
		readonly ContainerTests sharedTests = new();

		[DebuggerStepThrough]
		IContainer CreateContainer() => new WeakReferencingContainer();

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
