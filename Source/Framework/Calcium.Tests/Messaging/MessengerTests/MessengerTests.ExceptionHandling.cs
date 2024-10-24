#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-10-24 13:43:01Z</CreationDate>
</File>
*/
#endregion

using Calcium.ComponentModel;

using FluentAssertions;

using Moq;
// ReSharper disable ExplicitCallerInfoArgument

namespace Calcium.Messaging
{
	public partial class MessengerTests
	{
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
			public Task ReceiveMessageAsync(MessageDefinition1 message)
			{
				throw new InvalidOperationException("Simulated exception");
			}
		}
	}
}