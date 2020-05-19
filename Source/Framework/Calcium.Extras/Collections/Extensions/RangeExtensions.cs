#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-01 18:26:18Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

using Calcium.ComponentModel;

namespace Calcium.Collections
{
	/// <summary>
	/// This class contains extension methods for manipulating 
	/// a range (more than one item) in a collection at the same time.
	/// Note that to avoid multiple change notifications,
	/// a collection may implement <see cref="IRangeOperations"/>,
	/// such as the <c>ObservableList</c> collection.
	/// </summary>
	public static class RangeExtensions
    {
		/// <summary>
		/// Adds the items from one set of items to the specified list.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="toList">The destination list.</param>
		/// <param name="fromList">The source list.</param>
		public static void AddRange<T>(
			this IList<T> toList, IEnumerable<T> fromList)
		{
			AssertArg.IsNotNull(toList, nameof(toList));
			AssertArg.IsNotNull(fromList, nameof(fromList));

			IRangeOperations collection = toList as IRangeOperations;
			if (collection != null)
			{
				collection.AddRange(fromList);
				return;
			}

			var suspendableList = toList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}

				foreach (var item in fromList)
				{
					toList.Add(item);
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

		/// <summary>
		/// Adds the items from one set of items to the specified list.
		/// </summary>
		/// <param name="toList">The destination list.</param>
		/// <param name="fromList">The source list.</param>
		public static void AddRange(this IList toList, IEnumerable fromList)
		{
			AssertArg.IsNotNull(toList, nameof(toList));
			AssertArg.IsNotNull(fromList, nameof(fromList));

			IRangeOperations collection = toList as IRangeOperations;
			if (collection != null)
			{
				collection.AddRange(fromList);
				return;
			}

			var suspendableList = toList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}
				foreach (var item in fromList)
				{
					toList.Add(item);
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

		/// <summary>
		/// Inserts items from the specified fromList collection
		/// at the specified location in the toList collection.
		/// </summary>
		/// <param name="toList"></param>
		/// <param name="fromList"></param>
		/// <param name="startIndex"></param>
		public static void InsertRange(
			this IList toList, IEnumerable fromList, int startIndex)
		{
			AssertArg.IsNotNull(toList, nameof(toList));
			AssertArg.IsNotNull(fromList, nameof(fromList));
			AssertArg.IsGreaterThanOrEqual(0, startIndex, nameof(startIndex));

			IRangeOperations collection = toList as IRangeOperations;
			if (collection != null)
			{
				collection.InsertRange(fromList, startIndex);
				return;
			}

			var suspendableList = toList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}

				int newIndex = startIndex;

				foreach (var item in fromList)
				{
					toList.Insert(newIndex, item);
					newIndex++;
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

		/// <summary>
		/// Removes the specified items from the specified list.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="fromList">The list from which items will be removed.</param>
		/// <param name="removeItems">The list of items to remove.</param>
		public static void RemoveRange<T>(
			this IList<T> fromList, IEnumerable<T> removeItems)
		{
			AssertArg.IsNotNull(fromList, nameof(fromList));
			AssertArg.IsNotNull(removeItems, nameof(removeItems));

			IRangeOperations collection = fromList as IRangeOperations;
			if (collection != null)
			{
				collection.RemoveRange(removeItems);
				return;
			}

			var suspendableList = fromList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}
				foreach (var item in removeItems)
				{
					fromList.Remove(item);
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

		/// <summary>
		/// Removes the specified items from the specified list.
		/// </summary>
		/// <param name="fromList">The list from which items will be removed.</param>
		/// <param name="removeItems">The list of items to remove.</param>
		public static void RemoveRange(
			this IList fromList, IEnumerable removeItems)
		{
			AssertArg.IsNotNull(fromList, nameof(fromList));
			AssertArg.IsNotNull(removeItems, nameof(removeItems));

			IRangeOperations collection = fromList as IRangeOperations;
			if (collection != null)
			{
				collection.RemoveRange(removeItems);
				return;
			}

			var suspendableList = fromList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}
				foreach (var item in removeItems)
				{
					fromList.Remove(item);
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

		/// <summary>
		/// Replaces the items in the fromList collection that
		/// are present in the oldItems collection with the items
		/// in the newItems collection.
		/// </summary>
		/// <param name="fromList">The list to have items replaced.</param>
		/// <param name="oldItems">The items in the fromList to be replaced.</param>
		/// <param name="newItems">The items to replace the oldItems collection.</param>
		public static void ReplaceRange(
			this IList fromList, IEnumerable oldItems, IEnumerable newItems)
		{
			AssertArg.IsNotNull(fromList, nameof(fromList));
			AssertArg.IsNotNull(oldItems, nameof(oldItems));
			AssertArg.IsNotNull(newItems, nameof(newItems));

			IRangeOperations collection = fromList as IRangeOperations;
			if (collection != null)
			{
				collection.ReplaceRange(oldItems, newItems);
				return;
			}

			var suspendableList = fromList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}

				int startIndex = fromList.Count - 1;
				foreach (var item in oldItems)
				{
					int index = fromList.IndexOf(item);
					if (index < startIndex)
					{
						startIndex = index;
					}

					fromList.Remove(item);
				}

				if (startIndex < 0)
				{
					foreach (var item in newItems)
					{
						fromList.Add(item);
					}

					return;
				}

				int newIndex = startIndex;

				foreach (var item in newItems)
				{
					fromList.Insert(newIndex, item);
					newIndex++;
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
