#if WPF || WINDOWS_PHONE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-01-04 15:16:08Z</CreationDate>
</File>
*/
#endregion

using System.Windows;

namespace Calcium.UI.Elements
{
	public static class UIElementExtensions
	{
		public static void ForceFocus(this UIElement uiElement)
		{
			AssertArg.IsNotNull(uiElement, nameof(uiElement));
			FocusForcer.Focus(uiElement);
		}

		public static Window GetWindow(this DependencyObject dependencyObject)
		{
			AssertArg.IsNotNull(dependencyObject, nameof(dependencyObject));

			var parent = LogicalTreeHelper.GetParent(dependencyObject);

			while (parent != null && !(parent is Window))
			{
				parent = LogicalTreeHelper.GetParent(parent);
			}

			return parent as Window;
		}
	}
}
#endif
