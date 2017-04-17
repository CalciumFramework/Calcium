#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-04-06 12:51:47Z</CreationDate>
</File>
*/
#endregion

using System;

using Android.App;
using Android.Content;
using Android.OS;

using Codon.ComponentModel;
using Codon.Device;
using Codon.Services;

namespace Codon.Devices
{
	/// <summary>
	/// Android implementation of the <see cref="IPowerService"/>.
	/// </summary>
	public class PowerService : ObservableBase, IPowerService
	{
		Intent batteryStatusIntent;
		PowerConnectionReceiver receiver;
		bool listening;
		readonly object listeningLock = new object();

		public void Start()
		{
			if (!listening)
			{
				lock (listeningLock)
				{
					if (!listening)
					{
						var intentFilter = new IntentFilter(/*Intent.ActionBatteryChanged*/);
						intentFilter.AddAction(Intent.ActionBatteryLow);
						intentFilter.AddAction(Intent.ActionBatteryOkay);

						var context = Application.Context;
						receiver = new PowerConnectionReceiver(this);
						batteryStatusIntent = context.RegisterReceiver(receiver, intentFilter);

						listening = true;
					}
				}
			}
		}

		public void Stop()
		{
			if (listening)
			{
				lock (listeningLock)
				{
					if (listening)
					{
						var context = Application.Context;
						context.UnregisterReceiver(receiver);

						listening = false;
					}
				}
			}
		}

		DevicePowerSource powerSource;

		public DevicePowerSource PowerSource
		{
			get => powerSource = GetPowerSource(batteryStatusIntent);
			set => Set(ref powerSource, value);
		}

		int chargeRemainingPercent;

		public int RemainingBatteryChargePercent
		{
			get => chargeRemainingPercent = (int)GetRemainingBatteryLevel(batteryStatusIntent);
			set => Set(ref chargeRemainingPercent, value);
		}

		/// <summary>
		/// Not supported on Android.
		/// </summary>
		/// <exception cref="NotSupportedException" />
		public TimeSpan RemainingBatteryDischargeTime => throw new NotSupportedException();

		float GetRemainingBatteryLevel(Intent intent)
		{
			int level = intent.GetIntExtra(BatteryManager.ExtraLevel, -1);
			int scale = intent.GetIntExtra(BatteryManager.ExtraScale, -1);
			float percentageRemaining = level / (float)scale;

			return percentageRemaining;
		}

		DevicePowerSource GetPowerSource(Intent intent)
		{
			BatteryPlugged chargePlug = (BatteryPlugged)intent.GetIntExtra(BatteryManager.ExtraPlugged, -1);
			bool usbCharge = chargePlug == BatteryPlugged.Usb;
			bool acCharge = chargePlug == BatteryPlugged.Ac;

			var result = usbCharge || acCharge
				? DevicePowerSource.External
				: DevicePowerSource.Battery;

			return result;
		}

		void Update(Intent intent)
		{

			RemainingBatteryChargePercent = (int)GetRemainingBatteryLevel(intent);
			PowerSource = GetPowerSource(intent);
			//BatteryStatus status = (BatteryStatus)intent.GetIntExtra(BatteryManager.ExtraStatus, -1);
			//			bool isCharging = status == BatteryStatus.Charging ||
			//								status == BatteryStatus.Full;
		}

		public class PowerConnectionReceiver : BroadcastReceiver
		{
			PowerService powerService;

			public PowerConnectionReceiver(PowerService powerService)
			{
				this.powerService = powerService;
			}

			public override void OnReceive(Context context, Intent intent)
			{
				powerService.Update(intent);
			}
		}
	}
}

#endif