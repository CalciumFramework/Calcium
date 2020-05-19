using System;
using System.ComponentModel;

namespace Codon.MissingTypes.System.ComponentModel
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class CancelEventArgs : EventArgs
	{
		/// <summary>
		/// Indicates, on return, whether or not the operation should be cancelled
		///     or not.  'true' means cancel it, 'false' means don't. 
		/// </summary>
		bool cancel;

		public CancelEventArgs() : this(false)
		{
		}

		public CancelEventArgs(bool cancel)
		{
			this.cancel = cancel;
		}

		/// <summary>
		/// Gets or sets a value indicating whether 
		/// the operation should be cancelled. 
		/// </summary>
		public bool Cancel
		{
			get => cancel;
			set => cancel = value;
		}
	}
}
