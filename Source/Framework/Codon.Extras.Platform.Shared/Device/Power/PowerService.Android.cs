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
using Codon.Concurrency;

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
				var synchronizationContext = Dependency.Resolve<ISynchronizationContext>();

				synchronizationContext.Send(() => { 
					lock (listeningLock)
					{
						if (!listening)
						{
							var intentFilter = new IntentFilter(/*Intent.ActionBatteryChanged*/);
							intentFilter.AddAction(Intent.ActionBatteryLow);
							intentFilter.AddAction(Intent.ActionBatteryOkay);

							intentFilter.AddAction(Intent.ActionPowerConnected);
							intentFilter.AddAction(Intent.ActionPowerDisconnected);

							//var context = Application.Context;
							var context = Dependency.Resolve<Activity>();
							receiver = new PowerConnectionReceiver(this);
							batteryStatusIntent = context.RegisterReceiver(receiver, intentFilter);

							listening = true;
						}
					}
				});
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
						//var context = Application.Context;
						var context = Dependency.Resolve<Activity>();
						context.UnregisterReceiver(receiver);

						listening = false;
					}
				}
			}
		}

		DevicePowerSource powerSource;

		public DevicePowerSource PowerSource
		{
			get
			{
				if (batteryStatusIntent == null)
				{
					return powerSource;
				}
				powerSource = GetPowerSource(batteryStatusIntent);
				return powerSource;
			}
			set => Set(ref powerSource, value);
		}

		int chargeRemainingPercent;

		public int RemainingBatteryChargePercent
		{
			get
			{
				if (batteryStatusIntent == null)
				{
					return chargeRemainingPercent;
				}
				chargeRemainingPercent = (int)GetRemainingBatteryLevel(batteryStatusIntent);

				return chargeRemainingPercent;
			}
			set => Set(ref chargeRemainingPercent, value);
		}

		/// <summary>
		/// We make some assumptions to return a value 
		/// that indicates how long the battery has enough charge to power the device.
		/// </summary>
		/// <exception cref="NotSupportedException" />
		public TimeSpan RemainingBatteryDischargeTime
		{
			get
			{
				TimeSpan remainingTime = EstimateRemainingTime(RemainingBatteryChargePercent);
				return remainingTime;
			}
		}

		TimeSpan EstimateRemainingTime(int chargePercent)
		{
			/* For now estimate remaining time using an estimated 4 hours on full load. */
			TimeSpan remainingTime = TimeSpan.FromHours(4.0 * (chargePercent / 100.0));
			return remainingTime;
		}

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
			batteryStatusIntent = intent;

			var oldPowerSource = powerSource;
			RemainingBatteryChargePercent = (int)GetRemainingBatteryLevel(intent);
			
			PowerSource = GetPowerSource(intent);
			//BatteryStatus status = (BatteryStatus)intent.GetIntExtra(BatteryManager.ExtraStatus, -1);
			//			bool isCharging = status == BatteryStatus.Charging ||
			//								status == BatteryStatus.Full;

			string intentAction = intent.Action;
			if (intentAction == Intent.ActionPowerConnected || intentAction == Intent.ActionPowerDisconnected)
			{
				var messenger = Dependency.Resolve<IMessenger>();
				messenger.PublishAsync(new PowerSourceChangeEvent(this, oldPowerSource, PowerSource));
			}
			else if (intentAction == Intent.ActionBatteryLow || intentAction == Intent.ActionBatteryOkay)
			{
				var messenger = Dependency.Resolve<IMessenger>();
				TimeSpan remainingTime = EstimateRemainingTime(chargeRemainingPercent);
				messenger.PublishAsync(new RemainingBatteryChargeEvent(this, chargeRemainingPercent, remainingTime));
			}
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