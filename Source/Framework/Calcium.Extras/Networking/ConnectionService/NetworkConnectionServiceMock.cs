#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-01-01 13:51:46Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Calcium.Services;

namespace Calcium.Networking
{
	/// <summary>
	/// This class is an implementation of 
	/// <see cref="INetworkConnectionService"/>
	/// designed for unit testing purposes.
	/// </summary>
	public class NetworkConnectionServiceMock : INetworkConnectionService
	{
		NetworkConnectionType networkConnectionType = NetworkConnectionType.Lan;

		public event EventHandler<EventArgs> NetworkConnectionChanged;

		public bool ApproachingDataLimit { get; set; }
		public bool Roaming { get; set; }
		public bool LimitData { get; set; }

		public void Update()
		{
			/* Intentionally left blank. */
		}

		public string Ssid { get; set; }

		public Task<IEnumerable<WirelessNetwork>> GetWirelessNetworksAsync()
		{
			return Task.FromResult((IEnumerable<WirelessNetwork>)WirelessNetworks);
		}

		public bool WifiEnabled { get; set; }

		public IList<WirelessNetwork> WirelessNetworks { get; } = new List<WirelessNetwork>();

		public virtual bool Connected 
			=> networkConnectionType != NetworkConnectionType.None;

		public virtual NetworkConnectionType NetworkConnectionType
		{
			get => networkConnectionType;
			set
			{
				if (networkConnectionType != value)
				{
					networkConnectionType = value;

					UIContext.Instance.Post(() =>
					{
						NetworkConnectionChanged?.Invoke(this, EventArgs.Empty);
						
						var info = new NetworkConnectionInfo(NetworkConnectionType, LimitData);
						var message = new NetworkAvailabilityChangedMessage(this, info);

						IMessenger messenger = Dependency.Resolve<IMessenger>();
						messenger.PublishAsync(message);
					});
				}
			}
		}

#if __ANDROID__

		public string IPAddress { get; set; }
#endif
	}
}
