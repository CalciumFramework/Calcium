#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-01-01 11:46:31Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.Networking
{
	/// <summary>
	/// Provides information about the network connection,
	/// such as if a usable connection is available,
	/// and whether is a LAN or mobile broadband connection.
	/// </summary>
	public class NetworkConnectionInfo
	{
		/// <summary>
		/// The type of network connection. It can be 
		/// none, lan, or mobile broadband.
		/// </summary>
		public NetworkConnectionType NetworkConnectionType { get; private set; }

		/// <summary>
		/// Gets a value indicating whether application data usage should
		/// be minimized.
		/// </summary>
		public bool LimitData { get; private set; }

		public NetworkConnectionInfo(
			NetworkConnectionType networkConnectionType, bool limitData)
		{
			NetworkConnectionType = networkConnectionType;
			LimitData = limitData;
		}
	}
}
