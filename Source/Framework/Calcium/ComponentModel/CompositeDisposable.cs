#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-02-23 14:55:28Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using System;
using System.Collections.Generic;

namespace Calcium.ComponentModel
{
	sealed class CompositeDisposable : IDisposable
	{
		readonly IDisposable[] items;
		readonly Disposable.DisposeExceptionHandler? onDisposeException;
		bool disposed;

		public CompositeDisposable(params IDisposable[] items)
			: this(items, onDisposeException: null)
		{
		}

		public CompositeDisposable(
			IDisposable[] items,
			Disposable.DisposeExceptionHandler? onDisposeException)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			this.items = new IDisposable[items.Length];

			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] is null)
				{
					throw new ArgumentException(
						$"Disposable at index {i} is null.", nameof(items));
				}

				this.items[i] = items[i];
			}

			this.onDisposeException = onDisposeException;
		}

		public CompositeDisposable(IList<IDisposable> items)
			: this(items, onDisposeException: null)
		{
		}

		public CompositeDisposable(
			IList<IDisposable> items,
			Disposable.DisposeExceptionHandler? onDisposeException)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			this.items = new IDisposable[items.Count];

			for (int index = 0; index < items.Count; index++)
			{
				if (items[index] is null)
				{
					throw new ArgumentException($"Disposable at index {index} is null.", nameof(items));
				}

				this.items[index] = items[index];
			}

			this.onDisposeException = onDisposeException;
		}

		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			disposed = true;

			for (int index = items.Length - 1; index >= 0; index--)
			{
				IDisposable item = items[index];

				if (onDisposeException is null)
				{
					item.Dispose();

					continue;
				}

				try
				{
					item.Dispose();
				}
				catch (Exception ex)
				{
					try
					{
						onDisposeException(item, index, ex);
					}
					catch
					{
						/* Ignore */
					}
				}
			}
		}
	}
}