#if WINDOWS_UWP

#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-02-24 17:17:39Z</CreationDate>
</File>
*/
#endregion

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Calcium.Services;

namespace Calcium.Navigation
{
	/// <summary>
	/// UWP implementation of <see cref="INavigationService"/>.
	/// </summary>
	public class NavigationService : INavigationService
	{
		object navigationArgument;

		public object NavigationArgument
		{
			get => navigationArgument;
			set => navigationArgument = value;
		}

		Frame frameUseProperty;

		Frame Frame
		{
			get
			{
				if (frameUseProperty == null)
				{
					frameUseProperty = (Frame)Window.Current.Content;

					if (frameUseProperty == null)
					{
						throw new InvalidOperationException(
							"The application does not contain a root frame.");
					}
				}

				return frameUseProperty;
			}
		}

		public void NavigateUsingFrame(Type type, object parameter)
		{
			NavigationArgument = parameter;

			Frame.Navigate(type, parameter);
		}

		public void GoBack()
		{
			NavigationArgument = null;

			Frame.GoBack();
		}

		public bool CanGoBack => Frame.CanGoBack;

		public void Navigate(string relativeUrl, object navigationArgument = null)
		{
			NavigationArgument = navigationArgument;

			var routingService = (RoutingService)Dependency.Resolve<IRoutingService>();
			routingService.Navigate(relativeUrl, navigationArgument);
		}
	}
}
#endif
