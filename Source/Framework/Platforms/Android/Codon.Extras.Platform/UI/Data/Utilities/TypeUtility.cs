#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Codon.UI.Data
{
	class TypeUtility
	{
		/// <summary>
		/// Retrieves a list of types deriving from the specified type, 
		/// and including the specified type.
		/// Based on code by Thomas Lebrun http://bit.ly/1OQsD8L
		/// </summary>
		/// <typeparam name="TDerivingFrom">The type to match.</typeparam>
		/// <returns>A list of types deriving from the specified type.</returns>
		internal static IEnumerable<Type> GetTypes<TDerivingFrom>()
		{
			IEnumerable<AssemblyName> assemblies = GetNonSystemAssemblies();

			List<Type> result = null;

			foreach (var customAssembly in assemblies)
			{
				var assembly = Assembly.Load(customAssembly);
				var assemblyTypes = assembly.GetTypes();

				var types = assemblyTypes.Where(t => typeof(TDerivingFrom).IsAssignableFrom(t)).ToList();
				foreach (var type in types)
				{
					if (!type.IsInterface && !type.IsAbstract)
					{
						if (result == null)
						{
							result = new List<Type>();
						}

						result.Add(type);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the set of non system assemblies.
		/// Based on code by Thomas Lebrun http://bit.ly/1OQsD8L
		/// </summary>
		/// <returns>The list of known assemblies that are not system assemblies 
		/// as determined by <seealso cref="IsSystemAssembly"/>.</returns>
		static IEnumerable<AssemblyName> GetNonSystemAssemblies()
		{
			var result = new List<AssemblyName>();
//			var currentDomain = AppDomain.CurrentDomain;
//			var assemblies = currentDomain.GetAssemblies();
//			foreach (Assembly assembly in assemblies)
//			{
//				var assemblyName = assembly.GetName();
//
//				if (!IsSystemAssembly(assemblyName.Name))
//				{
//					result.Add(assemblyName);
//				}
//			}
//
//			return result;

						StackFrame[] stackFrames = new StackTrace().GetFrames();
						if (stackFrames == null)
						{
							return result;
						}
			
						Assembly executingAssembly = Assembly.GetExecutingAssembly();
						string executingAssemblyName = executingAssembly.GetName().Name;
						var examinedAssemblies = new List<string>();
			
						foreach (var frame in stackFrames)
						{
							var declaringType = frame.GetMethod().DeclaringType;
							if (declaringType == null)
							{
								continue;
							}
			
							var assembly = declaringType.Assembly;
							var assemblyName = assembly.GetName();
			
							if (examinedAssemblies.Contains(assemblyName.FullName))
							{
								continue;
							}
			
							var currentAssemblyName = assemblyName;
			
							if (!IsSystemAssembly(currentAssemblyName.Name) 
								&& !result.Contains(currentAssemblyName))
							{
								result.Add(currentAssemblyName);
							}
			
							examinedAssemblies.Add(currentAssemblyName.FullName);
			
							AssemblyName[] referencedAssemblyNames = assembly.GetReferencedAssemblies();
			
							if (referencedAssemblyNames != null)
							{
								foreach (var referencedAssemblyName in referencedAssemblyNames)
								{
									if (referencedAssemblyName.Name == executingAssemblyName 
										|| examinedAssemblies.Contains(referencedAssemblyName.FullName))
									{
										continue;
									}
			
									if (!IsSystemAssembly(referencedAssemblyName.Name) 
										&& !result.Contains(referencedAssemblyName))
									{
										result.Add(referencedAssemblyName);
									}
			
									examinedAssemblies.Add(referencedAssemblyName.FullName);
								}
							}
						}
			
						return result;
		}

		static bool IsSystemAssembly(string assemblyName)
		{
			var result = assemblyName.StartsWith("System.");
			result |= string.Equals(assemblyName, "Mono.Android", StringComparison.InvariantCultureIgnoreCase);
			result |= string.Equals(assemblyName, "mscorlib", StringComparison.InvariantCultureIgnoreCase);
			result |= string.Equals(assemblyName, "System", StringComparison.InvariantCultureIgnoreCase);
			result |= string.Equals(assemblyName, "System.Core", StringComparison.InvariantCultureIgnoreCase);
			result |= string.Equals(assemblyName, "System.Xml", StringComparison.InvariantCultureIgnoreCase);
			result |= string.Equals(assemblyName, "System.Xml.Linq", StringComparison.InvariantCultureIgnoreCase);
			result |= string.Equals(assemblyName, "Microsoft.Practices.ServiceLocation", StringComparison.InvariantCultureIgnoreCase);
			result |= assemblyName.StartsWith("Xamarin.Android.Support", StringComparison.OrdinalIgnoreCase);

			return result;
		}
	}
}