using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codon.Concurrency
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

		public bool InitializeRequired { get; } = false;
	}
}
