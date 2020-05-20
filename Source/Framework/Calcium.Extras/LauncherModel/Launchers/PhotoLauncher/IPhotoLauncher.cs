#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-09 20:59:49Z</CreationDate>
</File>
*/
#endregion

using Calcium.InversionOfControl;

namespace Calcium.LauncherModel.Launchers
{
	/// <summary>
	/// Launchers an external activity to retrieve a photo.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." +
		nameof(LauncherModel) + nameof(Launchers) + ".PhotoLauncher, " +
		AssemblyConstants.ExtrasPlatformAssembly, Singleton = false)]
	public interface IPhotoLauncher : ILauncher<IPhotoResult>
	{
		int PixelHeight { get; set; }
		int PixelWidth { get; set; }
		bool ShowCamera { get; set; }
	}
}
