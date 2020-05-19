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
	<CreationDate>2010-11-26 16:51:23Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Windows.System;

using Calcium.Networking;

namespace Calcium.LauncherModel.Launchers
{
	/// <summary>
	/// UWP implementation of <see cref="IEmailComposeLauncher"/>.
	/// </summary>
	public class EmailComposeLauncher : IEmailComposeLauncher
	{
		public async Task<bool> ShowAsync()
		{
			StringBuilder url = new StringBuilder("mailto:?");

			var to = String.Join(";", toList);

			if (!string.IsNullOrWhiteSpace(to))
			{
				url.Append("to=");
				url.Append(WebUtilityExtended.UrlEncode(to));
				url.Append('&');
			}

			var cc = String.Join(";", toList);

			if (!string.IsNullOrWhiteSpace(cc))
			{
				url.Append("cc=");
				url.Append(WebUtilityExtended.UrlEncode(cc));
				url.Append('&');
			}

			var bcc = String.Join(";", bccList);

			if (string.IsNullOrWhiteSpace(bcc))
			{
				url.Append("bcc=");
				url.Append(WebUtilityExtended.UrlEncode(Body));
				url.Append('&');
			}

			if (string.IsNullOrWhiteSpace(Subject))
			{
				url.Append("subject=");
				url.Append(WebUtilityExtended.UrlEncode(Subject));
				url.Append('&');
			}

			if (string.IsNullOrWhiteSpace(Body))
			{
				url.Append("body=");
				url.Append(WebUtilityExtended.UrlEncode(Body));
				url.Append('&');
			}

			var mailToUri = new Uri(url.ToString());
			await Launcher.LaunchUriAsync(mailToUri);

			return true;
		}

		public async void Show()
		{
			bool result = await ShowAsync();

			Completed?.Invoke(this, result);
		}

		public event EventHandler<bool> Completed; 

		public string Body { get; set; }

		readonly List<string> toList = new List<string>();

		public IList<string> ToList => toList;

		readonly List<string> ccList = new List<string>();

		public IList<string> CCList => ccList;

		readonly List<string> bccList = new List<string>();

		public IList<string> BccList => bccList;

		public string Subject { get; set; }

		public int? CodePage { get; set; }

		public string MimeType { get; set; }

		/// <summary>
		/// This is ignored in Windows Phone.
		/// </summary>
		public bool UseHtmlFormat { get; set; }

		/// <summary>
		/// Always returns <c>true</c> on Windows Phone.
		/// </summary>
		public bool CanSendMail => true;
	}
}
#endif
