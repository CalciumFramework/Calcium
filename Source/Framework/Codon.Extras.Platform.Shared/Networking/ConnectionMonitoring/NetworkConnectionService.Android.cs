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
	<CreationDate>2013-01-01 11:32:08Z</CreationDate>
</File>
*/
#endregion

using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Codon.ApplicationModel;
using Codon.ComponentModel;
using Codon.Logging;
using Codon.Messaging;
using Codon.Services;

namespace Codon.Networking
{
	/// <summary>
	/// Android implementation of <see cref="INetworkConnectionService"/>.
	/// </summary>
	public class NetworkConnectionService : ObservableBase, 
		INetworkConnectionService, IMessageSubscriber<ApplicationLifeCycleMessage>
	{
		const int DefaultSampleRateMs = 1000;
		WifiReceiver wifiReceiver;
		int sampleRateMs;

		public int SampleRateMs => sampleRateMs;

		bool approachingDataLimit;

		public bool ApproachingDataLimit
		{
			get => approachingDataLimit;
			private set
			{
				bool temp = LimitData;
				if (Set(ref approachingDataLimit, value) 
							== AssignmentResult.Success)
				{
					if (temp != LimitData)
					{
						OnPropertyChanged(nameof(LimitData));
					}
				}
			}
		}

		bool roaming;

		public bool Roaming
		{
			get => roaming;
			private set
			{
				bool temp = LimitData;
				if (Set(ref roaming, value) == AssignmentResult.Success)
				{
					if (temp != LimitData)
					{
						OnPropertyChanged(nameof(LimitData));
					} 
				}
			}
		}

		public virtual bool LimitData => Roaming || ApproachingDataLimit;

		public NetworkConnectionType NetworkConnectionType { get; private set; }
		public event EventHandler<EventArgs> NetworkConnectionChanged;

		bool connected;

		public bool Connected
		{
			get => connected;
			set => Set(ref connected, value);
		}

		bool IsConnected(ConnectivityManager connectivityManager)
		{
			NetworkInfo info = connectivityManager.ActiveNetworkInfo;
			return info != null && info.IsConnected;
		}

		public NetworkConnectionService(int sampleRateMs = DefaultSampleRateMs)
		{
			this.sampleRateMs = sampleRateMs;

			UpdateInBackground();

			RegisterReceiver();

			var messenger = Dependency.Resolve<IMessenger>();
			messenger.Subscribe(this);
		}

		/// <summary>
		/// If this WiFi receiver is not registered in the application manifest,
		/// call this method to subscribe to network events.
		/// </summary>
		public void RegisterReceiver()
		{
			try
			{
				wifiReceiver = new WifiReceiver(this);
				var context = Application.Context;
				var filter = new IntentFilter(WifiManager.NetworkStateChangedAction);
				filter.AddAction(WifiManager.ScanResultsAvailableAction);
				context.RegisterReceiver(wifiReceiver, filter);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to start NetworkConnectionService. " +
									"Please ensure that you have set the permission in your manifest. " +
									"<uses-permission android:name=\"android.permission.ACCESS_NETWORK_STATE\" />", ex);
			}
		}

		/// <summary>
		/// Stops the service from receiving network connection events.
		/// </summary>
		public void UnregisterReceiver()
		{
			if (wifiReceiver != null)
			{
				var context = Application.Context;
				context.UnregisterReceiver(wifiReceiver);
			}
		}

		internal void HandleNetworkStatusChanged()
		{
			UpdateInBackground();

			UIContext.Instance.Post(SendConnectionChangedEvents);
		}

		void SendConnectionChangedEvents()
		{
			NetworkConnectionChanged?.Invoke(this, EventArgs.Empty);

			if (Dependency.TryResolve<IMessenger>(out IMessenger messenger))
			{
				messenger.PublishAsync(new NetworkAvailabilityChangedMessage(
					this, new NetworkConnectionInfo(NetworkConnectionType, LimitData)), true);
			}
		}

		string ssid;

		public string Ssid
		{
			get => ssid;
			set => Set(ref ssid, value);
		}

		string GetSsid(WifiManager wifiManager = null)
		{
			if (wifiManager == null)
			{
				var context = Application.Context;
				wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
			}

			WifiInfo wifiInfo = wifiManager.ConnectionInfo;
			var result = wifiInfo.SSID;

			return result;
		}

		void UpdateInBackground()
		{
			Task.Run(() =>
			{
				try
				{
					Update();
				}
				catch (Exception ex)
				{
					var log = Dependency.Resolve<ILog>();
					log.Error("Error raised updating network connection service.", ex);
				}
			});
		}

		public void Update()
		{
			var context = Application.Context;
			var wifi = (WifiManager)context.GetSystemService(Context.WifiService);
			ConnectivityManager cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
			Connected = IsConnected(cm);
			NetworkInfo info = cm.ActiveNetworkInfo;
			Roaming = info?.IsRoaming ?? false;
			Ssid = GetSsid(wifi);
			IPAddress = GetIPAddress();

			if (info != null && info.IsConnected)
			{
				if (info.Type == ConnectivityType.Wifi)
				{
					NetworkConnectionType = NetworkConnectionType.Lan;
				}
				else
				{
					NetworkConnectionType = NetworkConnectionType.MobileBroadband;
				}
			}
			else
			{
				NetworkConnectionType = NetworkConnectionType.None;
			}
		}

