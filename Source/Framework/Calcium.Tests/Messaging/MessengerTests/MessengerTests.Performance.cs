#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:43:42Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;

using FluentAssertions;

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
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
	}
}