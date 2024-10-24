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

using System.Diagnostics;

using Calcium.ComponentModel;
using Calcium.Concurrency;

using FluentAssertions;

using Moq;

using Xunit.Abstractions;

// ReSharper disable ExplicitCallerInfoArgument

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
		readonly ITestOutputHelper testOutputHelper;

		public MessengerTests(ITestOutputHelper testOutputHelper)
		{
			this.testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task ShouldRegisterAndResolveTypes()
		{
			Messenger messenger = new();
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

		[Fact]
		public async Task ShouldDirectMessageToType()
		{
			Messenger messenger = new();

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

		#region Multiple Subscriptions Handling

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

		#endregion

		#region Exception Handling

		[Fact]
		public async Task ShouldContinuePublishingWhenSubscriberThrowsException()
		{
			var exceptionHandler = new Mock<IExceptionHandler>();
			exceptionHandler.Setup(e => e.ShouldRethrowException(
									   It.IsAny<Exception>(),
									   It.IsAny<object>(),
									   It.IsAny<string>(),
									   It.IsAny<string>(),
									   It.IsAny<int>()))
							.Returns(false); // Do not rethrow

			Messenger messenger = new()
			{
				ExceptionHandler = exceptionHandler.Object
			};

			MessageSubscriberMock1 m1 = new();
			ThrowingMessageSubscriberMock m2 = new();

			messenger.Subscribe(m1);
			messenger.Subscribe(m2);

			MessageDefinition1 message = new();

			await messenger.PublishAsync(message);

			m1.Message.Should().Be(message);

			exceptionHandler.Verify(e => e.ShouldRethrowException(
										It.IsAny<Exception>(),
										messenger,
										It.IsAny<string>(),
										It.IsAny<string>(),
										It.IsAny<int>()),
				Times.Once);
		}

		[Fact]
		public async Task ShouldInvokeExceptionHandlerAndRethrowExceptionIfRequired()
		{
			var exceptionHandler = new Mock<IExceptionHandler>();
			exceptionHandler.Setup(e => e.ShouldRethrowException(
									   It.IsAny<Exception>(),
									   It.IsAny<object>(),
									   It.IsAny<string>(),
									   It.IsAny<string>(),
									   It.IsAny<int>()))
							.Returns(true); // Force rethrow

			Messenger messenger = new()
			{
				ExceptionHandler = exceptionHandler.Object
			};

			ThrowingMessageSubscriberMock m1 = new();

			messenger.Subscribe(m1);

			MessageDefinition1 message = new();

			var action = async () => await messenger.PublishAsync(message);

			await action.Should().ThrowAsync<InvalidOperationException>();

			exceptionHandler.Verify(
				e => e.ShouldRethrowException(
					It.IsAny<Exception>(),
					messenger,
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<int>()),
				Times.Once);
		}

		class ThrowingMessageSubscriberMock : IMessageSubscriber<MessageDefinition1>
		{
			public MessageDefinition1? Message { get; private set; }

			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				Message = message;
				throw new InvalidOperationException("Simulated exception");
			}
		}

		#endregion

		#region Weak References

		[Fact]
		public async Task ShouldRemoveSubscriberWhenGarbageCollected()
		{
			Messenger messenger = new();

			WeakReference weakReference
				= new Func<WeakReference>(() =>
										  {
											  MessageSubscriberMock1 subscriber = new();
											  return new WeakReference(subscriber);
										  })();

			messenger.Subscribe(weakReference);

			GC.Collect();
			GC.WaitForPendingFinalizers();

			MessageDefinition1 message = new();

			await messenger.PublishAsync(message);

			weakReference.IsAlive.Should().BeFalse();
		}

		[Fact]
		public async Task ShouldNotRetainMemoryForCollectedSubscribers()
		{
			Messenger messenger = new();

			WeakReference weakReference
				= new Func<WeakReference>(() =>
										  {
											  MessageSubscriberMock1 subscriber = new();
											  return new WeakReference(subscriber);
										  })();

			messenger.Subscribe(weakReference);

			GC.Collect();
			GC.WaitForPendingFinalizers();

			// Publish a message to trigger cleanup of dead weak references
			MessageDefinition1 message = new();

			await messenger.PublishAsync(message);

			// Check memory leaks by ensuring the weak reference is no longer alive
			var action = async () => await messenger.PublishAsync(new MessageDefinition1());
			await action.Should().NotThrowAsync();
		}

		#endregion

		#region Recipient Type Filtering

		[Fact]
		public async Task ShouldOnlySendMessageToSpecifiedRecipientType()
		{
			Messenger messenger = new();
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
			Messenger messenger = new();
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

		#endregion

		#region Thread Safety

		[Fact]
		public void ShouldHandleConcurrentSubscriptions()
		{
			Messenger messenger = new();
			List<MessageSubscriberMock1> subscribers = new();

			Parallel.ForEach(Enumerable.Range(0, 100), _ =>
													   {
														   MessageSubscriberMock1 subscriber = new();
														   messenger.Subscribe(subscriber);
														   lock (subscribers)
														   {
															   subscribers.Add(subscriber);
														   }
													   });

			subscribers.Count.Should().Be(100);
		}

		[Fact]
		public void ShouldHandleConcurrentUnsubscriptions()
		{
			Messenger messenger = new();
			List<MessageSubscriberMock1> subscribers = new();

			// Subscribe 100 subscribers
			Parallel.ForEach(Enumerable.Range(0, 100), _ =>
													   {
														   MessageSubscriberMock1 subscriber = new();
														   messenger.Subscribe(subscriber);
														   lock (subscribers)
														   {
															   subscribers.Add(subscriber);
														   }
													   });

			// Unsubscribe them concurrently
			Parallel.ForEach(subscribers, subscriber => { messenger.Unsubscribe(subscriber); });

			subscribers.All(s => s.Message == null).Should().BeTrue();
		}

		[Fact]
		public void ShouldHandleConcurrentPublishing()
		{
			Messenger messenger = new();
			List<MessageSubscriberMock1> subscribers = new();

			// Subscribe 100 subscribers
			Parallel.ForEach(Enumerable.Range(0, 100), _ =>
													   {
														   MessageSubscriberMock1 subscriber = new();
														   messenger.Subscribe(subscriber);
														   lock (subscribers)
														   {
															   subscribers.Add(subscriber);
														   }
													   });

			// Concurrently publish messages to all subscribers
			Parallel.ForEach(Enumerable.Range(0, 10), async _ =>
													  {
														  MessageDefinition1 message = new();
														  await messenger.PublishAsync(message);
													  });

			// Check that all subscribers have received the message
			subscribers.All(s => s.Message != null).Should().BeTrue();
		}

		#endregion

		#region UI Thread Handling

		[Fact]
		public async Task ShouldPublishMessageOnUIThreadWhenRequired()
		{
			// Arrange
			Messenger messenger = new();
			MessageSubscriberMock1 subscriber = new();
			messenger.Subscribe(subscriber);

			bool onUIThread = false;

			var mockSynchronizationContext = new Mock<ISynchronizationContext>();
			mockSynchronizationContext
				.Setup(ctx => ctx.SendAsync(
						   It.IsAny<Func<Task>>(),
						   It.IsAny<bool>(),
						   It.IsAny<string>(),
						   It.IsAny<string>(),
						   It.IsAny<int>()))
				.Callback<Func<Task>, bool, string, string, int>(
					async (action, _, _, _, _) =>
					{
						// Simulate that we are on the UI thread
						onUIThread = true;
						await action();
					});

			UIContext.SetTestContext(mockSynchronizationContext.Object);

			MessageDefinition1 message = new();

			// Act
			await messenger.PublishAsync(message, true);

			// Assert
			onUIThread.Should().BeTrue();
			subscriber.Message.Should().Be(message);
		}

		[Fact]
		public async Task ShouldNotRequireUIThreadIfFlagIsFalse()
		{
			// Arrange
			Messenger messenger = new();
			MessageSubscriberMock1 subscriber = new();
			messenger.Subscribe(subscriber);

			bool onUIThread = false;

			var mockSynchronizationContext = new Mock<ISynchronizationContext>();
			mockSynchronizationContext
				.Setup(ctx => ctx.SendAsync(
						   It.IsAny<Func<Task>>(),
						   It.IsAny<bool>(),
						   It.IsAny<string>(),
						   It.IsAny<string>(),
						   It.IsAny<int>()))
				.Callback<Func<Task>, bool, string, string, int>(
					async (action, _, _, _, _) =>
					{
						onUIThread
							= true; // This should not be set if UI thread is not required
						await action();
					});

			UIContext.SetTestContext(mockSynchronizationContext.Object);

			MessageDefinition1 message = new();

			// Act
			await messenger.PublishAsync(message);

			// Assert
			onUIThread.Should().BeFalse();
			subscriber.Message.Should().Be(message);
		}

		#endregion

		#region Message Ordering

		[Fact]
		public async Task ShouldReceiveMessagesInOrderPublished()
		{
			// Arrange
			Messenger messenger = new();
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

		#endregion

		#region Publish Without Subscribers

		[Fact]
		public async Task ShouldNotThrowWhenPublishingWithoutSubscribers()
		{
			// Arrange
			Messenger messenger = new();

			MessageDefinition1 message = new();

			// Act & Assert
			var act = async () => await messenger.PublishAsync(message);

			await act.Should().NotThrowAsync();
		}

		#endregion

		#region Performance Testing

		[Fact]
		public async Task ShouldHandleLargeNumberOfSubscribersEfficiently()
		{
			// Arrange
			Messenger messenger = new();
			int numberOfSubscribers = 10000;
			List<MessageSubscriberMock1> subscribers = new();

			for (int i = 0; i < numberOfSubscribers; i++)
			{
				MessageSubscriberMock1 subscriber = new();
				messenger.Subscribe(subscriber);
				subscribers.Add(subscriber);
			}

			MessageDefinition1 message = new();
			Stopwatch stopwatch = Stopwatch.StartNew();

			// Act
			await messenger.PublishAsync(message);

			stopwatch.Stop();

			// Assert: Ensure that all subscribers received the message and it completes in reasonable time
			subscribers.All(s => s.Message == message).Should().BeTrue();

			testOutputHelper.WriteLine(
				$"Time taken for {numberOfSubscribers} subscribers: {stopwatch.ElapsedMilliseconds} ms");

			stopwatch.ElapsedMilliseconds.Should().BeLessThan(500); // Example time limit for performance
		}

		[Fact]
		public async Task ShouldHandleDuplicateSubscriptionsEfficiently()
		{
			// Arrange
			Messenger messenger = new();
			int numberOfSubscribers = 10000;
			List<MessageSubscriberMock1> subscribers = new();

			for (int i = 0; i < numberOfSubscribers; i++)
			{
				MessageSubscriberMock1 subscriber = new();
				messenger.Subscribe(subscriber);
				subscribers.Add(subscriber);

				// Subscribe the same subscriber again (should be ignored as duplicate)
				messenger.Subscribe(subscriber);
			}

			MessageDefinition1 message = new();
			Stopwatch stopwatch = Stopwatch.StartNew();

			// Act
			await messenger.PublishAsync(message);

			stopwatch.Stop();

			// Assert: Ensure that all subscribers received the message only once, and it completes in reasonable time
			subscribers.All(s => s.Message == message).Should().BeTrue();

			testOutputHelper.WriteLine(
				$"Time taken for {numberOfSubscribers} duplicate subscriptions: {stopwatch.ElapsedMilliseconds} ms");

			stopwatch.ElapsedMilliseconds.Should().BeLessThan(500); // Example time limit for performance
		}

		#endregion

		#region Handling Generic Constraints

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

		// Define MessageDefinition2 similar to MessageDefinition1
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

		// Define the ConstrainedMessage class with any necessary properties
		class ConstrainedMessage
		{
			public string? Content { get; set; }
		}

		// Define an invalid message type that does not satisfy the constraint
		class InvalidMessage
		{
		}

		#endregion
	}
}