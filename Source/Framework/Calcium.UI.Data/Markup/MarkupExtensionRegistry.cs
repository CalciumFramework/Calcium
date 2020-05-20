#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using Calcium.UI.Elements;

namespace Calcium.UI.Data
{
	[Preserve(AllMembers = true)]
	public class MarkupExtensionRegistry : IMarkupExtensionRegistry
	{
		readonly Dictionary<string, Type> extensionDictionary = new Dictionary<string, Type>();

		readonly Dictionary<string, Func<object[], IMarkupExtension>> funcDictionary 
			= new Dictionary<string, Func<object[], IMarkupExtension>>();

		IMarkupTypeResolver xamlTypeResolver_UseProperty;

		IMarkupTypeResolver XamlTypeResolver
		{
			get
			{
				if (xamlTypeResolver_UseProperty == null)
				{
					xamlTypeResolver_UseProperty = Dependency.Resolve<IMarkupTypeResolver>();
				}

				return xamlTypeResolver_UseProperty;
			}
		}

		public MarkupExtensionRegistry()
		{
			funcDictionary["Static"] = objects => new StaticExtension(objects?[0]?.ToString());
			funcDictionary["Ioc"] = objects => objects.Length > 1 
			? new IocContainerExtension(objects?[0]?.ToString(), objects?[1]?.ToString()) 
			: new IocContainerExtension(objects?[0]?.ToString());
			funcDictionary["StaticResource"] = objects => new StaticResourceExtension(objects?[0]?.ToString());
		}

		public void RegisterExtension<T>(string xmlName) where T : IMarkupExtension
		{
			extensionDictionary[xmlName] = typeof(T);
		}

		public void RegisterExtension<T>(string xmlName, Func<object[], IMarkupExtension> createExtensionFunc) 
			where T : IMarkupExtension
		{
			funcDictionary[xmlName] = createExtensionFunc;
		}

		public bool TryGetExtensionCreationFunc(
			string xmlName, out Func<object[], IMarkupExtension> resultFunc)
		{
			if (funcDictionary.TryGetValue(xmlName, out resultFunc) && resultFunc != null)
			{
				return true;
			}

			Type extensionType = null;

			if (!xmlName.EndsWith("Extension", StringComparison.OrdinalIgnoreCase))
			{
				string conventionName = xmlName + "Extension";
				if (!extensionDictionary.TryGetValue(conventionName, out extensionType))
				{
					XamlTypeResolver.TryResolve(conventionName, out extensionType);
				}
			}

			if (extensionType == null)
			{
				if (!extensionDictionary.TryGetValue(xmlName, out extensionType))
				{
					XamlTypeResolver.TryResolve(xmlName, out extensionType);
					if (extensionType == null)
					{
						return false;
					}
				}
			}

			resultFunc = objects =>
			{
				try
				{
					var extension = (IMarkupExtension)Activator.CreateInstance(extensionType, objects);
					return extension;
				}
				catch (Exception ex)
				{
					throw new BindingException("Unable to create MarkupExtension. " + extensionType + " Parameters: " + string.Join(", ", objects), ex);
				}
			};

			funcDictionary[xmlName] = resultFunc;

			return true;
		}

		public bool GetExtensionCreationFunc<T>(
			string xmlName, out Func<object[], IMarkupExtension> resultFunc)
		{
			if (funcDictionary.TryGetValue(xmlName, out resultFunc) && resultFunc != null)
			{
				return true;
			}

			resultFunc = objects =>
			{
				try
				{
					var extension = (IMarkupExtension)Activator.CreateInstance(typeof(T), objects);
					return extension;
				}
				catch (Exception ex)
				{
					throw new BindingException("Unable to create MarkupExtension. " + typeof(T) + " Parameters: " + string.Join(", ", objects), ex);
				}
			};

			funcDictionary[xmlName] = resultFunc;

			return true;
		}
	}
}
