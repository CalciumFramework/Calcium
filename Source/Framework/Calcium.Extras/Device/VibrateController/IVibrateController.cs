#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 15:17:00Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.Device;
using Calcium.InversionOfControl;

namespace Calcium.Services
{
	/// <summary>
	/// This interface allows the abstraction of the platform
	/// specific device vibration APIs.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Device) +
		".VibrateController, " + AssemblyConstants.ExtrasPlatformAssembly, Singleton = true)]
	[DefaultType(typeof(NullVibrateController), Singleton = true)]
	public interface IVibrateController
	{
		/// <summary>
		/// Start vibrating for the specified duration.
		/// </summary>
		/// <param name="duration">
		/// The vibration duration.</param>
		void Start(TimeSpan duration);

		/// <summary>
		/// Cancel all in progress vibrations.
		/// </summary>
		void Stop();
	}
}
