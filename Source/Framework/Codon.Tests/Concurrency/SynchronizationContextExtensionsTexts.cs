using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.Concurrency
{
	[TestClass]
	public class SynchronizationContextExtensionsTexts
	{
		[TestMethod]
		public async Task PostActionAfterDelay()
		{
			var context = new SynchronizationContextForTests();

			bool executed = false;
			DateTime startTime = DateTime.Now;
			const int delayMs = 1000;
			
			await context.PostWithDelayAsync(() =>
			{
				executed = true;
			}, delayMs);

			Assert.IsTrue(executed, "Action was not executed.");
			var timeDifference = DateTime.Now - startTime;

			Assert.IsTrue(
				timeDifference.TotalMilliseconds >= delayMs,
				"Action should have been executed after delay.");
		}

//		[TestMethod]
//		public async Task PostActionAtDepth()
//		{
//			var context = new UISynchronizationContext();
//
//			bool executed = false;
//			
//			const int depth = 5;
//
//			context.PostWithDeferralAsync(() =>
//			{
//				executed = true;
//			}, depth);
//
//			for (int i = 0; i < depth; i++)
//			{
//				Assert.IsFalse(executed, 
//					"Action should not have been executed yet. " + i);
//				await Task.Yield();
//			}
//			
//			await Task.Yield();
//			await Task.Yield();
//
//			Assert.IsTrue(executed, "Action should have been executed.");
//		}
	}
}
