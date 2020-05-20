#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;

using Calcium.Services;

namespace Calcium.Device
{
	/// <summary>
	/// Android implementation of the <see cref="IClipboardService"/>.
	/// </summary>
	public class ClipboardService : IClipboardService
	{
		public void CopyToClipboard(object content, string description)
		{
			var text = content as string;

			if (text != null)
			{
				var context = Application.Context;
				var clipboardManager = (ClipboardManager)context.GetSystemService(Application.ClipboardService);
				ClipData clipData = ClipData.NewPlainText(description, text);
				clipboardManager.PrimaryClip = clipData;

				return;
			}

			var uri = content as Uri;

			if (uri != null)
			{
				var context = Application.Context;
				var clipboardManager = (ClipboardManager)context.GetSystemService(Application.ClipboardService);
				var androidUri = Android.Net.Uri.Parse(uri.ToString());
				ClipData clipData = ClipData.NewRawUri(description, androidUri);
				clipboardManager.PrimaryClip = clipData;

				return;
			}
			else
			{
				throw new NotImplementedException(
				   "Data formats for content have not yet been implemented.");
			}
		}

		public Task<object> GetClipboardContentsAsync()
		{
			var context = Application.Context;
			var clipboardManager = (ClipboardManager)context.GetSystemService(Application.ClipboardService);
			object result = clipboardManager.Text;

			//if (result == null && clipboardManager.HasPrimaryClip)
			//{
			//	var clip = clipboardManager.PrimaryClip;
			//	var item = clip.GetItemAt(0);
			//	/* TODO: Extract data. */
			//	result = item;
			//}

			return Task.FromResult(result);
		}
	}
}

#endif
