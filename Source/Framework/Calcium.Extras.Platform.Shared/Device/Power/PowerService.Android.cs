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
	<CreationDate>2017-04-06 12:51:47Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Calcium.ApplicationModel;
using Calcium.ComponentModel;
using Calcium.Device;
using Calcium.Services;
using Calcium.Concurrency;
using Calcium.Devices;
using Calcium.Logging;
using Calcium.Messaging;

namespace Calcium.Device
{
	/// <summary>
	/// Android implementation of the <see cref="IPowerService"/>.
	/// </summary>
	public class PowerService : ObservableBase, IPowerService, IMessageSubscriber<ApplicationLifeCycleMessage>
	{
		PowerConnectionReceiver receiver;
		bool listening;
		const double batteryFullChargeHours = 8.0;

		public void Start()
		{
			if (!listening)
			{
				stoppedByUserCode = false;

				var intentFilter = new IntentFilter();
				intentFilter.AddAction(Intent.ActionBatteryLow);
				intentFilter.AddAction(Intent.ActionBatteryOkay);

				intentFilter.AddAction(Intent.ActionPowerConnected);
				intentFilter.AddAction(Intent.ActionPowerDisconnected);
				intentFilter.AddAction(Intent.ActionBatteryChanged);

				var context = Dependency.Resolve<Activity>();
				receiver = new PowerConnectionReceiver(this);
				context.RegisterReceiver(receiver, intentFilter);

				listening = true;
			}
		}

		bool stoppedByUserCode;

		public void Stop()
		{
			stoppedByUserCode = true;
			StopCore();
		}

		void StopCore()
		{
			if (listening)
			{
				var context = Dependency.Resolve<Activity>();
				context.UnregisterReceiver(receiver);

				listening = false;
			}
		}

		DevicePowerSource powerSource;

		public DevicePowerSource PowerSource
		{
			get => powerSource;
			set => Set(ref powerSource, value);
		}

		int remainingBatteryChargePercent;

		public int RemainingBatteryChargePercent
		{
			get => remainingBatteryChargePercent;
			set => Set(ref remainingBatteryChargePercent, value);
		}

		float GetRemainingBatteryPercent(Intent intent)
		{
			if (intent == null)
			{
				return remainingBatteryChargePercent;
			}

			int scale = intent.GetIntExtra(BatteryManager.ExtraScale, -1);
			if (scale == -1)
			{
				return remainingBatteryChargePercent;
			}

			int level = intent.GetIntExtra(BatteryManager.ExtraLevel, -1);
			
			remainingBatteryChargePercent = (int)((level / (float)scale) * 100);

			return remainingBatteryChargePercent;
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
			/* For now estimate remaining time using an estimated batteryFullChargeHours hours on full load. */
			TimeSpan remainingTime = TimeSpan.FromHours(batteryFullChargeHours * (chargePercent / 100.0));
			return remainingTime;
		}

		/* This doesn't appear to work. Instead, we rely on the Intent.Action to indicate the power source. */
		//DevicePowerSource GetPowerSource(Intent intent)
		//{
		//	BatteryPlugged chargePlug = (BatteryPlugged)intent.GetIntExtra(BatteryManager.ExtraPlugged, -1);
		//
		//	bool externalPower = chargePlug == BatteryPlugged.Usb 
		//		|| chargePlug == BatteryPlugged.Ac 
		//		|| chargePlug == BatteryPlugged.Wireless;
		//
		//	var result = externalPower ? DevicePowerSource.External : DevicePowerSource.Battery;
		//
		//	var log = Dependency.Resolve<ILog>();
		//	log.Error("Power source is: " + chargePlug);
		//
		//	return result;
		//}

		BatteryStatus GetBatteryStatus(Intent intent)
		{
			BatteryStatus status = (BatteryStatus)intent.GetIntExtra(BatteryManager.ExtraStatus, -1);
			return status;
		}

		BatteryState batteryState;

		public BatteryState BatteryState
		{
			get => batteryState;
			private set => Set(ref batteryState, value);
		}

		void Update(Intent intent)
		{
			string intentAction = intent?.Action;
			
			if (intentAction == Intent.ActionPowerConnected || intentAction == Intent.ActionPowerDisconnected)
			{
				var oldPowerSource = powerSource;

				if (intentAction == Intent.ActionPowerConnected)
				{
					PowerSource = DevicePowerSource.External;
				}
				else if (intentAction == Intent.ActionPowerDisconnected)
				{
					PowerSource = DevicePowerSource.Battery;
				}
			
				var messenger = Dependency.Resolve<IMessenger>();
				messenger.PublishAsync(new PowerSourceChangeEvent(this, oldPowerSource, PowerSource), true);
			}
			else if (intentAction == Intent.ActionBatteryLow || intentAction == Intent.ActionBatteryOkay
															|| intentAction == Intent.ActionBatteryChanged)
			{
				bool sendMessage = true;
				if (intentAction == Intent.ActionBatteryChanged)
				{
					var newValue = (int)GetRemainingBatteryPercent(intent);
					/* Only send the message if the change is greater than 1%.
					 * This is because when charging, the value goes up and down. */
					if (Math.Abs(remainingBatteryChargePercent - newValue) < 2)
					{
						sendMessage = false;
					}

					RemainingBatteryChargePercent = newValue;
					var status = GetBatteryStatus(intent);
					BatteryState = status.ToBatteryState();
				}

				TimeSpan remainingTime = EstimateRemainingTime(remainingBatteryChargePercent);

				if (sendMessage)
				{
					var messenger = Dependency.Resolve<IMessenger>();
					messenger.PublishAsync(new RemainingBatteryChargeEvent(this, remainingBatteryChargePercent, remainingTime), true);
				}
			}
		}

		public class PowerConnectionReceiver : BroadcastReceiver
		{
			readonly PowerService powerService;

			public PowerConnectionReceiver(PowerService powerService)
			{
				this.powerService = powerService;
			}

			public override void OnReceive(Context context, Intent intent)
			{
				powerService.Update(intent);
			}
		}

		Task IMessageSubscriber<ApplicationLifeCycleMessage>.ReceiveMessageAsync(ApplicationLifeCycleMessage message)
		{
			switch (message.State)
			{
				case ApplicationLifeCycleState.Closing:
				case ApplicationLifeCycleState.Deactivated:
					StopCore();
					break;
				case ApplicationLifeCycleState.Launching:
				case ApplicationLifeCycleState.Activated:
					if (!stoppedByUserCode)
					{
						Start();
					}
					break;
			}

			return Task.CompletedTask;
		}
	}
}

namespace Calcium.Devices
{
	static class BatteryStatusExtensions
	{
		internal static BatteryState ToBatteryState(this BatteryStatus status)
		{
			switch (status)
			{
				case BatteryStatus.Charging:
					return BatteryState.Charging;
				case BatteryStatus.Discharging:
					return BatteryState.Discharging;
				case BatteryStatus.Full:
					return BatteryState.Full;
				case BatteryStatus.NotCharging:
					return BatteryState.NotCharging;
				case BatteryStatus.Unknown:
					return BatteryState.Unknown;
				default:
					var log = Dependency.Resolve<ILog>();
					log.Warn("Unknown BatteryStatus value: " + status);
					return BatteryState.Unknown;
			}
		}
	}
}

#endif
