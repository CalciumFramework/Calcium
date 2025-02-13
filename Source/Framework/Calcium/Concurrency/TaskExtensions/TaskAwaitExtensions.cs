using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Calcium.Concurrency.TaskExtensions
{
	/// <summary>
	/// Provides extension methods for configuring await behavior on tasks.
	/// </summary>
	public static class TaskAwaitExtensions
	{
		/// <summary>
		/// Shorthand for <c>.ConfigureAwait(false)</c>.
		/// Does not capture the current synchronization context.
		/// </summary>
		/// <param name="task">The task to configure.</param>
		/// <returns>A task that does not capture the current synchronization context.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConfiguredTaskAwaitable NoSync(this Task task) => task.ConfigureAwait(false);

		/// <summary>
		/// Shorthand for <c>.ConfigureAwait(false)</c>.
		/// Does not capture the current synchronization context.
		/// </summary>
		/// <typeparam name="T">The return type of the <c>Task</c>.</typeparam>
		/// <param name="task">The task to configure.</param>
		/// <returns>A task that does not capture the current synchronization context.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConfiguredTaskAwaitable<T> NoSync<T>(this Task<T> task) => task.ConfigureAwait(false);
	}
}
