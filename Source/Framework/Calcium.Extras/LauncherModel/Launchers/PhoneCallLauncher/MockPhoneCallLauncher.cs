#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-11-26 16:51:12Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;

namespace Calcium.LauncherModel.Launchers
{
	public class MockPhoneCallTask : MockLauncherBase, IPhoneCallLauncher
	{
		public override void Show()
		{
			Debug.WriteLine("MockPhoneCallTask Shown. DisplayName {0}, "
							+ "PhoneNumber {1}", DisplayName, PhoneNumber);
			base.Show();
		}

		public string DisplayName { get; set; }

		public string PhoneNumber { get; set; }
	}
}
