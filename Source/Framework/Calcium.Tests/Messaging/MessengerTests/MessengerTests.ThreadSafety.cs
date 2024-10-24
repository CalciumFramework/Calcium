#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:44:01Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
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
			// ReSharper disable once AsyncVoidLambda
			Parallel.ForEach(Enumerable.Range(0, 10), async _ =>
													  {
														  MessageDefinition1 message = new();
														  await messenger.PublishAsync(message);
													  });

			// Check that all subscribers have received the message
			subscribers.All(s => s.Message != null).Should().BeTrue();
		}
	}
}