#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:43:25Z</CreationDate>
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
		public async Task ShouldReceiveMessagesInOrderPublished()
		{
			// Arrange
			IMessenger messenger = GetMessenger();
			MessageSubscriberMock1 subscriber = new();
			messenger.Subscribe(subscriber);

			MessageDefinition1 message1 = new() { Content = "Message 1" };
			MessageDefinition1 message2 = new() { Content = "Message 2" };
			MessageDefinition1 message3 = new() { Content = "Message 3" };

			List<string> receivedMessages = new();

			subscriber.OnMessageReceived = message =>
										   {
											   message.Content.Should().NotBeNull();
											   receivedMessages.Add(message.Content!);
										   };

			// Act
			await messenger.PublishAsync(message1);
			await messenger.PublishAsync(message2);
			await messenger.PublishAsync(message3);

			// Assert
			receivedMessages.Should().ContainInOrder("Message 1", "Message 2", "Message 3");
		}
	}
}