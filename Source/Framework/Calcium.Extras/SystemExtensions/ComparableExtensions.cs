#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium
{
	/// <summary>
	/// Extension methods for classes 
	/// implementing <see cref="IComparable{T}"/>.
	/// </summary>
	public static class ComparableExtensions
	{
		/// <summary>
		/// Constrains a value to be within an
		/// upper and lower boundary.
		/// </summary>
		/// <typeparam name="T">The <c>Type</c> of value.</typeparam>
		/// <param name="value">The value to constrain.</param>
		/// <param name="minimum">The minimum value.
		/// If the specified value is less than the minimum,
		/// the specified minimum is returned.</param>
		/// <param name="maximum">The maximum value.
		/// If the specified value is greater than the maximum,
		/// the specified maximum is returned.</param>
		/// <returns>If the value falls within the specified range, 
		/// then the specified value is returned. 
		/// If the value is less than the specified minimum, 
		/// then minimum is returned.
		/// If the value is greater than the specified maximum, 
		/// then maximum is returned.</returns>
		public static T Clamp<T>(this T value, T minimum, T maximum) 
			where T : IComparable<T>
		{
			if (value.CompareTo(minimum) < 0)
			{
				return minimum;
			}

			if (value.CompareTo(maximum) > 0)
			{
				return maximum;
			}

			return value;
		}
	}
}
