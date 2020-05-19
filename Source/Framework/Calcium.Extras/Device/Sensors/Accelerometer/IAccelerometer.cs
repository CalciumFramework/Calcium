#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.InversionOfControl;

namespace Calcium.Device.Sensors
{
	/// <summary>
	/// Represents the hardware accelerometer sensor.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Device) + "." + nameof(Sensors) +
					".Accelerometer, " + AssemblyConstants.ExtrasPlatformAssembly, Singleton = true)]
	public interface IAccelerometer
	{
		/// <summary>
		/// Starts monitoring for changes.
		/// </summary>
		void StartMonitoring();

		/// <summary>
		/// Stops monitoring for changes.
		/// </summary>
		void StopMonitoring();

		/// <summary>
		/// Gets a value indicating whether device supports the accelerometer sensor.
		/// It is not a requirement that all do.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the device supports accelerometer; 
		/// otherwise, <c>false</c>.
		/// </value>
		bool DeviceSupportsAccelerometer { get; }

		/// <summary>
		/// Gets the last reading.
		/// </summary>
		/// <value>The last reading.</value>
		AccelerometerReading Reading { get; }

		/// <summary>
		/// Occurs when a new reading is received.
		/// </summary>
		event EventHandler<AccelerometerReadingChangedEventArgs> ReadingChanged;

		/// <summary>
		/// Indicate that the calibration of the sensor 
		/// would succeed along the X and Y axis
		/// because the device is stable enough, 
		/// or not inclined beyond a reasonable amount.
		/// </summary>
		/// <returns><c>true</c> if all of the X and Y axis 
		/// were stable enough and were not too inclined.</returns>
		bool CanCalibrate();

		/// <summary>
		/// Calibrates the accelerometer on X and Y axis 
		/// and saves the value to persistent storage.
		/// </summary>
		/// <returns><c>true</c> if succeeds.</returns>
		bool Calibrate();

		/// <summary>
		/// Occurs when the device is shaken.
		/// </summary>
		event EventHandler<AccelerometerShakenEventArgs> Shaken;
	}

	public class AccelerometerShakenEventArgs : EventArgs
	{
		public DateTimeOffset TimeStamp { get; private set; }

		public AccelerometerShakenEventArgs(DateTimeOffset timeStamp)
		{
			TimeStamp = timeStamp;
		}
	}

	public class AccelerometerReadingChangedEventArgs : EventArgs
	{
		public AccelerometerReading Reading { get; private set; }

		public AccelerometerReadingChangedEventArgs(
			AccelerometerReading reading)
		{
			Reading = reading;
		}
	}
}
