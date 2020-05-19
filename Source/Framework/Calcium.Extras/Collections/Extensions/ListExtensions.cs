#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2020, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2020-01-01 22:42:46Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Codon.Collections
{
	/// <summary>
	/// This class provides extension methods
	/// for types implementing <see cref="IList{T}"/> and <see cref="IList"/>.
	/// </summary>
	public static class ListExtensions
	{
		/// <summary>
		/// Moves an item from the specified fromIndex to the specified toIndex.
		/// </summary>
		/// <typeparam name="T">The generic list type.</typeparam>
		/// <param name="list">The list where the item is located.</param>
		/// <param name="fromIndex">The current location.</param>
		/// <param name="toIndex">The new location.</param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		/// <exception cref="NullReferenceException"></exception>
		public static void Move<T>(this IList<T> list, int fromIndex, int toIndex)
		{
			AssertArg.IsNotNull(list, nameof(list));

			var item = list[fromIndex];
			list.RemoveAt(fromIndex);
			list.Insert(toIndex, item);
		}

		// /// <summary>
		// /// Moves an item from the specified fromIndex to the specified toIndex.
		// /// </summary>
		// /// <param name="list">The list where the item is located.</param>
		// /// <param name="fromIndex">The current location.</param>
		// /// <param name="toIndex">The new location.</param>
		// /// <exception cref="IndexOutOfRangeException"></exception>
		// /// <exception cref="NotSupportedException"></exception>
		// /// <exception cref="NullReferenceException"></exception>
		// public static void Move(this IList list, int fromIndex, int toIndex)
		// {
		// 	AssertArg.IsNotNull(list, nameof(list));
		//
		// 	var item = list[fromIndex];
		// 	list.RemoveAt(fromIndex);
		// 	list.Insert(toIndex, item);
		// }
	}
}
