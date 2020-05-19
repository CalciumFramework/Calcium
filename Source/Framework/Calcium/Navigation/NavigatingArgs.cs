#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 14:25:20Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.MissingTypes.System.Windows.Navigation;

namespace Calcium.Navigation
{
	/// <summary>
	/// Informs of, and allows cancellation of, 
	/// a navigation event.
	/// </summary>
	public class NavigatingArgs : EventArgs
	{
		/// <summary>
		/// Gets the destination.
		/// </summary>
		public Uri Uri { get; private set; }

		/// <summary>
		/// Indicates whether the navigation is a new page,
		/// a back navigation or so forth.
		/// </summary>
		public NavigationType NavigationType { get; private set; }

		/// <summary>
		/// If <c>true</c> then a receiver may cancel
		/// the navigation. If <c>false</c> the navigation
		/// is cannot be cancelled.
		/// </summary>
		public bool Cancellable { get; private set; }

		/// <summary>
		/// If <c>true</c> this property indicates 
		/// that the application caused the navigation.
		/// If <c>false</c> then the operating system 
		/// caused the navigation, which is indicative
		/// of a navigation to a location outside the app;
		/// such as the homescreen or another app. 
		/// </summary>
		public bool IsNavigationInitiator { get; private set; }

		/// <summary>
		/// A parameter that is sent to the destination.
		/// </summary>
		public object Parameter { get; private set; }

		bool cancel;

		/// <summary>
		/// If supported, cancels the navigation.
		/// <seealso cref="Cancellable"/> 
		/// </summary>
		public bool Cancel
		{
			get => (BuiltInArgs as NavigatingCancelEventArgs)?.Cancel ?? cancel;
			set
			{
				if (BuiltInArgs is NavigatingCancelEventArgs cancelArgs)
				{
					cancelArgs.Cancel = value;
				}
				else
				{
					cancel = value;
				}
			}
		}

		/// <summary>
		/// The built in event args. May be <c>null</c>.
		/// </summary>
		public /*NavigatingCancelEventArgs*/ object BuiltInArgs { get; private set; }

		/// <summary>
		/// The target page of the navigation.
		/// The page type that is being navigated to.
		/// May be <c>null</c>.
		/// <seealso cref="Uri"/>
		/// </summary>
		public Type TargetPageType { get; private set; }

		public NavigatingArgs(
			Uri uri, 
			NavigationType navigationType, 
			bool cancellable = true, 
			bool isNavigationInitiator = true,
			object parameter = null,
			object builtInArgs = null)
		{
			Uri = uri;
			NavigationType = navigationType;
			Cancellable = cancellable;
			IsNavigationInitiator = isNavigationInitiator;
			Parameter = parameter;
			BuiltInArgs = builtInArgs;
		}

		public NavigatingArgs(
			Type targetPageType, 
			NavigationType navigationType, 
			bool cancellable = true, 
			bool isNavigationInitiator = true, 
			object parameter = null,
			object builtInArgs = null)
		{
			TargetPageType = targetPageType;
			NavigationType = navigationType;
			Cancellable = cancellable;
			IsNavigationInitiator = isNavigationInitiator;
			Parameter = parameter;
			BuiltInArgs = builtInArgs;
		}

		public NavigatingArgs(NavigatingCancelEventArgs args)
		{
			AssertArg.IsNotNull(args, nameof(args));

			BuiltInArgs = args;

			Uri = args.Uri;
			NavigationType = args.NavigationType;
			Parameter = args.ExtraData;
			IsNavigationInitiator = args.IsNavigationInitiator;
		}
	}
}
