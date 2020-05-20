#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-01-01 11:50:21Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Calcium.InversionOfControl;
using Calcium.Networking;

namespace Calcium.Services
{
	/// <summary>
	/// This class allows you to monitor the status of the current network connection.
	/// When the connection changes an event is raised. In addition a <c>NetworkAvailabilityChangedMessage</c>
	/// is disptached using the <see cref="IMessenger"/> implementation if registered with the IoC container.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Networking)
		+ ".NetworkConnectionService, " + AssemblyConstants.ExtrasPlatformAssembly, Singleton = true)]
	public interface INetworkConnectionService
	{
		/// <summary>
		/// Gets a value indicating if the application 
		/// has a network connection for transmitting data.
		/// </summary>
		bool Connected { get; }

		/// <summary>
		/// The type of network connection. It can be 
		/// none, lan, or mobile broadband.
		/// </summary>
		NetworkConnectionType NetworkConnectionType { get; }

		/// <summary>
		/// This event is raised on the UI thread when the connection changes. 
		/// In addition, when using the default implementation, a <c>NetworkAvailabilityChangedMessage</c>
		/// is disptached using the <see cref="IMessenger"/> implementation if registered with the IoC container.
		/// </summary>
		event EventHandler<EventArgs> NetworkConnectionChanged;

		/// <summary>
		/// Gets a value indicating whether the mobile broadband account
		/// is approaching a data limit. In which case, usage should
		/// be minimized.
		/// </summary>
		bool ApproachingDataLimit { get; }

		/// <summary>
		/// Gets a value indicating whether the mobile broadband account
		/// is relying on a third-party telecom; which may mean increased
		/// data usage costs for the user. If <c>true</c>, usage should
		/// be minimized.
		/// </summary>
		bool Roaming { get; }

		/// <summary>
		/// Gets a value indicating whether application data usage should
		/// be minimized.
		/// </summary>
		bool LimitData { get; }

		/// <summary>
		/// Refreshes network information.
		/// </summary>
		void Update();

		/// <summary>
		/// The Service Set Identifier. 
		/// SSID is a case sensitive, 32 alphanumeric character 
		/// unique identifier attached to the header of packets 
		/// sent over a wireless local-area network (WLAN).
		/// </summary>
		string Ssid { get; }

#if __ANDROID__
		/// <summary>
		/// Gets the device IP address.
		/// </summary>
		string IPAddress { get; }
#endif
		/// <summary>
		/// Asynchronously retrieves a list of wireless networks.
		/// </summary>
		/// <returns>A list of networks visible to the device.</returns>
		Task<IEnumerable<WirelessNetwork>> GetWirelessNetworksAsync();

		/// <summary>
		/// Gets or sets whether the device is able
		/// to connect to wireless networks.
		/// </summary>
		bool WifiEnabled { get; set; }
	}
}
