#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 14:25:12Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;

namespace Calcium.Navigation
{
	/// <summary>
	/// Indicates the type of navigation.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public enum NavigationType
	{
		Unknown,
		Back,
		Forward,
		New,
		Refresh
	}
}
