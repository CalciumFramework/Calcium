#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-09 21:50:06Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.LauncherModel.Launchers
{
	public class MockPhotoResult : PhotoResultBase
	{
		LauncherResult? launcherResult;

		public override LauncherResult LauncherResult => launcherResult ?? base.LauncherResult;

		internal void SetTaskResult(LauncherResult launcherResult)
		{
			this.launcherResult = launcherResult;
		}
	}

}
