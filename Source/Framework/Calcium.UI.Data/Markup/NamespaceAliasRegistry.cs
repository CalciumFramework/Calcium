using System;
using System.Collections.Generic;
using Calcium.InversionOfControl;

namespace Calcium.UI.Data
{
	[DefaultType(typeof(NamespaceAliasRegistry), Singleton = true)]
	public interface INamespaceAliasRegistry
	{
		void RegisterNamespaceAlias(string xamlAlias, string @namespace, string assemblyName);
		bool TryResolveType(string alias, string typeShortName, out Type type);
	}

	[Preserve(AllMembers = true)]
	public class NamespaceAliasRegistry : INamespaceAliasRegistry
	{
		readonly Dictionary<string, string> aliasDictionary 
			= new Dictionary<string, string>();

		readonly Dictionary<string, Type> typeCache
			= new Dictionary<string, Type>();

		public NamespaceAliasRegistry()
		{
			RegisterNamespaceAlias("x", "Calcium.UI.Elements", "Calcium.Platform");
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
