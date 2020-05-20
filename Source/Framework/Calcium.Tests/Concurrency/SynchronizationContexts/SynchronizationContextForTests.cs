using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Calcium.Concurrency
{
	class SynchronizationContextForTests : ISynchronizationContext
	{
		public void Post(
			Action action,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			action();
		}

		public Task PostAsync(
			Action action, 
			bool ignoreExceptionHandler = false, 
			[CallerMemberName] string memberName = null, 
			[CallerFilePath] string filePath = null, 
			[CallerLineNumber] int lineNumber = 0)
		{
			action();

			return Task.CompletedTask;
		}

		public void Send(
			Action action,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			action();
		}

		public async Task SendAsync(
			Func<Task> action, 
			bool ignoreExceptionHandler = false,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			await action();
		}

		public bool InvokeRequired { get; set; }

		public void Initialize()
		{
		}

		public Task PostAsync(Action action, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
		{
			action();

			return Task.CompletedTask;
		}

		public Task SendAsync(Func<Task> action, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
		{
			action();

			return Task.CompletedTask;
		}

		public bool InitializeRequired { get; } = false;
	}
}
