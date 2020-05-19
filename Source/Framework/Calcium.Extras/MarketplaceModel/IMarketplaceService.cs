#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-19 11:55:01Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;
using Calcium.InversionOfControl;
using Calcium.MarketplaceModel;

namespace Calcium.Services
{
	/// <summary>
	/// The marketplace service is able to interact with a platform
	/// specific marketplace, such as Google Play or the Microsoft App Store.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(MarketplaceModel)
		+ ".MarketplaceService, " + AssemblyConstants.ExtrasPlatformAssembly, Singleton = true)]
	[DefaultType(typeof(MockMarketplaceService), Singleton = true)]
	public interface IMarketplaceService
	{
		/// <summary>
		/// Gets a value indicating whether the app 
		/// is a trial version.
		/// </summary>
		bool Trial { get; }

		/// <summary>
		/// Launches the built-in marketplace experience
		/// and shows the purchase page for the app.
		/// </summary>
		Task<object> PurchaseAppAsync();

		/// <summary>
		/// Launches the built-in marketplace experience
		/// and shows the review page for the app.
		/// </summary>
		void Review();

		/// <summary>
		/// Launches the built-in marketplace experience
		/// and shows the page for the specified content
		/// or the app if <c>contentId</c> is <c>null</c>.
		/// </summary>
		/// <param name="contentId">
		/// A marketplace specific resource identifier.
		/// For example, on Android the <c>contentId</c>
		/// is appended to the end of market://details?id= </param>
		void ShowDetails(object contentId = null);
	}
}
