#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-08 14:55:27Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codon.Concurrency
{
	/// <summary>
	/// Extension methods for objects implementing
	/// <see cref="ISynchronizationContext"/>.
	/// </summary>
	public static class SynchronizationContextExtensions
	{
		/// <summary>
		/// Invokes the specified callback asynchronously.
		/// Method returns immediately upon queuing the request.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="deferralDepth">This optional parameter invokes 
		/// the action after deferralDepth passes of the context.
		/// For example, a deferralDepth of 2 sees the action is invoked 
		/// only after two passes by the context.
		/// A value of 0 behaves the same as BeginInvoke and is placed directly 
		/// on the context queue and invoked immediately.</param>
		/// <param name="action">The delegate to invoke.</param>
		/// <param name="memberName"></param>
		/// <param name="filePath"></param>
		/// <param name="lineNumber"></param>
		/// <param name="ignoreExceptionHandler">If <c>true</c> exceptions are not delivered
		/// to the exception handling mechanism, but are re-thrown.
		/// This is useful where you would like to handle an exception raised
		/// on the synchronization context thread but have an ExceptionHandler defined.
		/// Default is <c>false</c>.</param>
		public static async Task PostWithDeferralAsync(
			this ISynchronizationContext context,
			Action action,
			uint deferralDepth,
			[CallerMemberName] string memberName = null, 
			[CallerFilePath] string filePath = null, 
			[CallerLineNumber] int lineNumber = 0,
			bool ignoreExceptionHandler = false)
		{
			AssertArg.IsNotNull(context, nameof(context));
			AssertArg.IsNotNull(action, nameof(action));

			await PostWithDeferralCoreAsync(
						context, action, deferralDepth, ignoreExceptionHandler,
						memberName, filePath, lineNumber);
		}

		static async Task PostWithDeferralCoreAsync(
			this ISynchronizationContext context,
			Action action,
			uint deferralDepth,
			bool ignoreExceptionHandler,
			string memberName,
			string filePath,
			int lineNumber)
		{
			if (deferralDepth < 1)
			{
				await context.PostAsync(action, ignoreExceptionHandler, memberName, filePath, lineNumber);
				return;
			}

			await context.PostAsync(
				async () =>
				{
					await PostWithDeferralCoreAsync(
						context, action, deferralDepth - 1, ignoreExceptionHandler,
						memberName, filePath, lineNumber);
				});
		}

		/// <summary>
		/// Invokes the specified action after the specified delay in milliseconds.
		/// Note: Be sure to wrap the action code in a try catch, 
		/// as exceptions raised by the action are not handled.
		/// </summary>
		/// <param name="context">
		/// The context on which the action will be invoked.</param>
		/// <param name="action">
		/// The action to execute once the delay expires.</param>
		/// <param name="delayMs">
		/// The time, in milleseconds, to wait before executing the action.</param>
		/// <param name="ignoreExceptionHandler">If <c>true</c> exceptions are not delivered
		/// to the exception handling mechanism, but are re-thrown.
		/// This is useful where you would like to handle an exception raised
		/// on the synchonization context thread but have an ExceptionHandler defined.
		/// Default is <c>false</c>.</param>
		public static async Task PostWithDelayAsync(
			this ISynchronizationContext context, 
			Action action, 
			int delayMs,
			[CallerMemberName]string memberName = null,
			[CallerFilePath]string filePath = null,
			[CallerLineNumber]int lineNumber = 0,
			bool ignoreExceptionHandler = false)
		{
			AssertArg.IsNotNull(context, nameof(context));
			AssertArg.IsNotNull(action, nameof(action));
			AssertArg.IsGreaterThan(-1, delayMs, nameof(delayMs));
			
			if (delayMs >= 0)
			{
				await Task.Delay(delayMs);
			}
			
			await context.PostAsync(
				action, ignoreExceptionHandler, memberName, filePath, lineNumber);
		}
	}
}