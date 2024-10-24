#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2020, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2020-01-02 23:25:23Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.InversionOfControl.Containers
{
	public class ContainerDefaultTypeTests
	{
		internal void ShouldResolveDefaultType(IContainer container)
		{
			var r1 = container.Resolve<IHaveDefaultType>();

			r1.Should().BeOfType<ClassForResolvingViaDefaultType>();
		}

		internal void ShouldNotResolveNonDefaultType(IContainer container)
		{
			// This would throw an exception as the type does not have a default resolution
			Assert.Throws<ResolutionException>(() => container.Resolve<IDontHaveDefaultType>());
		}

		[DefaultType(typeof(ClassForResolvingViaDefaultType))]
		interface IHaveDefaultType
		{
		}

		class ClassForResolvingViaDefaultType : IHaveDefaultType
		{
		}

		interface IDontHaveDefaultType
		{
		}
	}
}
