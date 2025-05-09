#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 14:37:33Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Calcium.Reflection
{
	/// <summary>
	/// This class is intended to improve performance
	/// by caching types by their full names.
	/// </summary>
	class CachedTypeResolver
	{
		static readonly ConcurrentDictionary<string, Type> typeCache 
			= new ConcurrentDictionary<string, Type>();

		/// <summary>
		/// This method resolves a <c>Type</c> instance using Type.GetType. 
		/// Some type names in UWP are unresolvable when expressed with an assembly name. 
		/// Such as System.String, System.Private.CoreLib.
		/// If this method it is unable to resolve a type with an assembly name, it removes the assembly name.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		internal static Type GetType(string typeName)
		{
			Type result;

            if (typeCache.TryGetValue(typeName, out result))
			{
				return result;
			}

			result = Type.GetType(typeName, false);

			if (result == null)
			{
				string withoutAssemblyName = StripAssemblyVersion(typeName);

#if WINDOWS_UWP || NETFX_CORE
				withoutAssemblyName = withoutAssemblyName.Replace(", System.Private.CoreLib", string.Empty);
#endif
				result = Type.GetType(withoutAssemblyName, false);
			}

			if (result != null)
			{
				typeCache[typeName] = result;
			}

			return result;
		}

		static readonly Regex typeWithAssemblyRegex = new Regex(
			@"^(?<type>.*?)(?<assemblyDetails>, Version=[^\]]*?)(?<end>\].*$|\s*$)"/*, RegexOptions.Compiled (commented for AoT)*/);

		static string StripAssemblyVersion(string typeName)
		{
			Match match = typeWithAssemblyRegex.Match(typeName);
			string result = typeName;
			while (match.Success)
			{
				result = string.Format("{0}{1}",
					match.Groups["type"].Value,
					match.Groups["end"].Value);
				match = typeWithAssemblyRegex.Match(result);
			}

			return result;
		}
	}
}
