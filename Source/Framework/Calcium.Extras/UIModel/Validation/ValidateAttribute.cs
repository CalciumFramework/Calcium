#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-02-18 20:26:04Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.UIModel.Validation
{
	/// <summary>
	/// Decorate properties with this attribute
	/// for input validation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ValidateAttribute : Attribute
	{
		/* Intentionally left blank. */
	}
}
