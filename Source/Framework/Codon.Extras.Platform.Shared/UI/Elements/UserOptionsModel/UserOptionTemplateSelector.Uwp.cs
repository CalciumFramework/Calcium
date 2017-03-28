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

using Windows.UI.Xaml;

using Codon.UserOptionsModel;

namespace Codon.UI.Elements
{
	public class UserOptionTemplateSelector : TypeTemplateSelector
	{
		protected override DataTemplate GetTemplate(object item, FrameworkElement resourcesOwner, bool lookInCache = true)
		{
			DataTemplate template;

			IUserOptionReaderWriter writer = item as IUserOptionReaderWriter;
			if (writer != null)
			{
				string templateName = writer.UserOption.TemplateName;
				if (string.IsNullOrWhiteSpace(templateName))
				{
					templateName = writer.UserOption.GetType().Name;
				}

				if (lookInCache)
				{
					if (TryGetCachedTemplate(templateName, out template) && template != null)
					{
						return template;
					}
				}

				template = GetTemplate(templateName, resourcesOwner);

				if (template == null)
				{
					template = base.GetTemplate(item, resourcesOwner, false);
				}
				else if (templateName != null)
				{
					CacheTemplate(templateName, template);
				}
			}
			else
			{
				template = base.GetTemplate(item, resourcesOwner, true);
			}

			return template;
		}
	}
}
#endif