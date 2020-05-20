#if WINDOWS_UWP || NETFX_CORE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.Store;
using Windows.System;

using Calcium.Services;

namespace Calcium.MarketplaceModel
{
	public class MarketplaceService : IMarketplaceService
	{
		public bool Trial
		{
			get
			{
				var licenseInformation = CurrentApp.LicenseInformation;
				return !licenseInformation.IsActive || licenseInformation.IsTrial;
			}
		}

		public async Task<object> PurchaseAppAsync()
		{
			var result =  await CurrentApp.RequestAppPurchaseAsync(true);
			return result;
		}

		public async void Review()
		{
			var storeUri = new Uri(string.Format("ms-windows-store:reviewapp?appid={0}", CurrentApp.AppId));
			var result = await Launcher.LaunchUriAsync(storeUri);
		}

		public async void ShowDetails(object contentId = null)
		{
			var storeUri = new Uri(string.Format("ms-windows-store:PDP?PFN={0}", contentId ?? CurrentApp.AppId.ToString()));
			var result = await Launcher.LaunchUriAsync(storeUri);
		}
	}
}
#endif
