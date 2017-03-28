#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
#if __ANDROID__ || MONODROID
using Android.Views;
#else
using View = System.Object;
#endif

namespace Codon.UI.Data
{
	public class ViewEventBinder<TView, TArgs, TNewValue> : IViewBinder
#if __ANDROID__ || MONODROID
		where TView : View
#endif
		where TArgs : EventArgs
	{
		readonly Action<TView, EventHandler<TArgs>> addHandler;
		readonly Action<TView, EventHandler<TArgs>> removeHandler;
		readonly Func<TView, TArgs, TNewValue> newValueFunc;

		public Type ViewType => typeof(TView);

		public ViewEventBinder(
			Action<TView, EventHandler<TArgs>> addHandler,
			Action<TView, EventHandler<TArgs>> removeHandler,
			Func<TView, TArgs, TNewValue> newValueFunc)
		{
			this.addHandler = AssertNotNull(addHandler, nameof(addHandler));
			this.removeHandler = AssertNotNull(removeHandler, nameof(removeHandler));
			this.newValueFunc = AssertNotNull(newValueFunc, nameof(newValueFunc));
		}

		T AssertNotNull<T>(T value, string name)
		{
			if (value == null)
			{
				throw new ArgumentNullException(name);
			}

			return value;
		}

		public Action BindView(PropertyBinding propertyBinding, object dataContext)
		{
			EventHandler<TArgs> handler =
				(sender, args) =>
				{
					ViewValueChangedHandler.HandleViewValueChanged(propertyBinding, newValueFunc, dataContext, args);
				};

			addHandler((TView)propertyBinding.View, handler);

			Action removeAction = () => { removeHandler((TView)propertyBinding.View, handler); };
			return removeAction;
		}
	}
}