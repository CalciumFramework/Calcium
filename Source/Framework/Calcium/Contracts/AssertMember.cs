#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2025-03-11 12:57:43Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

namespace Calcium
{
	public class AssertMember
	{
		[return: NotNull]
		public static T IsNotNull<T>(T? value, string memberName)
		{
			if (value == null)
			{
				throw new InvalidOperationException($"{memberName} must not be null.");
			}

			return value;
		}
	}
}
