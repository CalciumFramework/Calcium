#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:23:11Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.Messaging
{
	[TestClass]
	public class MessengerTests
	{
		[TestMethod]
		public async Task ShouldRegisterAndResolveTypes()
		{
			var messenger = new Messenger();
			var m1 = new MessageSubscriberMock1();
			messenger.Subscribe(m1);

			var message1 = new MessageDefinition1();
			await messenger.PublishAsync(message1);

			Assert.AreEqual(message1, m1.Message);

			messenger.Unsubscribe(m1);

			var message2 = new MessageDefinition1();
			await messenger.PublishAsync(message2);

			Assert.AreEqual(message1, m1.Message, 
				"Message should be the same as before the unsubscription.");
		}

		[TestMethod]
		public async Task ShouldDirectMessageToType()
		{
			var messenger = new Messenger();

			var m1 = new MessageSubscriberMock1();
			messenger.Subscribe(m1);

			var m2 = new MessageSubscriberMock2();
			messenger.Subscribe(m2);

			var message1 = new MessageDefinition1();
			await messenger.PublishAsync(message1);

			Assert.AreEqual(message1, m1.Message);
			Assert.AreEqual(message1, m2.Message);

			var message2 = new MessageDefinition1();
			await messenger.PublishAsync(message2, 
				recipientType:typeof(MessageSubscriberMock1));

			Assert.AreEqual(message2, m1.Message);
			Assert.AreEqual(message1, m2.Message);
		}

		class MessageSubscriberMock1 : IMessageSubscriber<MessageDefinition1>
		{
			public MessageDefinition1 Message { get; set; }

			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				Message = message;

				return Task.CompletedTask;
			}
		}

		class MessageSubscriberMock2 : IMessageSubscriber<MessageDefinition1>
		{
			public MessageDefinition1 Message { get; set; }

			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				Message = message;

				return Task.CompletedTask;
			}
		}

		class MessageDefinition1
		{
		}
	}
}
