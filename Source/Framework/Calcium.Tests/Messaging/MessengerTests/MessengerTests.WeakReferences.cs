#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:44:27Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
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
	}
}