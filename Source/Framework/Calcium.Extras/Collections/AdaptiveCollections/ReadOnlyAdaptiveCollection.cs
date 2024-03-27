#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2020, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2020-01-01 20:40:48Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System;
using System.Collections;

namespace Calcium.Collections
{
	/// <summary>
	/// A read-only collection that adapts items from one type to another using a provided factory function.
	/// It listens to changes from an underlying ReadOnlyObservableCollection and updates accordingly.
	/// </summary>
	/// <typeparam name="TTo">The type of objects that the collection will contain.</typeparam>
	/// <typeparam name="TFrom">The type of objects contained in the source collection.</typeparam>
	public class ReadOnlyAdaptiveCollection<TTo, TFrom> : IEnumerable<TTo>,
														  IReadOnlyList<TTo>,
														  INotifyCollectionChanged,
														  INotifyPropertyChanged,
														  IDisposable where TFrom : notnull
	{
		readonly ReadOnlyObservableCollection<TFrom> monitoredCollection;
		readonly Dictionary<TFrom, TTo> itemsMap = new();
		readonly ObservableCollection<TTo> items = new();

		public ReadOnlyObservableCollection<TTo> ReadOnlyObservableCollection { get; }

		readonly Func<TFrom, TTo> getItem;
		bool handlingChange;

		public ReadOnlyAdaptiveCollection(
			ReadOnlyObservableCollection<TFrom> monitoredCollection,
			Func<TFrom, TTo> getItem)
		{
			this.monitoredCollection = monitoredCollection ?? throw new ArgumentNullException(nameof(monitoredCollection));
			this.getItem = getItem ?? throw new ArgumentNullException(nameof(getItem));

			AdaptItems();

			ReadOnlyObservableCollection = new(items);

			((INotifyCollectionChanged)monitoredCollection).CollectionChanged += OnMonitoredCollectionChanged;
		}

		void OnMonitoredCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (handlingChange)
			{
				throw new InvalidOperationException("Reentrant modification detected on the monitored collection.");
			}

			try
			{
				handlingChange = true;
				ProcessChangeEvent(e);
			}
			finally
			{
				handlingChange = false;
			}
		}

		void ProcessChangeEvent(NotifyCollectionChangedEventArgs e)
		{
			bool countChanged = false;

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						if (e.NewItems != null)
						{
							foreach (TFrom newItem in e.NewItems)
							{
								TTo adaptedItem = getItem(newItem);
								itemsMap.Add(newItem, adaptedItem);
								items.Add(adaptedItem);
							}

							countChanged = true;
							CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems));
						}
						break;
					}

				case NotifyCollectionChangedAction.Remove:
					{
						if (e.OldItems != null)
						{
							foreach (TFrom oldItem in e.OldItems)
							{
								if (itemsMap.TryGetValue(oldItem, out TTo? itemToRemove))
								{
									itemsMap.Remove(oldItem);
									items.Remove(itemToRemove);
									CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemToRemove));
								}
							}

							countChanged = true;
						}

						break;
					}

				case NotifyCollectionChangedAction.Replace:
					{
						if (e is { OldItems: not null, NewItems: not null } && e.OldItems.Count == e.NewItems.Count)
						{
							for (int i = 0; i < e.OldItems.Count; i++)
							{
								if (e.OldItems[i] is TFrom oldItem && e.NewItems[i] is TFrom newItem)
								{
									var newAdaptedItem = getItem(newItem);

									if (itemsMap.TryGetValue(oldItem, out TTo? oldAdaptedItem))
									{
										itemsMap.Remove(oldItem);
										itemsMap.Add(newItem, newAdaptedItem);

										int index = items.IndexOf(oldAdaptedItem);
										if (index != -1)
										{
											items[index] = newAdaptedItem;
										}
										CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newAdaptedItem, oldAdaptedItem, index));
									}
								}
							}
						}
						break;
					}

				case NotifyCollectionChangedAction.Move:
					{
						if (e is { OldStartingIndex: >= 0, NewStartingIndex: >= 0, OldItems: not null })
						{
							foreach (TFrom item in e.OldItems)
							{
								TTo movingItem = itemsMap[item];
								items.Remove(movingItem);
								items.Insert(e.NewStartingIndex, movingItem);
							}

							CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.OldItems, e.NewStartingIndex, e.OldStartingIndex));
						}
						break;
					}

				case NotifyCollectionChangedAction.Reset:
					{
						itemsMap.Clear();
						items.Clear();

						AdaptItems();

						CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

						countChanged = true;

						break;
					}
			}

			if (countChanged)
			{
				OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
			}
		}

		void AdaptItems()
		{
			foreach (var item in monitoredCollection)
			{
				TTo adaptedItem = getItem(item);
				itemsMap.Add(item, adaptedItem);
				items.Add(adaptedItem);
			}
		}

		public IEnumerator<TTo> GetEnumerator() => items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

		public void Dispose()
		{
			((INotifyCollectionChanged)monitoredCollection).CollectionChanged -= OnMonitoredCollectionChanged;
		}

		public int Count => items.Count;

		public bool IsReadOnly => true;

		public TTo this[int index] => items[index];
	}
}
