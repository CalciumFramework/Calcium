#if __IOS__

#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-02-24 21:45:58Z</CreationDate>
</File>
*/
#endregion

using System;
using Codon.Services;
using UIKit;

namespace Codon.Navigation
{
	/// <summary>
	/// iOS implementation of <see cref="INavigationService"/>.
	/// </summary>
    public class NavigationService : INavigationService
    {
		object navigationArgument;

	    public object NavigationArgument
	    {
		    get => navigationArgument;
		    set => navigationArgument = value;
	    }

		public void GoBack()
	    {
		    NavigateBack();
	    }

	    public bool CanGoBack
	    {
		    get
		    {
				if (Dependency.TryResolve(out UINavigationController controller))
				{
					/* Untested. */
					if (controller.NavigationBar.BackItem != null)
					{
						return true;
					}
				}

			    return true;
		    }
	    }

	    public void Navigate(string relativeUrl, object navigationArgument = null)
	    {
		    NavigationArgument = navigationArgument;

			var routingService = (RoutingService)Dependency.Resolve<IRoutingService>();
			routingService.Navigate(relativeUrl, navigationArgument);
		}

	    void NavigateBack()
	    {
		    NavigationArgument = null;

			if (Dependency.TryResolve(out UINavigationController controller))
			{
				controller.PopViewController(true);
			}
		}
	}
}
#endif