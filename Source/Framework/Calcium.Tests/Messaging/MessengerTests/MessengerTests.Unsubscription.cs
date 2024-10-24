#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:44:18Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
		[Fact]
		public async Task ShouldNotReceiveMessagesAfterUnsubscribing()
		{
			Messenger messenger = new();
			MessageSubscriberMock1 m1 = new();

			messenger.Subscribe(m1);
			messenger.Unsubscribe(m1);

			MessageDefinition1 message = new();
			await messenger.PublishAsync(message);

			m1.Message.Should().BeNull();
		}

		[Fact]
		public void ShouldNotThrowWhenUnsubscribingWithoutSubscription()
		{
			Messenger messenger = new();
			MessageSubscriberMock1 m1 = new();

			// Unsubscribing without prior subscription
			messenger.Invoking(m => m.Unsubscribe(m1)).Should().NotThrow();
		}

		[Fact]
		public void ShouldNotThrowWhenUnsubscribingTwice()
		{
			Messenger messenger = new();
			MessageSubscriberMock1 m1 = new();

			messenger.Subscribe(m1);
			messenger.Unsubscribe(m1);

			// Unsubscribing again
			messenger.Invoking(m => m.Unsubscribe(m1)).Should().NotThrow();
		}
	}
}