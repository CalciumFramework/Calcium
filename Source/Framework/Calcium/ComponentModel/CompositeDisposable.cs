#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2025, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-02-23 14:55:28Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Calcium.ComponentModel
{
	sealed class CompositeDisposable : IDisposable
	{
		readonly IDisposable[] items;

		public CompositeDisposable(params IDisposable[] items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			/* Copy to avoid caller mutating the array after construction. */
			this.items = new IDisposable[items.Length];

			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] is null)
				{
					throw new ArgumentException($"Disposable at index {i} is null.", nameof(items));
				}

				this.items[i] = items[i];
			}
		}

		public CompositeDisposable(IList<IDisposable> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			this.items = new IDisposable[items.Count];

			for (int i = 0; i < items.Count; i++)
			{
				if (items[i] is null)
				{
					throw new ArgumentException($"Disposable at index {i} is null.", nameof(items));
				}

				this.items[i] = items[i];
			}
		}

		public void Dispose()
		{
			for (int i = items.Length - 1; i >= 0; i--)
			{
				items[i].Dispose();
			}
		}
	}
}
