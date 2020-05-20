#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2015-04-21 16:10:15Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.Messaging
{
	/// <summary>
	/// The base class for a message informing subscribers
	/// that a value is about to change.
	/// <seealso cref="ValueChangedMessageBase{TValue}"/>
	/// </summary>
	/// <typeparam name="TValue">
	/// The type of the value.</typeparam>
	public abstract class ValueChangingMessageBase<TValue>
	{
		/// <summary>
		/// The current value.
		/// </summary>
		public TValue CurrentValue { get; private set; }

		/// <summary>
		/// The value after the change occurs.
		/// </summary>
		public TValue NewValue { get; private set; }

		/// <summary>
		/// The originator of the change.
		/// </summary>
		public object Sender { get; private set; }

		/// <summary>
		/// Initializes a new instance of <c>ValueChangingMessageBase</c>
		/// </summary>
		/// <param name="sender">
		/// Commonly this is the initiator of the message. 
		/// Cannot be <c>null</c>.</param>
		/// <param name="currentValue">
		/// The current value.</param>
		/// <param name="newValue">
		/// What the new value will be if not cancelled.</param>
		protected ValueChangingMessageBase(
			object sender, TValue currentValue, TValue newValue)
		{
			Sender = AssertArg.IsNotNull(sender, nameof(sender));

			CurrentValue = currentValue;
			NewValue = newValue;
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
}
