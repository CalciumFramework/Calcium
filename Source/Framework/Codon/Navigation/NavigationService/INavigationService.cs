#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-05-29 19:12:26Z</CreationDate>
</File>
*/
#endregion

using Codon.InversionOfControl;

namespace Codon.Services
{
	/// <summary>
	/// This service provides information on the current page location 
	/// and allows the client to navigate within the application.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Navigation)
		+ ".NavigationService, " + AssemblyConstants.PlatformAssembly, Singleton = true)]
	public interface INavigationService
	{
		/// <summary>
		/// Navigates to the previous page.
		/// </summary>
		void GoBack();

		/// <summary>
		/// Indicates that the application is able 
		/// to navigate to the previous page.
		/// </summary>
		bool CanGoBack { get; }

		/// <summary>
		/// Navigates to the specified relative URL.
		/// The URL should be relative to the root of the application 
		/// and begin with a slash e.g., "/Views/Settings.xaml".
		/// </summary>
		/// <param name="relativeUrl">The relative URL.</param>
		/// <param name="navigationArgument">
		/// An argument to be passed to the page being navigated to.</param>
		void Navigate(string relativeUrl, object navigationArgument = null);

		/// <summary>
		/// An object that was passed to the <see cref="Navigate"/> method.
		/// The property value is replaced or set <c>null</c>
		/// upon each navigation.
		/// </summary>
		object NavigationArgument { get; }
	}
}
