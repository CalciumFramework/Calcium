#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-09-18 15:13:09Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using System;
using System.Collections.Generic;

namespace Calcium.ComponentModel
{
	public static class Disposable
	{
		public delegate void DisposeExceptionHandler(IDisposable disposable,
													 int index,
													 Exception exception);

		public static IDisposable Empty { get; } = NoOpDisposable.Instance;

		public static IDisposable Create(Action disposeAction)
			=> new DisposableAction(disposeAction);

		/// <summary>
		/// Creates an <see cref="IDisposable"/> that disposes
		/// the supplied items in reverse order.
		/// </summary>
		/// <remarks>
		/// The last item supplied is disposed first.
		/// If an item throws during disposal, the exception is allowed
		/// to propagate and any remaining undisposed items are not disposed.
		/// </remarks>
		public static IDisposable Combine(params IDisposable[] items)
			=> new CompositeDisposable(items);

		/// <summary>
		/// Creates an <see cref="IDisposable"/> that disposes
		/// the supplied items in reverse order.
		/// </summary>
		/// <remarks>
		/// The last item supplied is disposed first.
		/// If an item throws during disposal, the exception is passed
		/// to <paramref name="onDisposeException"/>,
		/// and disposal continues with the remaining items.
		/// Exceptions thrown by <paramref name="onDisposeException"/> are ignored.
		/// </remarks>
		public static IDisposable Combine(
			DisposeExceptionHandler onDisposeException,
			params IDisposable[] items)
			=> new CompositeDisposable(items, onDisposeException);

		/// <summary>
		/// Creates an <see cref="IDisposable"/> that disposes
		/// the supplied items in reverse order.
		/// </summary>
		/// <remarks>
		/// The last item in the list is disposed first.
		/// If <paramref name="onDisposeException"/> is supplied,
		/// disposal exceptions are passed to it and disposal continues
		/// with the remaining items. Exceptions
		/// thrown by <paramref name="onDisposeException"/> are ignored.
		/// If <paramref name="onDisposeException"/> is <see langword="null"/>,
		/// disposal exceptions are allowed to propagate
		/// and any remaining undisposed items are not disposed.
		/// </remarks>
		public static IDisposable Combine(
			IList<IDisposable> items,
			DisposeExceptionHandler? onDisposeException = null)
			=> new CompositeDisposable(items, onDisposeException);
	}

	sealed class NoOpDisposable : IDisposable
	{
		public static NoOpDisposable Instance { get; } = new();

		NoOpDisposable()
		{
		}

		public void Dispose()
		{
		}
	}

	class DisposableAction : IDisposable
	{
		readonly Action disposeAction;
		bool disposed;

		public DisposableAction(Action disposeAction)
		{
			this.disposeAction = disposeAction 
								 ?? throw new ArgumentNullException(nameof(disposeAction));
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				disposeAction();
			}
		}
	}
}
