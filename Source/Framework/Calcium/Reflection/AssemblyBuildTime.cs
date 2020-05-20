#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2015-04-04 18:51:45Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.Reflection
{
	/// <summary>
	/// This class allows you to retrieve a version number based 
	/// on the build time of a specified assembly.
	/// </summary>
	public class AssemblyBuildTime
	{
		/// <summary>
		/// Initializes a new <c>AssemblyBuildTime</c> object 
		/// using the specified version.
		/// Assembly version must be defined using a wildcard. 
		/// E.g. [assembly: AssemblyVersion(\"1.0.*\")]
		/// The build time is able to be determined because 
		/// the algorithm for generating a version from a wildcard 
		/// is known.
		/// </summary>
		/// <param name="assemblyVersion">
		/// The assembly version for which to retrieve the build time.</param>
		/// <example>
		/// var assembly = Assembly.GetExecutingAssembly();
		/// var buildTime = new AssemblyBuildTime(assembly.GetName().Version);
		/// </example>
		/// <exception cref="ArgumentException">
		/// Is thrown if the <c>Build</c> and <c>Revision</c>
		/// values of the <paramref name="assemblyVersion"/> are both 0;
		/// indicating that the assembly does not use a wildcard pattern.
		/// </exception>
		public AssemblyBuildTime(Version assemblyVersion)
		{
			AssertArg.IsNotNull(assemblyVersion, nameof(assemblyVersion));

			var buildNumber = assemblyVersion.Build;
			var revisionNumber = assemblyVersion.Revision;

			if (buildNumber == 0 && revisionNumber == 0)
			{
				throw new ArgumentException(
					"Assembly version must use a wildcard. E.g. [assembly: AssemblyVersion(\"1.0.*\")]");
			}

			AssemblyVersion = assemblyVersion;
			var startDate = new DateTime(2000, 01, 01);
			TimeOfBuild = startDate.AddDays(buildNumber).AddSeconds(revisionNumber * 2);

			var difference = TimeOfBuild - startDate;

			VersionIdentifier = (long)difference.TotalSeconds;
		}

		internal Version AssemblyVersion { get; }

		public string AssemblyFileVersion => AssemblyVersion.ToString();

		/// <summary>
		/// Creates a version number with the embedded date 
		/// and time of build. 
		/// For example "1.0.03281433", which reflects the format
		/// to {MajorVersion}.{MinorVersion}.{Month}{Day}{Hour}{Minute},
		/// and corresponds to 28 March 2.33pm.
		/// </summary>
		/// <param name="includeYearInDisplayVersion">
		/// If <c>true</c> includes a two digit year value 
		/// in the <c>DisplayVersion</c>, which changes the format to:
		/// {MajorVersion}.{MinorVersion}.{Year}{Month}{Day}{Hour}{Minute}</param>
		/// <returns></returns>
		public string GenerateDisplayVersion(bool includeYearInDisplayVersion = false)
		{
			var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
				TimeSpan.TicksPerDay * AssemblyVersion.Build + // days since 1 January 2000
				TimeSpan.TicksPerSecond * 2 * AssemblyVersion.Revision)); // seconds since midnight, (multiply by 2 to get original)

			string year = includeYearInDisplayVersion ?
				buildDateTime.Year.ToString().Substring(2)
				: string.Empty;

			int month = buildDateTime.Month;
			int day = buildDateTime.Day;
			int hour = buildDateTime.Hour;
			int minute = buildDateTime.Minute;

			var result = $"{AssemblyVersion.Major}.{AssemblyVersion.Minor}.{year}{month:D2}{day:D2}{hour:D2}{minute:D2}"; //version.Build + "." + version.Revision;

			return result;
		}

		public DateTime TimeOfBuild { get; }

		public long VersionIdentifier { get; }
	}

}
