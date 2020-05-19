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
	<CreationDate>2017-03-21 18:52:17Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;

using Codon.LauncherModel;

namespace Codon.LauncherModel.Launchers
{
	/// <summary>
	/// Android implementation of <see cref="IEmailComposeLauncher"/>.
	/// </summary>
	public class EmailComposeLauncher : IEmailComposeLauncher
	{
		public string Subject { get; set; }

		public string Body { get; set; }

		readonly IList<string> toList = new List<string>();

		public IList<string> ToList => toList;

		readonly IList<string> ccList = new List<string>();

		public IList<string> CCList => ccList;

		/// <summary>
		/// This property is ignored in Android.
		/// </summary>
		public int? CodePage { get; set; }

		public string MimeType { get; set; }

		public bool UseHtmlFormat { get; set; }

		public bool CanSendMail => true;

		readonly IList<string> bccList = new List<string>();

		public IList<string> BccList => bccList;

		public Task<bool> ShowAsync()
		{
			var emailIntent = new Intent(Intent.ActionSend);

			if (toList.Any())
			{
				emailIntent.PutExtra(Intent.ExtraEmail, toList.ToArray());
			}

			if (ccList.Any())
			{
				emailIntent.PutExtra(Intent.ExtraCc, ccList.ToArray());
			}

			if (bccList.Any())
			{
				emailIntent.PutExtra(Intent.ExtraBcc, bccList.ToArray());
			}

			if (!string.IsNullOrWhiteSpace(Subject))
			{
				emailIntent.PutExtra(Intent.ExtraSubject, Subject);
			}

			if (!string.IsNullOrWhiteSpace(Body))
			{
				emailIntent.PutExtra(Intent.ExtraText, Body);
			}

			if (!string.IsNullOrWhiteSpace(MimeType))
			{
				emailIntent.SetType(MimeType);
			}
			else
			{
				emailIntent.SetType("text/html");
				//emailIntent.SetType("message/rfc822");
			}

			/* TODO: Make localizable resource. */
			var chooser = Intent.CreateChooser(emailIntent, "Email");

			var context = Dependency.Resolve<Context>();

			context.StartActivity(chooser);

			return Task.FromResult(true);
		}

		public async void Show()
		{
			bool result = await ShowAsync();

			Completed?.Invoke(this, result);
		}

		public event EventHandler<bool> Completed;
	}
}
#endif