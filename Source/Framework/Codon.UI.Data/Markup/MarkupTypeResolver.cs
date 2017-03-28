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

namespace Codon.UI.Data
{
	public class MarkupTypeResolver : IMarkupTypeResolver
	{
		static readonly INamespaceAliasRegistry namespaceRegistry 
			= Dependency.Resolve<INamespaceAliasRegistry, NamespaceAliasRegistry>(true);

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

				if (!namespaceRegistry.TryResolveType(aliasAndTypeNameArray[0], aliasAndTypeNameArray[1], out type))
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

				if (!namespaceRegistry.TryResolveType(aliasAndTypeNameArray[0], aliasAndTypeNameArray[1], out type))
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