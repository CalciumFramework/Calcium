#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:43:32Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
		[Fact]
		public async Task ShouldSubscribeToMultipleMessageTypes()
		{
			Messenger messenger = new();
			MultipleMessageSubscriberMock m1 = new();

			messenger.Subscribe(m1);

			MessageDefinition1 message1 = new();
			MessageDefinition2 message2 = new();

			await messenger.PublishAsync(message1);
			await messenger.PublishAsync(message2);

			m1.Message1.Should().Be(message1);
			m1.Message2.Should().Be(message2);
		}

		[Fact]
		public async Task ShouldSendMessagesToMultipleSubscribersOfSameType()
		{
			Messenger messenger = new();
			MessageSubscriberMock1 m1 = new();
			MessageSubscriberMock1 m2 = new();

			messenger.Subscribe(m1);
			messenger.Subscribe(m2);

			MessageDefinition1 message = new();

			await messenger.PublishAsync(message);

			m1.Message.Should().Be(message);
			m2.Message.Should().Be(message);
		}

		class MultipleMessageSubscriberMock : IMessageSubscriber<MessageDefinition1>,
			IMessageSubscriber<MessageDefinition2>
		{
			public MessageDefinition1? Message1 { get; set; }
			public MessageDefinition2? Message2 { get; set; }

			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				Message1 = message;
				return Task.CompletedTask;
			}

			public Task ReceiveMessageAsync(MessageDefinition2 message)
			{
				Message2 = message;
				return Task.CompletedTask;
			}
		}
	}
}