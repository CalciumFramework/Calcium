#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
#if !__ANDROID__
using View = System.Object;
#else
using Android.Views;
#endif

namespace Calcium.UI.Data
{
	/// <summary>
	/// This interface represents an extensibility point for adding support 
	/// for different types of views that may, or may not, already be included 
	/// in the <see cref="ViewBinderRegistry"/>.
	/// </summary>
	public interface IViewBinder
	{
		Action BindView(PropertyBinding binding, object viewModel);

		Type ViewType { get; }
	}
}
