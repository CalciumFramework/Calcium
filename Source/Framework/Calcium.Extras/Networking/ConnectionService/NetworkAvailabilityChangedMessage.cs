#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-01-01 11:50:30Z</CreationDate>
</File>
*/
#endregion

using Codon.Messaging;

namespace Codon.Networking
{
	/// <summary>
	/// This message is dispatched via the <see cref="Services.IMessenger"/>
	/// and indicates that the network availability has changed.
	/// Perhaps there is now no network connection. Or perhaps
	/// the device now has a usable connection.
	/// <seealso cref="NetworkConnectionInfo"/>"/>
	/// </summary>
	public class NetworkAvailabilityChangedMessage 
		: MessageBase<NetworkConnectionInfo>
	{
		public NetworkAvailabilityChangedMessage(
			object sender, NetworkConnectionInfo payload)
			: base(sender, payload)
		{
		}
	}
}