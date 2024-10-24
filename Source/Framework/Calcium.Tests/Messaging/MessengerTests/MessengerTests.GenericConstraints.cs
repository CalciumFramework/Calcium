#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:43:12Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
		[Fact]
		public async Task ShouldHandleMultipleIMessageSubscriberInterfaces()
		{
			// Arrange
			Messenger messenger = new();
			MultiTypeMessageSubscriberMock subscriber = new();
			messenger.Subscribe(subscriber);

			MessageDefinition1 message1 = new() { Content = "Message 1" };
			MessageDefinition2 message2 = new() { Content = "Message 2" };

			// Act
			await messenger.PublishAsync(message1);
			await messenger.PublishAsync(message2);

			// Assert
			subscriber.ReceivedMessage1.Should().Be(message1);
			subscriber.ReceivedMessage2.Should().Be(message2);
		}

		class MultiTypeMessageSubscriberMock : IMessageSubscriber<MessageDefinition1>,
			IMessageSubscriber<MessageDefinition2>
		{
			public MessageDefinition1? ReceivedMessage1 { get; private set; }
			public MessageDefinition2? ReceivedMessage2 { get; private set; }

			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				ReceivedMessage1 = message;
				return Task.CompletedTask;
			}

			public Task ReceiveMessageAsync(MessageDefinition2 message)
			{
				ReceivedMessage2 = message;
				return Task.CompletedTask;
			}
		}

		class MessageDefinition2
		{
			public string? Content { get; set; }
		}

		[Fact]
		public async Task ShouldHandleGenericTypeConstraintsCorrectly()
		{
			// Arrange
			Messenger messenger = new();
			ConstrainedMessageSubscriberMock subscriber = new();
			messenger.Subscribe(subscriber);

			ConstrainedMessage validMessage = new() { Content = "Valid Message" };
			InvalidMessage invalidMessage = new();

			// Act
			await messenger.PublishAsync(validMessage);

			// Assert
			subscriber.ReceivedMessage.Should().Be(validMessage);

			// Ensure that invalidMessage (which doesn't satisfy the constraint) is not received
			var act = async () => await messenger.PublishAsync(invalidMessage);
			await act.Should().NotThrowAsync();
			subscriber.ReceivedMessage.Should().NotBe(invalidMessage); // Shouldn't be set
		}

		class ConstrainedMessageSubscriberMock : IMessageSubscriber<ConstrainedMessage>
		{
			public ConstrainedMessage? ReceivedMessage { get; private set; }

			public Task ReceiveMessageAsync(ConstrainedMessage message)
			{
				ReceivedMessage = message;
				return Task.CompletedTask;
			}
		}

		class ConstrainedMessage
		{
			public string? Content { get; set; }
		}

		class InvalidMessage
		{
		}
	}
}