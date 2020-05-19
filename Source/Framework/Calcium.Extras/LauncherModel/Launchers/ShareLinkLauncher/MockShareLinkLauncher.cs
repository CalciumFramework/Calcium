#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 18:32:22Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.LauncherModel.Launchers
{
	public class MockShareLinkLauncher : MockLauncherBase, 
		IShareLinkLauncher
	{
		public Uri LinkUri { get; set; }
		public string Description { get; set; }
		public string Title { get; set; }
	}
}
