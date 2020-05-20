#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-28 13:06:45Z</CreationDate>
</File>
*/
#endregion

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.Reflection
{
	[TestClass]
	public class AssemblyBuildTimeTests
	{
		[TestMethod]
		public void DisplayVersionShouldNotBeNull()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var version = assembly.GetName().Version;
			AssemblyBuildTime buildTime = new AssemblyBuildTime(version);

			var displayVersion = buildTime.GenerateDisplayVersion();
			Assert.IsNotNull(displayVersion);
		}
	}
}
