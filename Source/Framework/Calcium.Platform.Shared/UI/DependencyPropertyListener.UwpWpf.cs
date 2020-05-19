#if WPF || WINDOWS_UWP
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-23 13:26:15Z</CreationDate>
</File>
*/
#endregion

using System;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Calcium.UI
{
	/// <summary>
	/// This class is used to listen to change events
	/// for dependency properties. It provides an
	/// event, <see cref="Changed"/>, that signals
	/// that the value of a dependency property has changed.
	/// </summary>
	public class DependencyPropertyListener
	{
		static int index;
		readonly DependencyProperty property;
		DependencyObject target;

		public DependencyPropertyListener()
		{
			if (index == int.MaxValue - 1)
			{
				index = 0;
			}

			property = DependencyProperty.RegisterAttached(
				"DependencyPropertyListener" + index++,
				typeof(object),
				typeof(DependencyPropertyListener),
				new PropertyMetadata(null, HandleValueChanged));
		}

		public event EventHandler<BindingChangedEventArgs> Changed;

		void HandleValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			OnChanged(new BindingChangedEventArgs(e));
		}

		protected virtual void OnChanged(BindingChangedEventArgs e)
		{
			Changed?.Invoke(target, e);
		}

		public void Attach(DependencyObject dependencyObject, Binding binding)
		{
			if (target != null)
			{
				throw new Exception("Cannot attach an already attached listener");
			}

			target = dependencyObject;
			BindingOperations.SetBinding(dependencyObject, property, binding);
		}

		public void Detach()
		{
			target.ClearValue(property);
			target = null;
		}
	}

	public class BindingChangedEventArgs : EventArgs
	{
		public DependencyPropertyChangedEventArgs EventArgs { get; private set; }

		public BindingChangedEventArgs(DependencyPropertyChangedEventArgs e)
		{
			EventArgs = e;
		}
	}
}
#endif
