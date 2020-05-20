#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-01-10 16:27:11Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Calcium.ComponentModel;

namespace Calcium.Collections
{
	/// <summary>
	/// This class extends <see cref="ObservableCollection{T}"/>
	/// to provide range operations. 
	/// <see cref="IRangeOperations"/>
	/// </summary>
	/// <typeparam name="T">
	/// The type of items contained within the collection.</typeparam>
	public class ObservableList<T> : ObservableCollection<T>, 
		ISuspendChangeNotification, IRangeOperations
	{
		public ObservableList()
		{
			/* Intentionally left blank. */
		}

#if WINDOWS_PHONE
		public ObservableList(IEnumerable<T> collection)
		{
			AssertArg.IsNotNull(collection, nameof(collection));
			AddRangeCore(collection);
		}

		public ObservableList(List<T> collection)
		{
			AssertArg.IsNotNull(collection, nameof(collection));
			AddRangeCore(collection);
		}
#else
		public ObservableList(IEnumerable<T> collection) : base(collection)
		{
			/* Intentionally left blank. */
		}

		public ObservableList(List<T> collection) : base(collection)
		{
			/* Intentionally left blank. */
		}
#endif

		bool changeNotificationSuspended;

		public bool ChangeNotificationSuspended
		{
			get => changeNotificationSuspended;
			set => changeNotificationSuspended = value;
		}
		
		protected override void OnCollectionChanged(
			NotifyCollectionChangedEventArgs e)
		{
			if (!changeNotificationSuspended)
			{
				base.OnCollectionChanged(e);
			}
		}

		/// <summary>
		/// Adds the specified range of items
		/// to the list.
		/// </summary>
		/// <param name="items">
		/// The list of items to add. Cannot be <c>null</c>.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if <c>items</c> is <c>null</c>.</exception>
		public void AddRange(IEnumerable<T> items)
		{
			AssertArg.IsNotNull(items, nameof(items));

			AddRangeCore(items);
		}

		void AddRangeCore(IEnumerable<T> items)
		{
			bool wasSuspended = changeNotificationSuspended;
			IList<T> newItems = new List<T>(items);
			
			try
			{
				changeNotificationSuspended = true;

				foreach (var item in newItems)
				{
					base.Add(item);
				}
			}
			finally
			{
				if (!wasSuspended) /* Avoid unsetting suspended, 
								 * if already explicitly set. This allow the user 
								 * to add more items without unsetting 
								 * ChangeNotificationSuspended again. */
				{
					changeNotificationSuspended = false;
				}
			}

			if (!wasSuspended)
			{
				/* Using NotifyCollectionChangedAction.Add 
				 * raises an exception on the desktop, 
				 * hence we use Reset for both. */
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset);
				OnCollectionChanged(eventArgs);
			}
		}

		/// <summary>
		/// Removes the specified range of items
		/// from the list.
		/// </summary>
		/// <param name="items">
		/// The list of items to remove. Cannot be <c>null</c>.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if <c>items</c> is <c>null</c>.</exception>
		public void RemoveRange(IEnumerable<T> items)
		{
			AssertArg.IsNotNull(items, nameof(items));

			RemoveRangeCore(items);
		}
		
