#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-09 21:35:13Z</CreationDate>
</File>
*/
#endregion

using System.IO;

namespace Calcium.LauncherModel.Launchers
{
	/// <summary>
	/// The base class for a photo chooser result, which includes,
	/// for example, the photo that the user selected.
	/// </summary>
	public abstract class PhotoResultBase :
#if WINDOWS_PHONE
		Microsoft.Phone.Tasks.TaskEventArgs,
#endif
		IPhotoResult
	{
		public virtual Stream ChosenPhoto { get; set; }

		public virtual string OriginalFileName { get; set; }

#if __ANDROID__
		public virtual Android.Net.Uri Uri { get; set; }
#endif

#if !WINDOWS_PHONE
		public virtual LauncherResult LauncherResult { get; set; }
#endif
	}
}
