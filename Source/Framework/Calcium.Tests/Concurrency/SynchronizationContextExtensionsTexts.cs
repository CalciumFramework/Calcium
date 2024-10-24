#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:15Z</CreationDate>
</File>
*/
#endregion

using FluentAssertions;

namespace Calcium.Concurrency
{
	public class SynchronizationContextExtensionsTests
	{
		[Fact]
		public async Task PostActionAfterDelay()
		{
			SynchronizationContextForTests context = new();

			bool executed = false;
			DateTime startTime = DateTime.UtcNow;
			const int delayMs = 1000;

			await context.PostWithDelayAsync(() =>
											 {
												 executed = true;
											 }, delayMs);

			executed.Should().BeTrue("Action was not executed.");
			var timeDifference = DateTime.UtcNow - startTime;

			timeDifference.TotalMilliseconds.Should().BeGreaterOrEqualTo(delayMs,
				"Action should have been executed after the delay.");
		}

		//        [Fact]
		//        public async Task PostActionAtDepth()
		//        {
		//            var context = new UISynchronizationContext();
		//
		//            bool executed = false;
		//
		//            const int depth = 5;
		//
		//            context.PostWithDeferralAsync(() =>
		//            {
		//                executed = true;
		//            }, depth);
		//
		//            for (int i = 0; i < depth; i++)
		//            {
		//                executed.Should().BeFalse("Action should not have been executed yet. " + i);
		//                await Task.Yield();
		//            }
		//
		//            await Task.Yield();
		//            await Task.Yield();
		//
		//            executed.Should().BeTrue("Action should have been executed.");
		//        }
	}
}
