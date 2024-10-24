#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:44:10Z</CreationDate>
</File>
*/
#endregion

using Calcium.Concurrency;

using FluentAssertions;

using Moq;
// ReSharper disable ExplicitCallerInfoArgument

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
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
					// ReSharper disable once AsyncVoidLambda
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
					// ReSharper disable once AsyncVoidLambda
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
	}
}