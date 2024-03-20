using System;
using System.Reflection;

namespace Calcium.Reflection
{
	/// <summary>
	/// Extension methods for <see cref="Assembly"/>.
	/// </summary>
	public static class AssemblyExtensions
	{
		/// <summary>
		/// Retrieves the <c>AssemblyBuildTime</c> for the specified assembly.
		/// <see cref="AssemblyBuildTime"/>
		/// Requires that the Assembly version must use a wildcard.
		/// For example: [assembly: AssemblyVersion(\"1.0.*\")]
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Occurs if the Assembly version was not specified using a wildcard.</exception>
		public static AssemblyBuildTime GetBuildTime(this Assembly assembly)
		{
			AssertArg.IsNotNull(assembly, nameof(assembly));

			return new AssemblyBuildTime(assembly.GetName().Version);
		}

		/// <summary>
		/// Retrieves the <c>AssemblyBuildTime</c> for the specified assembly version.
		/// <see cref="AssemblyBuildTime"/>
		/// Requires that the Assembly version must use a wildcard.
		/// For example: [assembly: AssemblyVersion(\"1.0.*\")]
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Occurs if the Assembly version was not specified using a wildcard.</exception>
		public static AssemblyBuildTime GetBuildTime(this Version assemblyVersion)
		{
			AssertArg.IsNotNull(assemblyVersion, nameof(assemblyVersion));

			return new AssemblyBuildTime(assemblyVersion);
        }

		/// <summary>
		/// Gets the DateTime object representing when the specified assembly was built.
		/// To enable add the following to your .csproj file:
        ///   &lt;ItemGroup&gt;
		///   &lt;AssemblyAttribute Include="Calcium.Reflection.TimeOfBuildAttribute"&gt;
		///     &lt;_Parameter1&gt;$([System.DateTime]::UtcNow.ToString("yyyyMMddHHmmss"))&lt;/_Parameter1&gt;
		///   &lt;/AssemblyAttribute&gt;
		/// &lt;/ItemGroup&gt;
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns>The DateTime object representing when the specified assembly was built,
		/// or <c>null</c> if the assembly was not build with the <see cref="TimeOfBuildAttribute"/> attribute.</returns>
		public static DateTime? GetTimeOfBuild(this Assembly assembly) 
		{
			AssertArg.IsNotNull(assembly, nameof(assembly));
			return TimeOfBuildAttribute.GetTimeOfBuild(assembly);
		}

	}
}
