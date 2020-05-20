#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-09 21:30:23Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.LauncherModel.Launchers
{
	/// <summary>
	/// This interface represents the result of an 
	/// <see cref="IPhotoLauncher"/>.
	/// It includes a <c>Stream</c> containing the selected
	/// photo data, and the file name of the selected photo.
	/// </summary>
	public interface IPhotoResult
	{
		System.IO.Stream ChosenPhoto { get; }

		string OriginalFileName { get; }

		LauncherResult LauncherResult { get; }
	}
}
