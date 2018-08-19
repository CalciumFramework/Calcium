namespace Codon.Services
{
	/// <summary>
	/// Represents a scan result of local networks that a device can potentially connect to.
	/// </summary>
	//[Serializable]
	public class WirelessNetwork
	{
		public string Bssid { get; set; }
		public string Ssid { get; set; }
		public string Capabilities { get; set; }
		public bool IsPasspointNetwork { get; set; }
		public int Level { get; set; }
		public string OperatorFriendlyName { get; set; }
	}
}