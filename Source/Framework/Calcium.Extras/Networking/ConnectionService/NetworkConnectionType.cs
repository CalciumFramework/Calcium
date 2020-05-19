#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-01-01 11:46:25Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.Networking
{
	/// <summary>
	/// Indicates the type of internet connection 
	/// available to the application.
	/// </summary>
	public enum NetworkConnectionType
	{
		None = 0,
		Lan = 1,
		MobileBroadband = 2
	}
}
