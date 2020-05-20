#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2020, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2020-01-02 23:25:23Z</CreationDate>
</File>
*/
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.InversionOfControl.Containers
{
	class ContainerDefaultTypeTests
	{
		internal void ShouldResolveDefaultType(IContainer container)
		{
			var r1 = container.Resolve<IHaveDefaultType>();

			Assert.IsInstanceOfType(r1, typeof(ClassForResolvingViaDefaultType));
		}

		internal void ShouldNotResolveNonDefaultType(IContainer container)
		{
			var r1 = container.Resolve<IDontHaveDefaultType>();
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
