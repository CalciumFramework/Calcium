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
using FluentAssertions;

namespace Calcium.Reflection
{
	public class AssemblyBuildTimeTests
	{
		//[Fact]
		//public void DisplayVersionShouldNotBeNull()
		//{
		//	/* The executing assembly does not use a wildcard for the version,
		//	   which is required by the AssemblyBuildTime class. 
		//	   Hence, we must use a different assembly. */

		//	throw new NotImplementedException(
		//		"This test is inconclusive. We need a wildcard project to test AssemblyBuildTime.");

		//	//var assembly = typeof(AssemblyBuildTime).Assembly;
		//	//var version = assembly.GetName().Version;
		//	//AssemblyBuildTime buildTime = new(version);

		//	//var displayVersion = buildTime.GenerateDisplayVersion();
		//	//displayVersion.Should().NotBeNull();
		//}
	}
}
