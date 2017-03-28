#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-02 01:17:49Z</CreationDate>
</File>
*/
#endregion

namespace Codon.Navigation
{
	/// <summary>
	/// This message is dispatched when a navigation event
	/// occurs. Ordinarily this is when the app 
	/// transitions to a new page or activity.
	/// </summary>
	public class NavigatedMessage
	{
		/// <summary>
		/// An object that provides further information
		/// regarding the navigation.
		/// </summary>
		public NavigatedArgs Args { get; }

		public NavigatedMessage(NavigatedArgs args)
		{
			Args = args;
		}
	}
}