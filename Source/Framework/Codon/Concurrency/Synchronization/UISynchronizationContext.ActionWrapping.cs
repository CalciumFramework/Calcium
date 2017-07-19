#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-05 21:13:52Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;
using Codon.ComponentModel;

namespace Codon.Concurrency
{
	partial class UISynchronizationContext
	{
		IExceptionHandler settableExceptionHandler;
		int retrieveAttempts;

		/// <summary>
		/// When an exception occurs during execution or during evaluating 
		/// if the command can execute, then the exception is passed to the exception manager.
		/// If <c>null</c> the IoC container is used to locate an instance.
		/// </summary>
		public IExceptionHandler ExceptionHandler
		{
			get
			{
				/* We limit the attempts to retrieve the exception handler; 
				 * otherwise it would slow things down unnecessarily. */
				if (settableExceptionHandler == null
					&& retrieveAttempts++ < 3)
				{
					if (Dependency.Initialized)
					{
						Dependency.TryResolve(out settableExceptionHandler);
					}
				}

				return settableExceptionHandler;
			}
			set => settableExceptionHandler = value;
		}

		#region Action Wrapping

		Action EmbedTaskCompletionSource(
			Action action,
			TaskCompletionSource<object> source)
		{
			if (source == null)
			{
				return action;
			}

			return () =>
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					source.SetException(ex);
					return;
				}

				source.SetResult(null);
			};
		}

		Func<Task> EmbedTaskCompletionSource(
			Func<Task> action, TaskCompletionSource<object> source)
		{
			if (source == null)
			{
				return action;
			}

			return () =>
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					source.SetException(ex);
					return Task.CompletedTask;
				}

				source.SetResult(null);

				return Task.CompletedTask;
			};
		}

		Action GetWrappedAction(
			Action action,
			TaskCompletionSource<object> source,
			string memberName = null,
			string filePath = null,
			int lineNumber = 0)
		{
			var handler = ExceptionHandler;
			if (handler == null)
			{
				return EmbedTaskCompletionSource(action, source);
			}

			Action invokableAction = () =>
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					bool rethrow = handler.ShouldRethrowException(
										ex, this,
										memberName, filePath, lineNumber);
					if (rethrow)
					{
						if (source != null)
						{
							source.SetException(ex);
							return;
						}

						throw;
					}
				}

				source?.SetResult(null);
			};

			return invokableAction;
		}

		Func<Task> GetWrappedAction(
			Func<Task> action,
			TaskCompletionSource<object> source,
			string memberName = null,
			string filePath = null,
			int lineNumber = 0)
		{
			var handler = ExceptionHandler;
			if (handler == null)
			{
				return EmbedTaskCompletionSource(action, source);
			}

			Func<Task> invokableAction = async () =>
			{
				try
				{
					await action();
				}
				catch (Exception ex)
				{
					bool rethrow = handler.ShouldRethrowException(
										ex, this, memberName, filePath, lineNumber);
					if (rethrow)
					{
						if (source != null)
						{
							source.SetException(ex);
							return;
						}

						throw;
					}
				}

				source?.SetResult(null);
			};

			return invokableAction;
		}

		#endregion
	}
}
