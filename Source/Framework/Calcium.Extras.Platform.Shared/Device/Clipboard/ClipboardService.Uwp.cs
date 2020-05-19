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
	<CreationDate>2017-03-15 09:18:03Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Codon.Services;

namespace Codon.Device
{
	/// <summary>
	/// UWP implementation of the <see cref="IClipboardService"/>.
	/// </summary>
	public class ClipboardService : IClipboardService
	{
		public void CopyToClipboard(object content, string description)
		{
			var dataPackage = new DataPackage();

			var text = content as string;
			if (text != null)
			{
				dataPackage.SetText(text);
				goto SetContent;
			}

			var uri = content as Uri;
			if (uri != null)
			{
				dataPackage.SetWebLink(uri);
				goto SetContent;
			}

			var bitmap = content as RandomAccessStreamReference;
			if (bitmap != null)
			{
				dataPackage.SetBitmap(bitmap);
				goto SetContent;
			}

			throw new NotImplementedException(
				   "Data format for this content have not yet been implemented.");

			SetContent:
			dataPackage.Properties.Description = description;
			Clipboard.SetContent(dataPackage);
		}

		public async Task<object> GetClipboardContentsAsync()
		{
			object result = null;
			var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

			if (dataPackageView.Contains(StandardDataFormats.Text))
			{
				result = await dataPackageView.GetTextAsync();
			}
			else if (dataPackageView.Contains(StandardDataFormats.WebLink))
			{
				result = await dataPackageView.GetWebLinkAsync();
			}
			else if (dataPackageView.Contains(StandardDataFormats.ApplicationLink))
			{
				result = await dataPackageView.GetApplicationLinkAsync();
			}
			else if (dataPackageView.Contains(StandardDataFormats.Bitmap))
			{
				result = await dataPackageView.GetBitmapAsync();
			}
			else if (dataPackageView.Contains(StandardDataFormats.Html))
			{
				result = await dataPackageView.GetHtmlFormatAsync();
			}
			else if (dataPackageView.Contains(StandardDataFormats.Rtf))
			{
				result = await dataPackageView.GetRtfAsync();
			}
			else if (dataPackageView.Contains(StandardDataFormats.StorageItems))
			{
				result = await dataPackageView.GetStorageItemsAsync();
			}

			return result;
		}
	}
}
#endif