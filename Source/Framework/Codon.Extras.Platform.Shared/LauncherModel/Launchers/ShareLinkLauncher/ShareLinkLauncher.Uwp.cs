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
	<CreationDate>2017-03-15 18:35:07Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;

namespace Codon.LauncherModel.Launchers
{
	public class ShareLinkLauncher : IShareLinkLauncher
	{
		public Uri LinkUri { get; set; }

		public string Description { get; set; }

		public string Title { get; set; }

		public Task<bool> ShowAsync()
		{
			var dataTransferManager = DataTransferManager.GetForCurrentView();
			dataTransferManager.DataRequested += HandleDataRequested;

			DataTransferManager.ShowShareUI();

			return Task.FromResult(true);
		}

		public async void Show()
		{
			bool result = await ShowAsync();

			Completed?.Invoke(this, result);
		}

		public event EventHandler<bool> Completed; 

		void HandleDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
		{
			var dataTransferManager = DataTransferManager.GetForCurrentView();
			dataTransferManager.DataRequested -= HandleDataRequested;

			e.Request.Data.Properties.Title = Title;
			e.Request.Data.Properties.Description = Description; // Optional 
			e.Request.Data.SetWebLink(LinkUri);
		}
	}
}
#endif