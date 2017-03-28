#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2008-12-28 19:52:25Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using Codon.ComponentModel;

namespace Codon.Collections
{
	/// <summary>
	/// This class provides extension methods for collections types. 
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		/// Determines whether the collection is null or contains no elements.
		/// </summary>
		/// <typeparam name="T">The IEnumerable type.</typeparam>
		/// <param name="enumerable">The enumerable, which may be null or empty.</param>
		/// <returns>
		/// 	<c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				return true;
			}

			/* If this is a list, use the Count property. 
			 * The Count property is O(1) while IEnumerable.Count() is O(N). */
			var collection = enumerable as ICollection<T>;
			if (collection != null)
			{
				return collection.Count < 1;
			}
			return enumerable.Any();
		}

		/// <summary>
		/// Determines whether the collection is null or contains no elements.
		/// </summary>
		/// <typeparam name="T">The IEnumerable type.</typeparam>
		/// <param name="collection">The collection, which may be null or empty.</param>
		/// <returns>
		/// 	<c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
		{
			if (collection == null)
			{
				return true;
			}
			return collection.Count < 1;
		}

		/// <summary>
		/// Examines each item in the double collection 
		/// and returns the index of the greatest value.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public static int GetIndexOfGreatest(this IEnumerable<double> values)
		{
			AssertArg.IsNotNull(values, nameof(values));

			int result = -1;
			int count = 0;
			double highest = -1;
			foreach (var d in values)
			{
				if (d > highest)
				{
					highest = d;
					result = count;
				}
				count++;
			}
			return result;
		}

		/// <summary>
		/// Removes all items from the list that do not pass the filter condition.
		/// </summary>
		/// <typeparam name="T">The generic type of the list.</typeparam>
		/// <param name="list">The list to remove from.</param>
		/// <param name="filter">The filter to evaluate each item with.</param>
		/// <returns>The removed items.</returns>
		public static IEnumerable<T> RemoveAllAndReturnItems<T>(
			this IList<T> list, Func<T, bool> filter)
		{
			var suspendableList = list as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}

				for (int i = 0; i < list.Count; i++)
				{
					if (filter(list[i]))
					{
						var item = list[i];
						list.Remove(list[i]);
						yield return item;
					}
				}
			}
			finally
			{
				if (suspendableList != null && !wasSuspended)
				{
					try
					{
						suspendableList.ChangeNotificationSuspended = false;
					}
					catch (Exception)
					{
						/* Suppress. */
					}
				}
			}
		}
	}
}
