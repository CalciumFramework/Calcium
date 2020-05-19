#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 13:22:08Z</CreationDate>
</File>
*/
#endregion

using System;
using Codon.InversionOfControl;

namespace Codon.Navigation
{
	/// <summary>
	/// Associates URLs with <c>Action</c>s that
	/// are used by the <see cref="Services.INavigationService"/>
	/// implementation. When a URL is requested via the 
	/// <see cref="Services.INavigationService"/>,
	/// the routing services retrieves the associated action
	/// and invokes the action.
	/// </summary>
	[DefaultType(typeof(RoutingService), Singleton = true)]
	public interface IRoutingService
	{
		/// <summary>
		/// Associates a URL with a navigation action.
		/// When the <see cref="Services.INavigationService"/>
		/// receives a request to navigate to a URL,
		/// the routing service retrieves the associated
		/// navigation action and the action is invoked. 
		/// </summary>
		/// <param name="url">The URL of the action.</param>
		/// <param name="navigationAction">
		/// The action to associate with the URL.
		/// The navigation action is passed an optional 
		/// object parameter by the caller.</param>
		void RegisterPath(string url, Action<object> navigationAction);
	}
}