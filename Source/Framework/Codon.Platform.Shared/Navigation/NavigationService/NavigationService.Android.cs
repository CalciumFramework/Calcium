#if __ANDROID__

#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-02-24 17:59:29Z</CreationDate>
</File>
*/
#endregion

using Android.App;
using Codon.Services;

namespace Codon.Navigation
{
	/// <summary>
	/// Android implementation of <see cref="INavigationService"/>.
	/// </summary>
	public class NavigationService : INavigationService
	{
		void NavigateBack()
		{
			if (Dependency.TryResolve(out Activity activity))
			{
				activity?.OnBackPressed();
			}
		}

		public void GoBack()
		{
			NavigateBack();
		}

		public bool CanGoBack => true;

		public void Navigate(string relativeUrl)
		{
			var routingService = (RoutingService)Dependency.Resolve<IRoutingService>();
			routingService.Navigate(relativeUrl);
		}
	}
}
#endif
