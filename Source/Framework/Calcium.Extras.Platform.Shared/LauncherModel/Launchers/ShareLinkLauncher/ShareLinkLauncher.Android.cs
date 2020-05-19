#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 18:33:19Z</CreationDate>
</File>
*/
#endregion

using Android.Content;
using System;
using System.Threading.Tasks;

using Calcium.Concurrency;

namespace Calcium.LauncherModel.Launchers
{
	public class ShareLinkLauncher : IShareLinkLauncher
	{
		public Task<bool> ShowAsync()
		{
			try
			{
				Intent intent = new Intent(Intent.ActionSend);
				intent.SetType("text/plain");
				intent.PutExtra(Intent.ExtraText, linkUri.ToString());
				intent.PutExtra(Intent.ExtraSubject, title);
				var chooser = Intent.CreateChooser(intent, "Share");

				var context = Dependency.Resolve<Context>();
				context.StartActivity(chooser);
			}
			catch (Exception ex)
			{
				return TaskUtility.CreateTaskFromException<bool>(ex);
			}

			return Task.FromResult(true);
		}

		public async void Show()
		{
			bool result = await ShowAsync();

			Completed?.Invoke(this, result);
		}

		public event EventHandler<bool> Completed;


		Uri linkUri;

		public Uri LinkUri
		{
			get => linkUri;
			set => linkUri = value;
		}

		string message;

		public string Description
		{
			get => message;
			set => message = value;
		}

		string title;

		public string Title
		{
			get => title;
			set => title = value;
		}
	}
}
#endif
