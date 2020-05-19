#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-01-01 11:36:50Z</CreationDate>
</File>
*/
#endregion

namespace Codon.Messaging
{
	/// <summary>
	/// This class is used as a base class for 
	/// cancellable messages.
	/// </summary>
	/// <typeparam name="TPayload">
	/// The type of data that accompanies the message
	/// when published.</typeparam>
	public abstract class CancellableMessageBase<TPayload>
		: MessageBase<TPayload>
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="sender">
		/// The object that sent the message.</param>
		/// <param name="payload">
		/// An object containing message information.</param>
		protected CancellableMessageBase(object sender, TPayload payload)
			: base(sender, payload)
		{
			/* Intentionally left blank. */
		}

		bool cancel;

		/// <summary>
		/// Allows the message to be cancelled
		/// so that other subscribers may see that
		/// it has been cancelled and not process the message.
		/// Note that setting this property to <c>true</c>
		/// does not prevent the message from being dispatched
		/// to other subscribers.
		/// Also not that, once set to <c>true</c>
		/// this property can not be set to <c>false</c>.
		/// </summary>
		public bool Cancel
		{
			get => cancel;
			set
			{
				if (cancel)
				{
					return;
				}
				cancel = value;
			}
		}
	}

	/// <summary>
	/// This class is used as a base class for 
	/// cancellable messages.
	/// </summary>
	public class CancellableMessageBase : CancellableMessageBase<object>
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="sender">
		/// The object that sent the message.</param>
		/// <param name="payload">
		/// An object containing message information.</param>
		public CancellableMessageBase(object sender, object payload = null) 
			: base(sender, payload)
		{
		}
	}
}