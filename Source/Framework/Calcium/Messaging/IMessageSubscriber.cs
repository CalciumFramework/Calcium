#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-10-29 17:45:33Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;

using Calcium.Services;

namespace Calcium.Messaging
{
	/// <summary>
	/// Implement this interface on a class that needs
	/// to receive notifications from the <see cref="IMessenger"/>.
	/// The generic type parameter is used to identify the type of message.
	/// When the <see cref="IMessenger.PublishAsync{TEvent}"/>
	/// is called, all implementations 
	/// of <c>IMessageSubscriber&lt;TEvent&gt;</c> receives the message.
	/// Please note that a class must subscribe to receive message
	/// using the <see cref="IMessenger.Subscribe"/> method.
	/// </summary>
	/// <typeparam name="TEvent">
	/// Used to identify the type of message.</typeparam>
	public interface IMessageSubscriber<TEvent>
	{
		/// <summary>
		/// When the <see cref="IMessenger.PublishAsync{TEvent}"/>
		/// is called, this method is called.
		/// </summary>
		/// <param name="message">The message payload.</param>
		Task ReceiveMessageAsync(TEvent message);
	}
}
