#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-03 14:21:06Z</CreationDate>
</File>
*/
#endregion

using System;
using System.ComponentModel;
using Calcium.Concurrency;

namespace Calcium.DialogModel
{
	/// <summary>
	/// This class is used primarily to close a dialog
	/// from calling code when using one of the Show*
	/// methods of the <see cref="Services.IDialogService" />.
	/// </summary>
	public class DialogController
	{
		// Should be internal but Xamarin Android doesn't support strong name for internals visible attribute.
		public bool CloseCalled { get; private set; }

		/// <summary>
		/// This event is not intended for user code.
		/// This event is used by the 
		/// <see cref="Services.IDialogService" />
		/// implementation to close the dialog 
		/// when the <see cref="Close"/> method is called.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler CloseRequested;

		/// <summary>
		/// Call this method to dismiss the dialog.
		/// The <see cref="Services.IDialogService" />
		/// implementation dismisses the dialog immediately.
		/// </summary>
		/// <param name="e">An optional event args object,
		/// which may be used to convey information to the 
		/// <see cref="Services.IDialogService" /> implementation</param>
		public virtual void Close(EventArgs e = null)
		{
			CloseCalled = true;

			var synchronizationContext = Dependency.Resolve<ISynchronizationContext>();
			synchronizationContext.Send(() =>
			{
				CloseRequested?.Invoke(this, e ?? EventArgs.Empty);
			});
		}

		/// <summary>
		/// If <c>true</c> the dialog is able to be dismissed 
		/// by the user. If <c>false</c> the dialog must be actioned
		/// using one of its buttons.
		/// </summary>
		public bool Cancellable { get; set; }

		/// <summary>
		/// Indicates how the dialog should be displayed.
		/// At present this affects the Android implementation
		/// of the <see cref="Calcium.Services.IDialogService" />.
		/// </summary>
		public DialogStyles? DialogStyles { get; set; }
	}
}
