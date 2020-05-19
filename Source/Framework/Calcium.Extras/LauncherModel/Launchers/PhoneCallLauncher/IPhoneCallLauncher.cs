#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-11-26 16:42:05Z</CreationDate>
</File>
*/
#endregion

using Codon.InversionOfControl;

namespace Codon.LauncherModel.Launchers
{
	[DefaultTypeName(AssemblyConstants.Namespace + "." +
		nameof(LauncherModel) + "." + nameof(Launchers) + ".PhoneCallLauncher, " +
		AssemblyConstants.ExtrasPlatformAssembly, Singleton = false)]
	public interface IPhoneCallLauncher : ILauncher<bool>
	{
		string DisplayName { get; set; }
		string PhoneNumber { get; set; }
	}
}
