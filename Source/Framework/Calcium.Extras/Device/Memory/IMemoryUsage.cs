using Calcium.InversionOfControl;

namespace Calcium.Device
{
	/// <summary>
	/// Classes that implement this interface provide
	/// information on application memory usage.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Device) +
					".MemoryUsage, " + AssemblyConstants.ExtrasPlatformAssembly, Singleton = true)]
	public interface IMemoryUsage
    {
		/// <summary>
		/// The memory usage of the application in megabytes.
		/// </summary>
	    long ApplicationMemoryUsageMB { get; }

		/// <summary>
		/// The available memory of the application in megabytes.
		/// </summary>
	    long ApplicationAvailableMemoryMB { get; }

		/// <summary>
		/// The maximum memory the application is allowed
		/// to consume, in megabytes.
		/// </summary>
	    long ApplicationMemoryLimitMB { get; }
	}
}
