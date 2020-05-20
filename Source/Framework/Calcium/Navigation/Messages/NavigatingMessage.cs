#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-02 01:17:38Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.Navigation
{
	/// <summary>
	/// This message is dispatched when a navigation event
	/// is about to occur. Ordinarily this is when the app 
	/// is transitioning to a new page or activity.
	/// </summary>
	public class NavigatingMessage
	{
		/// <summary>
		/// An object that provides further information
		/// regarding the navigation and allows the cancellation
		/// of the navigation.
		/// </summary>
		public NavigatingArgs Args { get; }

		public NavigatingMessage(NavigatingArgs args)
	    {
			Args = args;
	    }
    }
}
