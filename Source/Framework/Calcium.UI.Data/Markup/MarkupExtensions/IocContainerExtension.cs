using System;
using System.Linq;

using Calcium.InversionOfControl;
using Calcium.UI.Data;

namespace Calcium.UI.Elements
{
	public class IocContainerExtension : MarkupExtensionBase
	{
		readonly string namespaceAliasedTypeName;
		string containerKey;
		Type itemType;
		bool useItemType;
		string[] aliasAndTypeNameArray;

		public IocContainerExtension(string namespaceAliasedTypeNameOrContainerKey)
		{
			AssertArg.IsNotNull(namespaceAliasedTypeNameOrContainerKey, nameof(namespaceAliasedTypeNameOrContainerKey));

			if (namespaceAliasedTypeNameOrContainerKey.Contains(":"))
			{
				namespaceAliasedTypeName = namespaceAliasedTypeNameOrContainerKey;
				SetAliasAndTypeName(namespaceAliasedTypeName);
			}
			else
			{
				containerKey = namespaceAliasedTypeNameOrContainerKey;
			}
		}

		public IocContainerExtension(string namespaceAliasedTypeName, string containerKey)
		{
			this.namespaceAliasedTypeName = AssertArg.IsNotNullOrWhiteSpace(namespaceAliasedTypeName, nameof(namespaceAliasedTypeName));
			SetAliasAndTypeName(namespaceAliasedTypeName);

			this.containerKey = AssertArg.IsNotNull(containerKey, nameof(containerKey));
		}

		void SetAliasAndTypeName(string aliasAndType)
		{
			aliasAndTypeNameArray = aliasAndType.Split(':');
			if (aliasAndTypeNameArray.Length != 2)
			{
				throw new BindingException("Namespace aliased type name is invalid. " + aliasAndType);
			}
		}

		public override object ProvideValue(IContainer iocContainer)
		{
			object result;

			if (!useItemType && aliasAndTypeNameArray != null)
			{
				var namespaceRegistry = Dependency.Resolve<INamespaceAliasRegistry, NamespaceAliasRegistry>(true);

				if (namespaceRegistry.TryResolveType(aliasAndTypeNameArray[0], aliasAndTypeNameArray[1], out itemType))
				{
					useItemType = itemType != null;
				}
				else
				{
					throw new BindingException("Namespace aliased type name not found in namespace registry. " + namespaceAliasedTypeName);
				}
			}

			if (useItemType)
			{
				if (string.IsNullOrWhiteSpace(containerKey))
				{
					result = iocContainer.Resolve(itemType);
				}
				else
				{
					result = iocContainer.Resolve(itemType, containerKey);
				}
			}
			else
			{
				result = iocContainer.ResolveAll(containerKey).FirstOrDefault();
			}

			return result;
		}
	}
}
