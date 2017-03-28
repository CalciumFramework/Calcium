#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-10-29 17:45:28Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Codon.InversionOfControl;
using Codon.Messaging;

namespace Codon.Services
{
	/// <summary>
	/// This interface defines a pub/sub messaging system.
	/// Subscribers implement the <see cref="IMessageSubscriber{TEvent}"/> 
	/// interface for each message type they wish to receive.
	/// Subscribers then register themselves, with the <c>IMessenger</c>
	/// implementation, by calling <see cref="Subscribe"/>.
	/// When an object wishes to send a message to all objects
	/// implementing the <see cref="IMessageSubscriber{TEvent}"/>
	/// interface, it calls <see cref="PublishAsync{TEvent}"/>.
	/// </summary>
	[DefaultType(typeof(Messenger), Singleton = true)]
	public interface IMessenger
	{
		/// <summary>
		/// Notify the messenger that the specified object
		/// wishes to receive messages corresponding to that objects
		/// <see cref="IMessageSubscriber{TEvent}"/> implementations.
		/// If the specified subscriber does not implement
		/// <see cref="IMessageSubscriber{TEvent}"/> then the
		/// call to <see cref="Subscribe" /> is ignored.
		/// </summary>
		/// <param name="subscriber"></param>
		void Subscribe(object subscriber);

		/// <summary>
		/// Removes all subscriptions for the specified object.
		/// The subscriber no longer receives messages
		/// when <see cref="PublishAsync{TEvent}"/> is called.
		/// This call has no effect if the specified subscriber
		/// was not previously subscribed.
		/// </summary>
		/// <param name="subscriber">The subscriber object
		/// that was previously subscribed to receive
		/// messages using the <see cref="Subscribe"/> method.</param>
		void Unsubscribe(object subscriber);

		/// <summary>
		/// Asynchronously dispatches a message to all subscribers.
		/// </summary>
		/// <typeparam name="TEvent">
		/// Indicates the message type. Subscribers implementing 
		/// <see cref="IMessageSubscriber{TEvent}"/>
		/// receive the message.</typeparam>
		/// <param name="eventToPublish">
		/// The message payload that is received by subscribers.</param>
		/// <param name="requireUIThread">
		/// If <c>true</c> messages are dispatched using the UI thread.
		/// If <c>false</c> and the call to <c>PublishAsync</c>
		/// is made not from the UI thread, then messages are
		/// dispatched are dispatched on the current thread.</param>
		/// <param name="recipientType">
		/// Allows you to restrict messages to those subscribers
		/// deriving from this type.</param>
		/// <param name="memberName">
		/// The class member name of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="filePath">
		/// The file path of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="lineNumber">
		/// The line number of the call origin.
		/// Automatically populated by the compiler.</param>
		Task PublishAsync<TEvent>(
			TEvent eventToPublish,
			bool requireUIThread = false,
			Type recipientType = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath]string filePath = null,
			[CallerLineNumber]int lineNumber = 0);
	}
}