		string GetIPAddress()
		{
			IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
			string ipAddress = null;
			if (addresses != null && addresses[0] != null)
			{
				ipAddress = addresses[0].ToString();
			}

			return ipAddress;
		}

		string iPAddress;

		public string IPAddress
		{
			get => iPAddress;
			private set => Set(ref iPAddress, value);
		}

		readonly IList<WirelessNetwork> wiFiNetworks = new List<WirelessNetwork>();

		public Task<IEnumerable<WirelessNetwork>> GetWirelessNetworksAsync()
		{
			var tcs = new TaskCompletionSource<IEnumerable<WirelessNetwork>>();

			void HandleWifiScanComplete(object sender, EventArgs args)
			{
				WifiScanComplete -= HandleWifiScanComplete;

				tcs.TrySetResult(wiFiNetworks);
			}

			var wifi = (WifiManager)Activity.GetSystemService(Context.WifiService);

			WifiScanComplete += HandleWifiScanComplete;
			wifi.StartScan();

			return tcs.Task;
		}

		Activity Activity => Dependency.Resolve<Activity>();

		internal void HandleScanResultsAvailable()
		{
			var wifi = (WifiManager)Activity.GetSystemService(Context.WifiService);
			IList<ScanResult> scanResults = wifi.ScanResults;
			wiFiNetworks.Clear();
			if (scanResults != null)
			{
				foreach (ScanResult scanResult in scanResults)
				{
					wiFiNetworks.Add(ConvertToWirelessNetwork(scanResult));
				}
			}

			WifiScanComplete?.Invoke(this, EventArgs.Empty);
		}

		event EventHandler<EventArgs> WifiScanComplete;

		internal static WirelessNetwork ConvertToWirelessNetwork(ScanResult scanResult)
		{
			var result = new WirelessNetwork
			{
				Bssid = scanResult.Bssid,
				Ssid = scanResult.Ssid,
				Capabilities = scanResult.Capabilities,
				IsPasspointNetwork = scanResult.IsPasspointNetwork,
				OperatorFriendlyName = scanResult.OperatorFriendlyName?.ToString(),
				Level = scanResult.Level
			};

			return result;
		}

		WifiManager WifiManager
		{
			get
			{
				var context = Application.Context;
				var result = (WifiManager)context.GetSystemService(Context.WifiService);
				return result;
			}
		}

		public bool WifiEnabled
		{
			get => WifiManager.IsWifiEnabled;
			set => WifiManager.SetWifiEnabled(value);
		}

		void DetectConnectionStatusChange()
		{
			Task.Run(() =>
			{
				try
				{
					var temp = connected;
					Update();
					if (temp != connected)
					{
						UIContext.Instance.Post(SendConnectionChangedEvents);
					}
				}
				catch (Exception ex)
				{
					var log = Dependency.Resolve<ILog>();
					log.Error("Exception raised within DetectConnectionStatusChange." ,ex);
				}
			});
		}

		Task IMessageSubscriber<ApplicationLifeCycleMessage>.ReceiveMessageAsync(ApplicationLifeCycleMessage message)
		{
			var sd = message.State;
			switch (sd)
			{
				case ApplicationLifeCycleState.Launching:
				case ApplicationLifeCycleState.Activated:
					RegisterReceiver();
					DetectConnectionStatusChange();
					break;
				case ApplicationLifeCycleState.Closing:
				case ApplicationLifeCycleState.Deactivated:
					UnregisterReceiver();
					break;
			}

			return Task.CompletedTask;
		}
	}

	public class WifiReceiver : BroadcastReceiver
	{
		NetworkConnectionService networkConnectionService;

		public WifiReceiver(NetworkConnectionService networkConnectionService)
		{
			this.networkConnectionService = AssertArg.IsNotNull(networkConnectionService, nameof(networkConnectionService));
		}

		public override void OnReceive(Context context, Intent intent)
		{
			if (intent.Action == WifiManager.NetworkStateChangedAction)
			{
				networkConnectionService.HandleNetworkStatusChanged();
			}
			else if (intent.Action == WifiManager.ScanResultsAvailableAction)
			{
				networkConnectionService.HandleScanResultsAvailable();
			}

			//var wifi = (WifiManager)context.GetSystemService(Context.WifiService);
			//var state = wifi.WifiState;
			//			int wifiState = intent.GetIntExtra(WifiManager.ExtraWifiState, WifiState.Unknown);
			//		string wifiStateText = "No State";
			//
			//		switch (wifiState)
			//		{
			//			case WifiManager.WifiStateDisabling:
			//				wifiStateText = "WIFI_STATE_DISABLING";
			//				break;
			//			case WifiManager.WIFI_STATE_DISABLED:
			//				wifiStateText = "WIFI_STATE_DISABLED";
			//				break;
			//			case WifiManager.WIFI_STATE_ENABLING:
			//				wifiStateText = "WIFI_STATE_ENABLING";
			//				break;
			//			case WifiManager.WIFISTATEENABLED:
			//				wifiStateText = "WIFI_STATE_ENABLED";
			//				break;
			//			case WifiManager.WifiStateUnknown:
			//				wifiStateText = "WIFI_STATE_UNKNOWN";
			//				break;
			//			default:
			//				break;
		}
	}
}
#endif