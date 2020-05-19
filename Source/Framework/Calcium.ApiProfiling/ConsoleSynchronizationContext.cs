using System;
using System.Threading.Tasks;

namespace Codon.Concurrency
{
	class ConsoleSynchronizationContext : ISynchronizationContext
	{
		public void Post(Action action, string memberName = null, string filePath = null, int lineNumber = 0)
		{
			Task.Run(() => action());
		}

		public async Task PostAsync(Action action, bool ignoreExceptionHandler = false, string memberName = null, string filePath = null, int lineNumber = 0)
		{
			await Task.Run(() => action());
		}

		public void Send(Action action, string memberName = null, string filePath = null, int lineNumber = 0)
		{
			action();
		}

		public Task SendAsync(Func<Task> action, bool ignoreExceptionHandler = false, string memberName = null, string filePath = null, int lineNumber = 0)
		{
			action();
			return Task.CompletedTask;
		}

		public bool InvokeRequired { get; }

		public void Initialize()
		{
			
		}

		public bool InitializeRequired { get; }
	}
}