		void RemoveRangeCore(IEnumerable<T> items)
		{
			bool wasSuspended = changeNotificationSuspended;

			changeNotificationSuspended = true;
			IList<T> removedItems = new List<T>();

			try
			{
				foreach (var item in items.ToList())
				{
					if (base.Remove(item))
					{
						removedItems.Add(item);
					}
				}
			}
			finally
			{
				if (!wasSuspended) /* Avoid unsetting suspended, 
								 * if already explicitly set. This allow the user 
								 * to add more items without unsetting 
								 * ChangeNotificationSuspended again. */
				{
					changeNotificationSuspended = false;
				}
			}
			if (!wasSuspended)
			{
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset);
				OnCollectionChanged(eventArgs);
			}
		}

		public void AddRange(IEnumerable items)
		{
			AssertArg.IsNotNull(items, nameof(items));

			AddRangeCore(items.Cast<T>());
		}

		public void RemoveRange(IEnumerable items)
		{
			AssertArg.IsNotNull(items, nameof(items));

			RemoveRangeCore(items.Cast<T>());
		}

		public void ReplaceRange(int startIndex, IEnumerable newItems)
		{
			AssertArg.IsGreaterThanOrEqual(0, startIndex, nameof(startIndex));
			AssertArg.IsLessThanOrEqual(base.Count - 1, startIndex, nameof(startIndex));
			AssertArg.IsNotNull(newItems, nameof(newItems));

			ReplaceRangeCore(startIndex, newItems.Cast<T>());
		}

		void ReplaceRangeCore(int startIndex, IEnumerable<T> newItems)
		{
			bool wasSuspended = changeNotificationSuspended;

			int start = startIndex;

			if (start < 0)
			{
				start = 0;
			}

			List<T> itemsToRemove = new List<T>();

			try
			{
				changeNotificationSuspended = true;

				int segmentEndIndex = start + newItems.Count();
				int itemCount = base.Count;
				if (segmentEndIndex > itemCount)
				{
					segmentEndIndex = base.Count - start;
				}
				
				for (int i = start; i > segmentEndIndex; i++)
				{
					var item = base.Items[i];

					itemsToRemove.Add(item);
				}

				foreach (var item in itemsToRemove)
				{
					base.Remove(item);
				}

				if (start < 0)
				{
					foreach (var item in newItems)
					{
						base.Add(item);
					}

					return;
				}

				int newIndex = start;

				foreach (var item in newItems)
				{
					base.Insert(newIndex, item);
					newIndex++;
				}
			}
			finally
			{
				if (!wasSuspended) /* Avoid unsetting suspended, 
								 * if already explicitly set. This allow the user 
								 * to add more items without unsetting 
								 * ChangeNotificationSuspended again. */
				{
					changeNotificationSuspended = false;
				}
			}

			if (!wasSuspended)
			{
				/* Using NotifyCollectionChangedAction.Add 
				 * raises an exception on the desktop, 
				 * hence we use Reset for both. */
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Replace, newItems, itemsToRemove, start);
				OnCollectionChanged(eventArgs);
			}
		}

		public void ReplaceRange(IEnumerable oldItems, IEnumerable newItems)
		{
			AssertArg.IsNotNull(oldItems, nameof(oldItems));
			AssertArg.IsNotNull(newItems, nameof(newItems));

			ReplaceRangeCore(oldItems.Cast<T>(), newItems.Cast<T>());
		}

		void ReplaceRangeCore(IEnumerable<T> oldItems, IEnumerable<T> newItems)
		{
			bool wasSuspended = changeNotificationSuspended;

			int startIndex = base.Count - 1;

			try
			{
				changeNotificationSuspended = true;
				
				foreach (var item in oldItems)
				{
					int index = base.IndexOf(item);
					if (index < startIndex)
					{
						startIndex = index;
					}

					base.Remove(item);
				}

				if (startIndex < 0)
				{
					foreach (var item in newItems)
					{
						base.Add(item);
					}

					return;
				}

				int newIndex = startIndex;

				foreach (var item in newItems)
				{
					base.Insert(newIndex, item);
					newIndex++;
				}
			}
			finally
			{
				if (!wasSuspended) /* Avoid unsetting suspended, 
								 * if already explicitly set. This allow the user 
								 * to add more items without unsetting 
								 * ChangeNotificationSuspended again. */
				{
					changeNotificationSuspended = false;
				}
			}

			if (!wasSuspended)
			{
				/* Using NotifyCollectionChangedAction.Add 
				 * raises an exception on the desktop, 
				 * hence we use Reset for both. */
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Replace, 
					newItems, 
					oldItems, 
					startIndex);

				OnCollectionChanged(eventArgs);
			}
		}

		public void InsertRange(IEnumerable items, int startIndex)
		{
			AssertArg.IsNotNull(items, nameof(items));
			AssertArg.IsGreaterThanOrEqual(0, startIndex, nameof(startIndex));

			InsertRangeCore(items.Cast<T>(), startIndex);
		}

		void InsertRangeCore(IEnumerable<T> items, int startIndex)
		{
			bool wasSuspended = changeNotificationSuspended;

			try
			{
				changeNotificationSuspended = true;

				int newIndex = startIndex;

				foreach (var item in items)
				{
					base.Insert(newIndex, item);
					newIndex++;
				}
			}
			finally
			{
				if (!wasSuspended) /* Avoid unsetting suspended, 
								 * if already explicitly set. This allow the user 
								 * to add more items without unsetting 
								 * ChangeNotificationSuspended again. */
				{
					changeNotificationSuspended = false;
				}
			}

			if (!wasSuspended)
			{
				/* Using NotifyCollectionChangedAction.Add 
				 * raises an exception on the desktop, 
				 * hence we use Reset for both. */
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, items, startIndex);
				OnCollectionChanged(eventArgs);
			}
		}
	}
}
