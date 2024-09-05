#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-09-05 21:08:07Z</CreationDate>
</File>
*/
#endregion

#nullable enable
// ReSharper disable ExplicitCallerInfoArgument

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Calcium.Logging;
using Calcium.Services;

namespace Calcium.Messaging
{
	public static class MessengerExtensions
	{
		public static void PublishOnThreadPool<TEvent>(this IMessenger messenger,
		                                               TEvent eventToPublish,
		                                               bool requireUIThread = false,
		                                               Type? recipientType = null,
		                                               ILog? errorLog = null,
		                                               [CallerMemberName] string? memberName = null,
		                                               [CallerFilePath] string? filePath = null,
		                                               [CallerLineNumber] int lineNumber = 0)
		{
			Task.Run(() =>
			{
				void HandleMethodFailed(Task task, object? arg2)
				{
					var exception = task.Exception;
					if (exception != null)
					{
						if (messenger.ExceptionHandler?.ShouldRethrowException(exception, null, memberName, filePath,
						                                                       lineNumber) != false)
						{
							if (errorLog != null)
							{
								errorLog.Error("Exception thrown by IMessenger.PublishAsync on thread pool thread.",
								               exception, memberName: memberName, filePath: filePath, lineNumber: lineNumber);

								return;
							}

							throw exception;
						}
					}
				}

				messenger.PublishAsync(eventToPublish,
				                       requireUIThread,
				                       recipientType,
				                       memberName,
				                       filePath,
				                       lineNumber).ContinueWith(HandleMethodFailed, TaskContinuationOptions.OnlyOnFaulted);
			});
		}
	}
}
