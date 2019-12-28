#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2019, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2019-12-21 11:36:50Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

namespace Codon.Messaging
{
	/// <summary>
	/// This class allows subscription to the <c>IMessenger</c> implementation
	/// for the specified <c>TMessage</c>.
	/// It is intended to be used in places where implementing <c>IMessageSubscriber</c>
	/// on a parent class is deemed unnecessary.
	/// Please note that the default implementation of <c>IMessenger</c> (<see cref="Messenger"/>)
	/// maintains weak references to its subscribers.
	/// Therefore, when using the class, retain the instance as a field,
	/// otherwise the instance may be garbage collected and the subscription lost. 
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public class DelegateSubscriber<TMessage> : IMessageSubscriber<TMessage>
	{
		readonly Func<TMessage, Task> receiveMessageFunc;

		public DelegateSubscriber(Func<TMessage, Task> receiveMessageFunc)
		{
			this.receiveMessageFunc = receiveMessageFunc 
									?? throw new ArgumentNullException(nameof(receiveMessageFunc));
		}

		public Task ReceiveMessageAsync(TMessage message)
		{
			return receiveMessageFunc(message);
		}
	}
}
