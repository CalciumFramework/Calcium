using System;
using System.Reflection;

namespace Calcium.Reflection
{
	/// <summary>
	/// Extension methods for <see cref="Assembly"/>.
	/// </summary>
	public static class AssemblyExensions
	{
		/// <summary>
		/// Retrieves the <c>AssemblyBuildTime</c> for the specified assembly.
		/// <see cref="AssemblyBuildTime"/>
		/// </summary>
		public static AssemblyBuildTime GetBuildTime(this Assembly assembly)
		{
			AssertArg.IsNotNull(assembly, nameof(assembly));

			return new AssemblyBuildTime(assembly.GetName().Version);
		}

		/// <summary>
		/// Retrieves the <c>AssemblyBuildTime</c> for the specified assembly version.
		/// <see cref="AssemblyBuildTime"/>
		/// </summary>
		public static AssemblyBuildTime GetBuildTime(this Version assemblyVersion)
		{
			AssertArg.IsNotNull(assemblyVersion, nameof(assemblyVersion));

			return new AssemblyBuildTime(assemblyVersion);
		}

	}
}
