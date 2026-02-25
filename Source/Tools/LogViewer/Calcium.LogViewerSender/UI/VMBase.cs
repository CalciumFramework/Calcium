using Calcium.UIModel;

using Orpius.Shell.ComponentModel;

namespace Calcium.LogViewerSender.UI
{
	public abstract class VMBase : ViewModelBase
	{
		protected BusyTracker BusyTracker { get; }

		bool busy;

		public VMBase()
		{
			BusyTracker = new BusyTracker(() => busy, b => { Busy = b; });
		}

		public bool Busy
		{
			get => busy;
			private set => Set(ref busy, value);
		}
	}
}
