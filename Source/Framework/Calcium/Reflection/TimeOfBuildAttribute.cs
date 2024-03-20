#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-03-20 16:19:22Z</CreationDate>
</File>
*/
#endregion

using System.Globalization;
using System;
using System.Reflection;

namespace Calcium.Reflection
{
    /// <summary>
    /// This attribute is used to add a time stamp to an assembly at build time.
    /// To enable add the following to your .csproj file:
    ///   &lt;ItemGroup&gt;
    ///   &lt;AssemblyAttribute Include="Calcium.Reflection.TimeOfBuildAttribute"&gt;
    ///     &lt;_Parameter1&gt;$([System.DateTime]::UtcNow.ToString("yyyyMMddHHmmss"))&lt;/_Parameter1&gt;
    ///   &lt;/AssemblyAttribute&gt;
    /// &lt;/ItemGroup&gt;
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TimeOfBuildAttribute : Attribute
    {
        public TimeOfBuildAttribute(string value)
        {
            DateTime = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public DateTime DateTime { get; }

        /// <summary>
        /// Gets the DateTime object representing when the specified assembly was built.
        /// To enable add the following to your .csproj file:
        ///   &lt;ItemGroup&gt;
        ///   &lt;AssemblyAttribute Include="Calcium.Reflection.TimeOfBuildAttribute"&gt;
        ///     &lt;_Parameter1&gt;$([System.DateTime]::UtcNow.ToString("yyyyMMddHHmmss"))&lt;/_Parameter1&gt;
        ///   &lt;/AssemblyAttribute&gt;
        /// &lt;/ItemGroup&gt;
        /// </summary>
        /// <param name="assembly">The assembly that has been decorated with the <see cref="TimeOfBuildAttribute"/>.</param>
        /// <returns>The DateTime object representing when the specified assembly was built,
        /// or <c>null</c> if the assembly was not build with the <see cref="TimeOfBuildAttribute"/> attribute.</returns>
        public static DateTime? GetTimeOfBuild(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<TimeOfBuildAttribute>();
            return attribute?.DateTime;
        }
    }
}
