#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2019, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:23:11Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.Messaging
{
	[TestClass]
	public class DelegateSubscriberTests
	{
		class TestMessage
		{
		}

		[TestMethod]
		public async Task ShouldReceiveMessage()
		{
			bool receivedMessage = false;

			Task ReceiveMessage(TestMessage arg)
			{
				receivedMessage = true;
				return Task.CompletedTask;
			}

			var messenger = new Messenger();
			var subscriber = new DelegateSubscriber<TestMessage>(ReceiveMessage);
			messenger.Subscribe(subscriber);
			await messenger.PublishAsync(new TestMessage());

			Assert.IsTrue(receivedMessage, "Did not receive message back from messenger.");
		}
	}
}
