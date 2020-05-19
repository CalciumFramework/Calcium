#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2015-04-21 16:18:54Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.Messaging
{
	/// <summary>
	/// The base class for a message informing subscribers
	/// that a value has changed.
	/// <seealso cref="ValueChangingMessageBase{TValue}"/>
	/// </summary>
	/// <typeparam name="TValue">
	/// The type of the value.</typeparam>
	public abstract class ValueChangedMessageBase<TValue>
	{
		/// <summary>
		/// The value before the change occurred.
		/// </summary>
		public TValue OldValue { get; private set; }

		/// <summary>
		/// The value after the change.
		/// </summary>
		public TValue NewValue { get; private set; }

		/// <summary>
		/// The originator of the change.
		/// </summary>
		public object Sender { get; private set; }

		/// <summary>
		/// Initializes a new instance of <c>ValueChangedMessageBase</c>
		/// </summary>
		/// <param name="sender">
		/// Commonly this is the initiator of the message. 
		/// Cannot be <c>null</c>.</param>
		/// <param name="oldValue">The previous value.</param>
		/// <param name="newValue">The new value.</param>
		protected ValueChangedMessageBase(
			object sender, TValue oldValue, TValue newValue)
		{
			Sender = AssertArg.IsNotNull(sender, nameof(sender));

			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
