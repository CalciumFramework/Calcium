using System;

namespace Orpius.Shell.ComponentModel
{
	public class BusyTracker
	{
		int activeCount;
		readonly object lockObj = new();
		readonly Func<bool> getBusy;
		readonly Action<bool> setBusy;

		/// <summary>
		///     Initializes a new instance of BusyTracker.
		///     Accepts two delegates for getting and setting the Busy state externally.
		/// </summary>
		/// <param name="getBusy">A delegate that gets the current Busy state.</param>
		/// <param name="setBusy">A delegate that sets the Busy state.</param>
		public BusyTracker(Func<bool> getBusy, Action<bool> setBusy)
		{
			this.getBusy = getBusy ?? throw new ArgumentNullException(nameof(getBusy));
			this.setBusy = setBusy ?? throw new ArgumentNullException(nameof(setBusy));
		}

		/// <summary>
		///     Provides an IDisposable that will set Busy to true on creation and automatically
		///     decrement the count and set Busy to false when disposed.
		/// </summary>
		/// <returns>IDisposable that will control the Busy state.</returns>
		public IDisposable EnterBusy()
		{
			lock (lockObj)
			{
				if (activeCount == 0 && !getBusy())
				{
					setBusy(true);
				}

				activeCount++;
			}

			return new BusyScope(this);
		}

		void ExitBusy()
		{
			lock (lockObj)
			{
				activeCount--;

				if (activeCount == 0 && getBusy())
				{
					setBusy(false);
				}
			}
		}

		class BusyScope : IDisposable
		{
			readonly BusyTracker tracker;
			bool disposed;

			public BusyScope(BusyTracker tracker)
			{
				this.tracker = tracker;
			}

			public void Dispose()
			{
				if (!disposed)
				{
					disposed = true;
					tracker.ExitBusy(); // Calls the parent tracker to decrement active count
				}
			}
		}
	}
}