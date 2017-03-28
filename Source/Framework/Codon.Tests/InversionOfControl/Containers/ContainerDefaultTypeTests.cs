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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.InversionOfControl.Containers
{
	[TestClass]
	public class ContainerDefaultTypeTests
	{
		[TestMethod]
		public void ShouldResolveDefaultType()
		{
			var container = new FrameworkContainer();

			var r1 = container.Resolve<IHaveDefaultType>();

			Assert.IsInstanceOfType(r1, typeof(ClassForResolvingViaDefaultType));
		}

		[TestMethod]
		[ExpectedException(typeof(ResolutionException))]
		public void ShouldNotResolveNonDefaultType()
		{
			var container = new FrameworkContainer();

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
