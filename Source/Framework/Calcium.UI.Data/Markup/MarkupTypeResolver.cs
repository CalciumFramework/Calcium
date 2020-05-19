#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.UI.Data
{
	public class MarkupTypeResolver : IMarkupTypeResolver
	{
		public MarkupTypeResolver()
		{
			/* Intentionally left blank. */
		}

		public MarkupTypeResolver(INamespaceAliasRegistry namespaceRegistry)
		{
			this.namespaceRegistry_UseProperty = namespaceRegistry;
		}

		INamespaceAliasRegistry namespaceRegistry_UseProperty;

		INamespaceAliasRegistry NamespaceRegistry
		{
			get
			{
				if (namespaceRegistry_UseProperty == null)
				{
					namespaceRegistry_UseProperty = Dependency.Resolve<INamespaceAliasRegistry, NamespaceAliasRegistry>(true);
				}

				return namespaceRegistry_UseProperty;
			}
		}

		public Type Resolve(string qualifiedTypeName)
		{
			Type type;

			if (qualifiedTypeName.Contains(":"))
			{
				var aliasAndTypeNameArray = qualifiedTypeName.Split(':');
				if (aliasAndTypeNameArray.Length != 2)
				{
					throw new BindingException("Namespace aliased type name is invalid. " + qualifiedTypeName);
				}

				if (!NamespaceRegistry.TryResolveType(aliasAndTypeNameArray[0], aliasAndTypeNameArray[1], out type))
				{
					throw new BindingException("Unable to resolve namespace alias in " + qualifiedTypeName);
				}

				return type;
			}

			type = Type.GetType(qualifiedTypeName);

			return type;
		}

		public bool TryResolve(string qualifiedTypeName, out Type type)
		{
			if (qualifiedTypeName.Contains(":"))
			{
				var aliasAndTypeNameArray = qualifiedTypeName.Split(':');
				if (aliasAndTypeNameArray.Length != 2)
				{
					throw new BindingException("Namespace aliased type name is invalid. " + qualifiedTypeName);
				}

				if (!NamespaceRegistry.TryResolveType(aliasAndTypeNameArray[0], aliasAndTypeNameArray[1], out type))
				{
					return false;
				}

				return true;
			}

			type = Type.GetType(qualifiedTypeName, false);

			return type != null;
		}
	}
}
