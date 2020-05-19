#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

namespace Codon.Concurrency
{
	/// <summary>
	/// This class provides utility methods 
	/// related to the <see cref="Task"/> class.
	/// </summary>
	public static class TaskUtility
	{
		/// <summary>
		/// Awaits <see cref="Task.Yield"/>
		/// for the specified number of times.
		/// </summary>
		/// <param name="yieldCount">
		/// The number of times to yield.</param>
		public static async Task Yield(uint yieldCount)
		{
			for (int i = 0; i < yieldCount; i++)
			{
				await Task.Yield();
			}
		}

		/// <summary>
		/// This method provides a quick way to create
		/// a <see cref="TaskCompletionSource{T}"/>
		/// and set its Exception to the specified exception.
		/// </summary>
		/// <param name="exception">
		/// The exception to report was raised by the task.</param>
		/// <returns>A task with its Exception set 
		/// to the specified exception.</returns>
		public static Task<T> CreateTaskFromException<T>(
			Exception exception)
		{
			var source = new TaskCompletionSource<T>();
			source.SetException(exception);
			return source.Task;
		}

		/// <summary>
		/// This method provides a quick way to create
		/// a <see cref="TaskCompletionSource{T}"/>
		/// and set its Exception to the specified exception.
		/// </summary>
		/// <param name="exception">
		/// The exception to report was raised by the task.</param>
		/// <returns>A task with its Exception set 
		/// to the specified exception.</returns>
		public static Task CreateTaskFromException(Exception exception)
		{
			return CreateTaskFromException<object>(exception);
		}
	}
}
