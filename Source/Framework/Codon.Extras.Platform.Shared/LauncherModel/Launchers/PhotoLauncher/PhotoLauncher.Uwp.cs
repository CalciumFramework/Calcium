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
	<CreationDate>2010-12-09 21:03:34Z</CreationDate>
</File>
*/
#endregion

using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Storage;

namespace Codon.LauncherModel.Launchers
{
	/// <summary>
	/// The UWP implementation of the <c>IPhotoLauncher</c>.
	/// </summary>
	public class PhotoLauncher : IPhotoLauncher
	{
        public async Task<PhotoResultBase> ShowAsync()
        {
			CameraCaptureUI dialog = new CameraCaptureUI();

	        if (PixelHeight > 0 || PixelWidth > 0)
	        {
		        dialog.PhotoSettings.CroppedSizeInPixels = new Size(PixelWidth, PixelHeight);
	        }
			StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);

	        if (file != null)
	        {
		        var result = new PhotoResult()
			        {
				        ChosenPhoto = await file.OpenStreamForReadAsync(),
						OriginalFileName = file.Name
			        };

		        return result;
	        }

	        return null;
        }

		public int PixelHeight { get; set; }

		public int PixelWidth { get; set; }

		public bool ShowCamera { get; set; }

		public async void Show()
		{
			var result = await ShowAsync();

			Completed?.Invoke(this, result);
		}

		public event EventHandler<IPhotoResult> Completed;
	}

	public class PhotoResult : PhotoResultBase
	{
		
	}
}
#endif