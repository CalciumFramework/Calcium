#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion
using System;

using Android.App;
using Android.Content;
using Android.Hardware;

namespace Calcium.Device.Sensors
{
	/// <summary>
	/// Android implementation of the <see cref="IAccelerometer"/>
	/// interface.
	/// </summary>
	public class Accelerometer : IAccelerometer
	{
		SensorManager sensorManager;
		SensorListener sensorListener;
		bool monitoring;
		Sensor accelerometer;
		readonly float[] gravityArray = new float[3];

		/// <inheritdoc />
		public void StartMonitoring()
		{
			if (monitoring)
			{
				return;
			}

			monitoring = true;

			var activity = Dependency.Resolve<Activity>();

			sensorManager = (SensorManager)activity.GetSystemService(Context.SensorService);
			accelerometer = sensorManager.GetDefaultSensor(SensorType.Accelerometer);

			if (sensorListener == null)
			{
				sensorListener = new SensorListener(this);
			}

			sensorManager.RegisterListener(
				sensorListener,
				sensorManager.GetDefaultSensor(SensorType.Accelerometer),
				SensorDelay.Ui);
		}

		/// <inheritdoc />
		public void StopMonitoring()
		{
			if (!monitoring)
			{
				return;
			}

			monitoring = false;

			sensorManager.UnregisterListener(sensorListener);
		}

		void HandleSensorChanged(SensorEvent e)
		{
			var values = e.Values;
			if (values == null || values.Count < 3)
			{
				return;
			}

			var x = e.Values[0];
			var y = e.Values[1];
			var z = e.Values[2];

			DateTimeOffset dateTimeOffset;

			long timeStamp = e.Timestamp;
			if (timeStamp > -62135596800000 && timeStamp < 253402300799999)
			{
				dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(e.Timestamp);
			}
			else
			{
				dateTimeOffset = DateTimeOffset.Now;
			}

			var rawAcceleration = new Vector3D(x, y, z);

			float alpha = 0.8f;

			gravityArray[0] = alpha * gravityArray[0] + (1 - alpha) * x;
			gravityArray[1] = alpha * gravityArray[1] + (1 - alpha) * y;
			gravityArray[2] = alpha * gravityArray[2] + (1 - alpha) * z;

			var lx = x - gravityArray[0];
			var ly = y - gravityArray[1];
			var lz = z - gravityArray[2];

			var linearAcceleration = new Vector3D(lx, ly, lz);

			var newReading = new AccelerometerReading(dateTimeOffset,
				linearAcceleration, null, null, null);

			Reading = newReading;

			OnReadingChanged(new AccelerometerReadingChangedEventArgs(newReading));
		}

		void HandleAccuracyChanged(Sensor sensor, SensorStatus sensorStatus)
		{

		}

		public bool DeviceSupportsAccelerometer
		{
			get
			{
				if (accelerometer != null)
				{
					return true;
				}

				if (sensorManager == null)
				{
					var activity = Dependency.Resolve<Activity>();
					sensorManager = (SensorManager)activity.GetSystemService(Context.SensorService);
					accelerometer = sensorManager.GetDefaultSensor(SensorType.Accelerometer);
				}

				return accelerometer != null;
			}
		}

		/// <inheritdoc />
		public AccelerometerReading Reading { get; private set; }

		/// <inheritdoc />
		public event EventHandler<AccelerometerReadingChangedEventArgs> ReadingChanged;


		/// <summary>
		/// Raises the ReadingChanged event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnReadingChanged(AccelerometerReadingChangedEventArgs e)
		{
			ReadingChanged?.Invoke(this, e);
		}

		/// <inheritdoc />
		public bool CanCalibrate()
		{
			return false;
		}

		/// <inheritdoc />
		public bool Calibrate()
		{
			return true;
		}

		public event EventHandler<AccelerometerShakenEventArgs> Shaken;

		class SensorListener : Java.Lang.Object, ISensorEventListener
		{
			readonly Accelerometer accelerometer;

			internal SensorListener(Accelerometer accelerometer)
			{
				this.accelerometer = accelerometer;
			}

			public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
			{
				accelerometer.HandleAccuracyChanged(sensor, accuracy);
			}

			public void OnSensorChanged(SensorEvent e)
			{
				accelerometer.HandleSensorChanged(e);
			}
		}
	}
}
#endif
