#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 14:25:28Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.MissingTypes.System.Windows.Navigation;

namespace Calcium.Navigation
{
	/// <summary>
	/// Informs of a navigation event.
	/// </summary>
	public class NavigatedArgs
	{
		/// <summary>
		/// The destination page.
		/// </summary>
		public object Content { get; set; }

		/// <summary>
		/// Gets the destination.
		/// </summary>
		public Uri Uri { get; private set; }

		/// <summary>
		/// The type of the page that was navigated to.
		/// </summary>
		public Type PageType { get; set; }

		/// <summary>
		/// Indicates whether the navigation is a new page,
		/// a back navigation or so forth.
		/// </summary>
		public NavigationType NavigationType { get; set; }

		/// <summary>
		/// If <c>true</c> this property indicates 
		/// that the application caused the navigation.
		/// If <c>false</c> then the operating system 
		/// caused the navigation, which is indicative
		/// of a navigation to a location outside the app;
		/// such as the homescreen or another app. 
		/// </summary>
		public bool IsNavigationInitiator { get; set; }

		/// <summary>
		/// A parameter that is sent to the destination.
		/// </summary>
		public object Parameter { get; set; }

		/// <summary>
		/// The built in event args. May be <c>null</c>.
		/// </summary>
		public object BuiltInArgs { get; private set; }

		public NavigatedArgs(
			object content, Uri uri,
			NavigationType navigationType = NavigationType.New, 
			bool isNavigationInitiator = true,
			object parameter = null,
			object builtInArgs = null)
		{
			Content = content;
			Uri = uri;
			NavigationType = navigationType;
			IsNavigationInitiator = isNavigationInitiator;
			Parameter = parameter;
			BuiltInArgs = builtInArgs;
		}

		public NavigatedArgs(
			object content, Type pageType,
			NavigationType navigationType = NavigationType.New, 
			bool isNavigationInitiator = true,
			object parameter = null,
			object builtInArgs = null)
		{
			Content = content;
			PageType = pageType;
			NavigationType = navigationType;
			IsNavigationInitiator = isNavigationInitiator;
			Parameter = parameter;
			BuiltInArgs = builtInArgs;
		}

//		public NavigatedArgs(NavigationEventArgs args)
//		{
//			AssertArg.IsNotNull(args, nameof(args));
//
//			Uri = args.Uri;
//			Content = args.Content;
//			BuiltInArgs = args;
//		}
	}
}
