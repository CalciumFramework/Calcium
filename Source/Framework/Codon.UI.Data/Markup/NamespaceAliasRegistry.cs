using System;
using System.Collections.Generic;

namespace Codon.UI.Data
{
	public interface INamespaceAliasRegistry
	{
		void RegisterNamespaceAlias(string xamlAlias, string @namespace, string assemblyName);
		bool TryResolveType(string alias, string typeShortName, out Type type);
	}

	public class NamespaceAliasRegistry : INamespaceAliasRegistry
	{
		readonly Dictionary<string, string> aliasDictionary 
			= new Dictionary<string, string>();

		readonly Dictionary<string, Type> typeCache
			= new Dictionary<string, Type>();

		public NamespaceAliasRegistry()
		{
			RegisterNamespaceAlias("x", "Codon.UI.Elements", "Codon.Platform");
		}

		public void RegisterNamespaceAlias(string xamlAlias, string @namespace, string assemblyName)
		{
			string typeFormat = @namespace + ".{0}, " + assemblyName;
			aliasDictionary[xamlAlias] = typeFormat;
		}

		public bool TryResolveType(string alias, string typeShortName, out Type type)
		{
			string cacheKey = alias + ":" + typeShortName;
			if (typeCache.TryGetValue(cacheKey, out type))
			{
				return true;
			}

			string format;
			if (!aliasDictionary.TryGetValue(alias, out format))
			{
				return false;
			}

			string qualifiedName = string.Format(format, typeShortName);
			type = Type.GetType(qualifiedName, false);

			if (type != null)
			{
				typeCache[cacheKey] = type;

				return true;
			}

			return false;
		}
	}
}