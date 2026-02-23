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

using System;
using System.Collections.Generic;

namespace Calcium.ComponentModel
{
	public static class Disposable
	{
		public static IDisposable Empty { get; } = NoOpDisposable.Instance;

		public static IDisposable Create(Action disposeAction)
			=> new DisposableAction(disposeAction);

		public static IDisposable Combine(params IDisposable[] items)
			=> new CompositeDisposable(items);

		public static IDisposable Combine(IList<IDisposable> items)
			=> new CompositeDisposable(items);
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
