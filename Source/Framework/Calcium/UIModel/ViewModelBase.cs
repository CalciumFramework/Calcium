#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-17 16:09:31Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.ComponentModel;
using Calcium.Services;

namespace Calcium.UIModel
{
	/// <summary>
	/// A simple base implementation of an <see cref="IViewModel"/>.
	/// </summary>
	public abstract class ViewModelBase : ObservableBase, IViewModel
	{
		protected IMessenger Messenger 
			=> Dependency.Resolve<IMessenger>();

		protected ViewModelBase()
		{
			Messenger.Subscribe(this);
		}

		/// <summary>
		/// Invokes the specified action on the UI thread. 
		/// If this call is made from the UI thread, 
		/// the action is performed synchronously.
		/// </summary>
		/// <param name="action"></param>
		protected void InvokeIfRequired(Action action)
		{
			PropertyChangeNotifier.SynchronizationContext.Send(action);
		}

		/// <summary>
		/// Executes the specified action without blocking. 
		/// </summary>
		/// <param name="action"></param>
		protected void BeginInvoke(Action action)
		{
			PropertyChangeNotifier.SynchronizationContext.Post(action);
		}

		/// <summary>
		/// Performs any activities required to disconnect
		/// itself from the framework infrastructure, 
		/// such as unsubscribing from the <c>IMessenger</c>.
		/// </summary>
		public virtual void CleanUp()
		{
			CleanUpInternal();
		}

		internal void CleanUpInternal()
		{
			Messenger.Unsubscribe(this);
		}
	}
}
