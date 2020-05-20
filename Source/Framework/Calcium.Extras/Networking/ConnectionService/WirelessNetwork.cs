namespace Calcium.Services
{
	/// <summary>
	/// Represents a scan result of local networks that a device can potentially connect to.
	/// </summary>
	//[Serializable]
	public class WirelessNetwork
	{
		/// <summary>
		/// The address of the access point.
		/// </summary>
		public string Bssid { get; set; }

		/// <summary>
		/// The Service Set Identifier. SSID is a case sensitive,
		/// 32 alphanumeric character unique identifier attached to the header of packets
		/// sent over a wireless local-area network(WLAN).
		/// </summary>
		public string Ssid { get; set; }

		/// <summary>
		/// Describes the authentication, key management
		/// and encryption schemes supported by the access point.
		/// </summary>
		public string Capabilities { get; set; }

		/// <summary>
		/// Gets a value indicating if this network is a passpoint network,
		/// which is an improved method for connecting to Wi-Fi hotspots
		/// from the Wi-Fi Alliance. Authentication is performed automatically
		/// and silently by the compliant mobile device and hotspot
		/// without the user having to do anything.
		/// </summary>
		public bool IsPasspointNetwork { get; set; }

		/// <summary>
		/// The detected signal level in dBm, also known as the RSSI.
		/// </summary>
		public int Level { get; set; }

		/// <summary>
		/// The user readable name of the network.
		/// The name can be up to 64 alphanumeric characters,
		/// and can include special characters and spaces.
		/// If the name includes quotation marks ("),
		/// include a backslash character (\) before each quotation mark. (e.g. \"example\")
		/// </summary>
		public string OperatorFriendlyName { get; set; }
	}
}
