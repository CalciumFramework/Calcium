#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:43:50Z</CreationDate>
</File>
*/
#endregion

using Calcium.Services;

using FluentAssertions;

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
		[Fact]
		public async Task ShouldDirectMessageToType()
		{
			IMessenger messenger = GetMessenger();

			MessageSubscriberMock1 m1 = new();
			messenger.Subscribe(m1);

			MessageSubscriberMock2 m2 = new();
			messenger.Subscribe(m2);

			MessageDefinition1 message1 = new();
			await messenger.PublishAsync(message1);

			m1.Message.Should().Be(message1);
			m2.Message.Should().Be(message1);

			MessageDefinition1 message2 = new();
			await messenger.PublishAsync(message2, recipientType: typeof(MessageSubscriberMock1));

			m1.Message.Should().Be(message2);
			m2.Message.Should().Be(message1);
		}

		[Fact]
		public async Task ShouldOnlySendMessageToSpecifiedRecipientType()
		{
			IMessenger messenger = GetMessenger();
			MessageSubscriberMock1 m1 = new();
			MessageSubscriberMock2 m2 = new();

			messenger.Subscribe(m1);
			messenger.Subscribe(m2);

			MessageDefinition1 message = new();

			// Publish the message with a specific recipient type
			await messenger.PublishAsync(message, recipientType: typeof(MessageSubscriberMock1));

			// Ensure only the specified recipient type (MessageSubscriberMock1) received the message
			m1.Message.Should().Be(message);
			m2.Message.Should().BeNull();
		}

		[Fact]
		public async Task ShouldSendMessageToBaseAndInheritedClasses()
		{
			IMessenger messenger = GetMessenger();
			BaseMessageSubscriberMock m1 = new();
			DerivedMessageSubscriberMock m2 = new();

			messenger.Subscribe(m1);
			messenger.Subscribe(m2);

			MessageDefinition1 message = new();

			// Publish the message with the base class type
			await messenger.PublishAsync(message, recipientType: typeof(BaseMessageSubscriberMock));

			// Ensure both base and derived class instances receive the message
			m1.Message.Should().Be(message);
			m2.Message.Should().Be(message);
		}

		class BaseMessageSubscriberMock : IMessageSubscriber<MessageDefinition1>
		{
			public MessageDefinition1? Message { get; set; }

			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				Message = message;
				return Task.CompletedTask;
			}
		}

		class DerivedMessageSubscriberMock : BaseMessageSubscriberMock
		{
			// Inherits behavior from BaseMessageSubscriberMock
		}
	}
}