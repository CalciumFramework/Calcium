#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-06 12:12:38Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;
using SystemContext = System.Threading.SynchronizationContext;

using Codon.Logging;

namespace Codon.Concurrency
{
	/// <summary>
	/// The default implementation of the 
	/// <see cref="ISynchronizationContext"/> interface. 
	/// See the interface for API documentation.
	/// </summary>
	public sealed partial class UISynchronizationContext : ISynchronizationContext
	{
		SystemContext systemContext;
		int? uiThreadID;
		readonly object initializationLock = new object();

		SystemContext Context
		{
			get
			{
				if (systemContext == null)
				{
					Initialize();
				}

				return systemContext;
			}
		}

		public void Post(
			Action action, 
			string memberName = null, 
			string filePath = null, 
			int lineNumber = 0)
		{
			AssertArg.IsNotNull(action, nameof(action));
			EnsureInitialized();

			Action wrapper = GetWrappedAction(
								action, null,
								memberName, filePath, lineNumber);

			Context.Post(state => wrapper(), null);
		}

		public Task PostAsync(
			Action action, 
			string memberName = null, 
			string filePath = null, 
			int lineNumber = 0)
		{
			AssertArg.IsNotNull(action, nameof(action));
			EnsureInitialized();

			var source = new TaskCompletionSource<object>();

			Action wrapper = GetWrappedAction(
								action, source,
								memberName, filePath, lineNumber);

			Context.Post(state => wrapper(), null);

			return source.Task;
		}

		public void Send(Action action, string memberName = null, string filePath = null, int lineNumber = 0)
		{
			AssertArg.IsNotNull(action, nameof(action));
			EnsureInitialized();

			Action wrapper = GetWrappedAction(
								action, null,
								memberName, filePath, lineNumber);

			if (InvokeRequired)
			{
				Context.Send(state => wrapper(), null);
			}
			else
			{
				wrapper();
			}
		}

		public Task SendAsync(
			Func<Task> action, 
			string memberName = null, 
			string filePath = null, 
			int lineNumber = 0)
		{
			AssertArg.IsNotNull(action, nameof(action));
			EnsureInitialized();

			bool isBackground = InvokeRequired;

			TaskCompletionSource<object> source = isBackground
								? new TaskCompletionSource<object>()
								: null;

			Func<Task> wrappedAction
				= GetWrappedAction(
					action, source,
					memberName, filePath, lineNumber);

			if (isBackground)
			{
				Context.Send(state => wrappedAction(), null);
				return source.Task;
			}

			return wrappedAction();
		}

		public bool InvokeRequired
		{
			get
			{
				EnsureInitialized();

				return !uiThreadID.HasValue || 
					uiThreadID.Value != Environment.CurrentManagedThreadId;
			}
		}

		public void Initialize()
		{
			lock (initializationLock)
			{
				if (systemContext != null)
				{
					return;
				}

				systemContext = SystemContext.Current;

				if (systemContext != null)
				{
					uiThreadID = Environment.CurrentManagedThreadId;
				}
				else
				{
					const string message = "UISynchronizationContext must be initialized " +
											"on the UI thread. Please call " +
											"Dependency.Resolve<ISynchronizationContext>().Initialize() " +
											"from the application's main thread.";
					var log = Dependency.Resolve<ILog>();
					log.Error(message);

					throw new ConcurrencyException(message);
				}
			}
		}

		static int counter;
		int instanceCounter = counter++;

		void EnsureInitialized()
		{
			if (systemContext == null)
			{
				lock (initializationLock)
				{
					if (systemContext == null)
					{
						Initialize();
					}
				}
			}
		}

		public bool InitializeRequired { get; } = true;
	}
}