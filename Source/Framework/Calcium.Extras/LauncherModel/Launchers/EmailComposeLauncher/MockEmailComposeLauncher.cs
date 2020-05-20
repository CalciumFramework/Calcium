#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-11-26 16:51:12Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Calcium.LauncherModel.Launchers
{
	public class MockEmailComposeLauncher : MockLauncherBase, 
		IEmailComposeLauncher
	{
		public Task<bool> ShowAsync()
		{
			Show();

			return Task.FromResult(true);
		}

		public string Body { get; set; }

		readonly IList<string> ccList = new List<string>();

		public IList<string> CCList => ccList;

		public int? CodePage { get; set; }

		public string MimeType { get; set; }
		public bool UseHtmlFormat { get; set; }

		public bool CanSendMail => true;

		public string Subject { get; set; }

		readonly IList<string> toList = new List<string>();

		public IList<string> ToList => toList;

		public override void Show()
		{
			Shown = true;

			Debug.WriteLine("MockEmailComposeTask Shown. Body {0}, Cc {1}, Subject {2}, To {3}, MimeType {4}, CodePage {5}", Body, CCList, Subject, ToList, MimeType, CodePage);

			Completed?.Invoke(this, true);
		}

		public override event EventHandler<bool> Completed;
	}
}
