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
using Codon.UI.Elements;

namespace Codon.UI.Data
{
	public class MarkupExtensionRegistry : IMarkupExtensionRegistry
	{
		readonly Dictionary<string, Type> extensionDictionary = new Dictionary<string, Type>();

		readonly Dictionary<string, Func<object[], IMarkupExtension>> funcDictionary 
			= new Dictionary<string, Func<object[], IMarkupExtension>>();

		readonly IMarkupTypeResolver xamlTypeResolver
			= Dependency.Resolve<IMarkupTypeResolver, MarkupTypeResolver>(true);

		public MarkupExtensionRegistry()
		{
			funcDictionary["Static"] = objects => new StaticExtension(objects?[0]?.ToString());
			funcDictionary["Ioc"] = objects => objects.Length > 1 
			? new IocContainerExtension(objects?[0]?.ToString(), objects?[1]?.ToString()) 
			: new IocContainerExtension(objects?[0]?.ToString());
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
					xamlTypeResolver.TryResolve(conventionName, out extensionType);
				}
			}

			if (extensionType == null)
			{
				if (!extensionDictionary.TryGetValue(xmlName, out extensionType))
				{
					xamlTypeResolver.TryResolve(xmlName, out extensionType);
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