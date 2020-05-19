#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-23 17:05:43Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.UserOptionsModel
{
	/// <summary>
	/// Indicates the result of a user option validation.
	/// </summary>
	public enum ValidationResultValue
	{
		Valid = 0x0,
		Invalid = 0x1,
		InvalidFormat = Invalid | 0x2,
		InvalidOutOfRange = Invalid | 0x4,
		InvalidRaisedException = Invalid | 0x8
	}
}
