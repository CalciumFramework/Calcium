using System;
using Calcium.InversionOfControl;

namespace Calcium.LauncherModel.Launchers
{
	[DefaultTypeName(AssemblyConstants.Namespace + "." +
		nameof(LauncherModel) + "." + nameof(Launchers) + ".ShareLinkLauncher, " +
		AssemblyConstants.ExtrasPlatformAssembly, Singleton = false)]
	public interface IShareLinkLauncher : ILauncher<bool>
	{
		Uri LinkUri
		{
			get;
			set;
		}

		string Description
		{
			get;
			set;
		}

		string Title
		{
			get;
			set;
		}
	}
}
