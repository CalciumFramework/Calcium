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

// ReSharper disable ExplicitCallerInfoArgument

using Calcium.Services;

using FluentAssertions;

using Xunit.Abstractions;

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
		readonly ITestOutputHelper testOutputHelper;

		public MessengerTests(ITestOutputHelper testOutputHelper)
		{
			this.testOutputHelper = testOutputHelper;
		}

		IMessenger GetMessenger() => new Messenger();

		[Fact]
		public async Task ShouldRegisterAndResolveTypes()
		{
			IMessenger messenger = GetMessenger();
			MessageSubscriberMock1 m1 = new();
			messenger.Subscribe(m1);

			MessageDefinition1 message1 = new();
			await messenger.PublishAsync(message1);

			m1.Message.Should().Be(message1);

			messenger.Unsubscribe(m1);

			MessageDefinition1 message2 = new();
			await messenger.PublishAsync(message2);

			m1.Message.Should().Be(message1,
				"Message should be the same as before the unsubscription.");
		}

		class MessageSubscriberMock1 : IMessageSubscriber<MessageDefinition1>
		{
			public MessageDefinition1? Message { get; set; }

			public Action<MessageDefinition1>? OnMessageReceived { get; set; }

			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				Message = message;

				OnMessageReceived?.Invoke(message);

				return Task.CompletedTask;
			}
		}

		class MessageSubscriberMock2 : IMessageSubscriber<MessageDefinition1>
		{
			public MessageDefinition1? Message { get; set; }

			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				Message = message;
				return Task.CompletedTask;
			}
		}

		class MessageDefinition1
		{
			public string? Content { get; set; }
		}

		[Fact]
		public async Task ShouldNotThrowWhenPublishingWithoutSubscribers()
		{
			// Arrange
			IMessenger messenger = GetMessenger();

			MessageDefinition1 message = new();

			// Act & Assert
			var act = async () => await messenger.PublishAsync(message);

			await act.Should().NotThrowAsync();
		}
	}
}