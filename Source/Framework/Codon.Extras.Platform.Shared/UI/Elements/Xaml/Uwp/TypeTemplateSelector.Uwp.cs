#if WINDOWS_UWP || NETFX_CORE
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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Codon.Logging;

namespace Codon.UI.Elements
{
	public class TypeTemplateSelector : DataTemplateSelector
	{
		WeakReference<FrameworkElement> resourceOwnerReference; 

		readonly Dictionary<string, DataTemplate> templateLookup
						= new Dictionary<string, DataTemplate>();

		protected void CacheTemplate(string templateName, DataTemplate template)
		{
			templateLookup[templateName] = template;
		}

		protected void ClearCache()
		{
			templateLookup.Clear();
		}

		protected bool TryGetCachedTemplate(string templateName, out DataTemplate template)
		{
			return templateLookup.TryGetValue(templateName, out template);
		}

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			DataTemplate result = null;
			var resourcesOwner = container as FrameworkElement;
			if (resourcesOwner != null)
			{
				result = GetTemplate(item, resourcesOwner, true);
			}

			if (result == null)
			{
				result = base.SelectTemplateCore(item, container);
			}

			return result;
		}

		protected virtual DataTemplate GetTemplate(
			object item, FrameworkElement resourcesOwner, bool lookInCache)
		{
			if (item == null)
			{
				return null;
			}

			DataTemplate template;

			string templateName = item.GetType().Name;

			if (lookInCache)
			{
				if (TryGetCachedTemplate(templateName, out template)
						&& template != null)
				{
					return template;
				}
			}

			template = GetTemplate(templateName, resourcesOwner);

			if (template == null)
			{
				if (Dependency.TryResolve(out ILog log))
				{
					log.Error("A template was not found with the name:" + templateName);
				}

				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
			}

			return template;
		}

		protected virtual DataTemplate GetTemplate(
			string templateName, FrameworkElement resourcesOwner)
		{
			DataTemplate template;

			if (TryGetTemplateFromPreviousResourceOwner(templateName, out template) && template != null)
			{
				CacheTemplate(templateName, template);
				return template;
			}

			while (true)
			{
				var resourcesDictionary = resourcesOwner.Resources;
				object templateObject;
				if (resourcesDictionary != null && resourcesDictionary.TryGetValue(templateName, out templateObject))
				{
					template = (DataTemplate)templateObject;
				}
				else
				{
					var parent = VisualTreeHelper.GetParent(resourcesOwner) as FrameworkElement;
					if (parent != null)
					{
						resourcesOwner = parent;
						continue;
					}
				}

				if (template != null)
				{
					CacheTemplate(templateName, template);

					if (resourcesOwner != null)
					{
						resourceOwnerReference = new WeakReference<FrameworkElement>(resourcesOwner);
                    }
				}

				return template;
			}
		}

		protected bool TryGetTemplateFromPreviousResourceOwner(string templateName, out DataTemplate template)
		{
			FrameworkElement resourcesOwner;
			if (resourceOwnerReference != null && resourceOwnerReference.TryGetTarget(out resourcesOwner))
			{
				var resourcesDictionary = resourcesOwner.Resources;

				object templateObject;
				if (resourcesDictionary != null && resourcesDictionary.TryGetValue(templateName, out templateObject))
				{
					template = (DataTemplate)templateObject;
					return true;
				}
			}

			template = null;

			return false;
		}
	}
}
#endif