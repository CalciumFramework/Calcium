#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-01-01 11:35:56Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.Messaging
{
	/// <summary>
	/// The base class for a message that 
	/// supplies a single payload object 
	/// and has a known sender.
	/// </summary>
	/// <typeparam name="TPayload">
	/// The type of the payload, which accompanies 
	/// the message when it is published.</typeparam>
	public abstract class MessageBase<TPayload>
	{
		/// <summary>
		/// Associated information that accompanies
		/// the message when it is published.
		/// </summary>
		public TPayload Payload { get; private set; }

		/// <summary>
		/// The origin of the message.
		/// </summary>
		public object Sender { get; private set; }

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="sender">
		/// The publisher of the message.</param>
		/// <param name="payload">
		/// Associated information that accompanies
		/// the message when it is published.
		/// Can be <c>null</c>.</param>
		protected MessageBase(object sender, TPayload payload)
		{
			Sender = sender;
			Payload = payload;
		}
	}

	/// <summary>
	/// The base class for a message that 
	/// supplies a single payload object 
	/// and has a known sender.
	/// </summary>
	public abstract class MessageBase : MessageBase<object>
	{
		public MessageBase(object sender, object payload = null) 
			: base(sender, payload)
		{
		}
	}
}
