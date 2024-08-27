#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-08-27 16:55:39Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Calcium.InversionOfControl
{
	[DefaultType(typeof(NamedTypeResolver))]
	public interface INamedTypeResolver
	{
		bool TryResolveType(string typeName, out Type type);
	}

	public class NamedTypeResolver : INamedTypeResolver
	{
		readonly ConcurrentDictionary<string, Type> types = new();
		readonly ConcurrentDictionary<string, bool> assembliesToSkip = new();

		public bool TryResolveType(string typeName, out Type type)
		{
			if (types.TryGetValue(typeName, out type))
			{
				return type != null;
			}

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				string assemblyName = assembly.FullName;

				if (assembliesToSkip.ContainsKey(assembly.FullName))
				{
					continue;
				}

				if (ShouldIgnoreAssembly(assemblyName))
				{
					assembliesToSkip[assemblyName] = true;
					continue;
				}

				foreach (Type t in assembly.GetTypes())
				{
					var fullName = t.FullName;
					if (!string.IsNullOrWhiteSpace(fullName))
					{
						types[fullName] = t;
						if (fullName == typeName)
						{
							type = t;
							return true;
						}
					}
				}

				assembliesToSkip[assemblyName] = true;
			}

			types[typeName] = null;
			return false;
		}

		SortedSet<string> ignoredAssemblyPrefixes 
			= new(StringComparer.OrdinalIgnoreCase)
			  {
				  "System.", "Interop", "netstandard,", "Microsoft.", "Anonymously Hosted", "JetBrains.", "xunit", "ReSharper"
			  };

		bool ShouldIgnoreAssembly(string assemblyName)
		{
			foreach (string ignoredAssemblyPrefix in ignoredAssemblyPrefixes)
			{
				if (assemblyName.StartsWith(ignoredAssemblyPrefix, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}
	}
}
