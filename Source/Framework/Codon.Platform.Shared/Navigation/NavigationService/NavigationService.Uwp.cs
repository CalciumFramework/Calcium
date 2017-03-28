#if WINDOWS_UWP

#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
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
using Codon.Services;

namespace Codon.Navigation
{
	/// <summary>
	/// UWP implementation of <see cref="INavigationService"/>.
	/// </summary>
	public class NavigationService : INavigationService
	{
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
			Frame.Navigate(type, parameter);
		}

		public void GoBack()
		{
			Frame.GoBack();
		}

		public bool CanGoBack => Frame.CanGoBack;

		public void Navigate(string relativeUrl)
		{
			var routingService = (RoutingService)Dependency.Resolve<IRoutingService>();
			routingService.Navigate(relativeUrl);
		}
	}
}
#endif