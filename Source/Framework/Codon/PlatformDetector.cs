#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 11:54:25Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Codon.Platform
{
	/// <summary>
	/// .NET Standard currently does not provide
	/// a straight forward way to determine the platform
	/// that it is running on. This class probes 
	/// for a well-known type in platform specific assemblies
	/// to determine the platform. This then allows
	/// the <c>FrameworkContainer</c> IoC container 
	/// to automatically locate platform specific types
	/// at run-time.
	/// </summary>
	class PlatformDetector
    {
		static readonly List<string> platformNames
			= new List<string>
			{
				"Android",
				"Ios",
				"Uwp",
				"Wpf"
			};

	    static string platformName;
	    static bool searchedForName;

	    public static string PlatformName
	    {
		    get
		    {
			    if (!searchedForName && platformName == null)
			    {
				    searchedForName = true;

					string assemblyRoot = AssemblyConstants.Namespace;
					string knownType = $"{assemblyRoot}.PlatformIdentifier, {assemblyRoot}.";

					/* Look for the type in platform specific assemblies. 
					 * If the type is found, that indicates the platform.*/
					foreach (var pair in platformNames)
				    {
						var type = Type.GetType(knownType + pair, false);
					    if (type != null)
					    {
						    platformName = pair;
							break;
					    }
					}
			    }

			    return platformName;
		    }
	    }
    }
}
