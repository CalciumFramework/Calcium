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

using System.Runtime.CompilerServices;

namespace Calcium.Concurrency
{
	class SynchronizationContextForTests : ISynchronizationContext
	{
		public void Post(
			Action action,
			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			action();
		}

		public Task PostAsync(
			Action action,
			bool ignoreExceptionHandler = false,
			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			action();

			return Task.CompletedTask;
		}

		public void Send(
			Action action,
			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			action();
		}

		public async Task SendAsync(
			Func<Task> action,
			bool ignoreExceptionHandler = false,
			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			await action();
		}

		public bool InvokeRequired { get; set; }

		public void Initialize()
		{
		}

		public Task PostAsync(Action action, 
							  [CallerMemberName] string? memberName = null,
							  [CallerFilePath] string? filePath = null, 
							  [CallerLineNumber] int lineNumber = 0)
		{
			action();

			return Task.CompletedTask;
		}

		public Task SendAsync(Func<Task> action, 
							  [CallerMemberName] string? memberName = null,
							  [CallerFilePath] string? filePath = null, 
							  [CallerLineNumber] int lineNumber = 0)
		{
			action();

			return Task.CompletedTask;
		}

		public bool InitializeRequired { get; } = false;
	}
}