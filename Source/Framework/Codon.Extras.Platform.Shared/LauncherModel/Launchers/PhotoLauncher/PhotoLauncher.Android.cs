#if __Android__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-21 18:53:04Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Android.Webkit;

using Codon.ApplicationModel;
using Codon.Concurrency;
using Codon.Messaging;

namespace Codon.LauncherModel.Launchers
{
	/// <summary>
	/// Android implementation of <see cref="IPhotoLauncher"/>.
	/// </summary>
	public class PhotoLauncher : IPhotoLauncher, 
		IMessageSubscriber<ActivityResultMessage>
	{
		public void Show()
		{
			Intent intent = new Intent();
			intent.SetType("image/*");
			intent.SetAction(Intent.ActionGetContent);

			var activity = Dependency.Resolve<Activity>();
			var messenger = Dependency.Resolve<IMessenger>();
			messenger.Subscribe(this);

			activity.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), LauncherRequestIds.PhotoChooser);
		}

		public event EventHandler<PhotoResultBase> Completed;

		public int PixelHeight { get; set; }
		public int PixelWidth { get; set; }
		public bool ShowCamera { get; set; }

		Task IMessageSubscriber<ActivityResultMessage>.ReceiveMessageAsync(ActivityResultMessage message)
		{
			if (message.RequestCode == LauncherRequestIds.PhotoChooser)
			{
				var messenger = Dependency.Resolve<IMessenger>();
				messenger.Unsubscribe(this);

				PhotoResult photoResult;

				if (message.ResultCode == Result.Ok)
				{
					var uri = message.Intent.Data;

					var context = Dependency.Resolve<Context>();

					string fileName = GetFileName(context, uri);

					if (string.IsNullOrWhiteSpace(fileName))
					{
						string extension = GetExtension(context, uri);
						fileName = "Unknown." + extension;
					}

					photoResult = new PhotoResult
						{
							Uri = uri,
							OriginalFileName = fileName,
							LauncherResult = LauncherResult.OK
						};
				}
				else
				{
					photoResult = new PhotoResult
						{
							LauncherResult = LauncherResult.Cancel
						};
				}

				OnCompleted(photoResult);
			}

			return TaskUtility.CreateTaskWithResult();
		}

		protected virtual void OnCompleted(PhotoResultBase e)
		{
			Completed?.Invoke(this, e);
			Completed = null;
		}

		static string GetExtension(Context context, Android.Net.Uri uri)
		{
			string extension;

			//Check uri format to avoid null
			if (uri.Scheme.Equals(ContentResolver.SchemeContent))
			{
				//If scheme is a content
				MimeTypeMap mime = MimeTypeMap.Singleton;
				extension = mime.GetExtensionFromMimeType(context.ContentResolver.GetType(uri));
			}
			else
			{
				//If scheme is a File
				//This will replace white spaces with %20 and also other special characters. This will avoid returning null values on file name with spaces and special characters.
				extension = MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(new Java.IO.File(uri.Path)).ToString());

			}

			return extension;
		}

		string GetFileName(Context context, Android.Net.Uri selectedImage)
		{
			string[] filePathColumn = {MediaStore.Images.ImageColumns.Data,
						   MediaStore.Images.ImageColumns.DisplayName};
			var cursor = context.ContentResolver.Query(selectedImage, filePathColumn, null, null, null);
			try
			{
				if (cursor.MoveToFirst())
				{
					int columnIndex = cursor.GetColumnIndex(filePathColumn[0]);
					string filePath = cursor.GetString(columnIndex);
					//Bitmap bitmap = BitmapFactory.DecodeFile(filePath);
					int fileNameIndex = cursor.GetColumnIndex(filePathColumn[1]);
					string fileName = cursor.GetString(fileNameIndex);

					return fileName;
				}
			}
			finally
			{
				cursor.Close();
			}

			return null;
		}
	}
}
#endif