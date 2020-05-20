#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-11-26 16:42:05Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using Calcium.InversionOfControl;

namespace Calcium.LauncherModel.Launchers
{
	[DefaultTypeName(AssemblyConstants.Namespace + "." +
		nameof(LauncherModel) + "." + nameof(Launchers) + ".EmailComposeLauncher, " +
		AssemblyConstants.ExtrasPlatformAssembly, Singleton = false)]
	public interface IEmailComposeLauncher : ILauncher<bool>
	{
		string Body { get; set; }

		string Subject { get; set; }

		IList<string> ToList { get; }

		IList<string> CCList { get; }

		int? CodePage { set; get; }

		string MimeType { get; set; }

		bool UseHtmlFormat { get; set; }

		bool CanSendMail { get; }
	}
}
