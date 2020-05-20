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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Calcium.ComponentModel;

namespace Calcium.Collections
{
	/// <summary>
	/// An adaptive collection allows you to combine two different collection types.
	/// It allows you to materialize the objects in the inner collection,
	/// which are of type <c>TInner</c> as objects of type <c>T</c>
	/// using a <see cref="IAttachObject{T}"/> implementation.
	/// This class is useful, for example, in materializing a collection 
	/// of <c>ICommand</c> objects as a set of <c>DropDownMenuItem</c> objects.
	/// This class is a read-only variant of the <see cref="AdaptiveCollection{T,TInner}"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of objects that can be bound to a list.
	/// </typeparam>
	/// <typeparam name="TInner">
	/// The type of objects contained in the source collection.</typeparam>
	public class ReadOnlyAdaptiveCollection<T, TInner> : IReadOnlyList<T>,
														INotifyCollectionChanged, 
														INotifyPropertyChanged,
														IList<T>,
														IDisposable
		where T : class, IAttachObject<TInner>, new()
	{
	
		readonly Dictionary<TInner, T> lookupDictionary
			= new Dictionary<TInner, T>();
	
		readonly List<T> items = new List<T>();
	
		readonly ReadOnlyObservableCollection<TInner> monitoredCollection;
		public ReadOnlyObservableCollection<TInner> MonitoredCollection => monitoredCollection;
	
		bool ignoreChanges;
	
		public void Dispose()
		{
			((INotifyCollectionChanged)monitoredCollection).CollectionChanged -= HandleCollectionChanged;
			((INotifyPropertyChanged)monitoredCollection).PropertyChanged -= HandlePropertyChanged;
			lookupDictionary.Clear();
			items.Clear();
		}
	
		public ReadOnlyAdaptiveCollection(ReadOnlyObservableCollection<TInner> monitoredCollection)
		{
			this.monitoredCollection = monitoredCollection 
										?? throw new ArgumentNullException(nameof(monitoredCollection));
	
			((INotifyCollectionChanged)this.monitoredCollection).CollectionChanged += HandleCollectionChanged;
			((INotifyPropertyChanged)this.monitoredCollection).PropertyChanged += HandlePropertyChanged;

			SynchronizeCollections();
		}
	
		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}
	
		void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (ignoreChanges)
			{
				return;
			}
	
			NotifyCollectionChangedAction action = e.Action;
	
			if (action == NotifyCollectionChangedAction.Add)
			{
				var list = new List<T>();
	
				foreach (TInner command in e.NewItems)
				{
					var attacher = new T();
					attacher.AttachObject(command);
					lookupDictionary[command] = attacher;
					list.Add(attacher);
				}

				items.AddRange(list);

				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
				RaisePropertyCountChanged();
			}
			else if (action == NotifyCollectionChangedAction.Move)
			{
				int movedFromIndex = e.OldStartingIndex;
				int movedToIndex = e.NewStartingIndex;
				int itemCount = e.OldItems?.Count ?? 0;
				T movingItem = items[movedFromIndex];
				items.RemoveAt(movedFromIndex);
				items.Insert(movedToIndex, movingItem);
				
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movingItem, movedFromIndex, movedToIndex));
			}
			else if (action == NotifyCollectionChangedAction.Remove)
			{
				var oldItems = e.OldItems;
	
				var itemsToRemove = new List<T>();
	
				foreach (TInner oldItem in oldItems)
				{
					if (lookupDictionary.TryGetValue(oldItem, out T owner) && owner != null)
					{
						itemsToRemove.Add(owner);
					}
	
					lookupDictionary.Remove(oldItem);
				}
				
				Exception disposeException = null;

				foreach (T owner in itemsToRemove)
				{
					items.Remove(owner);
					
					try
					{
						owner.DetachObject();

						var disposable = owner as IDisposable;
						disposable?.Dispose();
					}
					catch (Exception ex)
					{
						disposeException = ex;
					}
				}

				if (disposeException != null)
				{
					throw new Exception("Problem detaching owner.", disposeException);
				}

				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsToRemove));
				RaisePropertyCountChanged();
			}
			else if (action == NotifyCollectionChangedAction.Replace)
			{
				var removedItems = e.OldItems;
	
				var itemsToRemove = new List<T>();
	
				foreach (TInner oldItem in removedItems)
				{
					if (lookupDictionary.TryGetValue(oldItem, out T owner) && owner != null)
					{
						itemsToRemove.Add(owner);
					}
	
					lookupDictionary.Remove(oldItem);
				}
	
				foreach (T owner in itemsToRemove)
				{
					try
					{
						ignoreChanges = true;
						items.Remove(owner);
					}
					finally
					{
						ignoreChanges = false;
					}
	
					owner.DetachObject();
	
					var disposable = owner as IDisposable;
					disposable?.Dispose();
				}
	
				var newItems = e.NewItems;
				var replacements = new List<T>();

				foreach (TInner newItem in newItems)
				{
					var index = monitoredCollection.IndexOf(newItem);
	
					var attacher = new T();
					attacher.AttachObject(newItem);
					lookupDictionary[newItem] = attacher;
					replacements.Add(attacher);

					try
					{
						ignoreChanges = true;
						itemsToRemove.Insert(index, attacher);
					}
					finally
					{
						ignoreChanges = false;
					}
				}

				items.ReplaceRange(itemsToRemove, replacements);

				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, replacements, itemsToRemove));
			}
			else if (action == NotifyCollectionChangedAction.Reset)
			{
				var list = new List<T>(items);
				items.Clear();
	
				foreach (var owner in list)
				{
					if (owner == null)
					{
						continue;
					}
	
					owner.DetachObject();
	
					var disposable = owner as IDisposable;
					disposable?.Dispose();
				}

				SynchronizeCollections();

				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				RaisePropertyCountChanged();
			}
		}

		void SynchronizeCollections()
		{
			lookupDictionary.Clear();
			items.Clear();

			foreach (TInner item in monitoredCollection)
			{
				T attacher = new T();
				attacher.AttachObject(item);
				lookupDictionary[item] = attacher;

				items.Add(attacher);
			}
		}

		void RaisePropertyCountChanged()
		{
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		}
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) 
			=> CollectionChanged?.Invoke(this, e);

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) 
			=> PropertyChanged?.Invoke(this, e);

		public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
	
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <inheritdoc />
		public void Add(T item) => throw new NotSupportedException();

		/// <inheritdoc />
		public void Clear() => throw new NotSupportedException();

		/// <inheritdoc />
		public bool Contains(T item) => items.Contains(item);

		/// <inheritdoc />
		public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

		/// <inheritdoc />
		public bool Remove(T item) => throw new NotSupportedException();

		public int Count => items.Count;

		/// <inheritdoc />
		public bool IsReadOnly => true;

		/// <inheritdoc />
		public int IndexOf(T item) => items.IndexOf(item);

		/// <inheritdoc />
		public void Insert(int index, T item) => throw new NotSupportedException();

		/// <inheritdoc />
		public void RemoveAt(int index) => throw new NotSupportedException();

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// index is not a valid index.</exception>
		/// <exception cref="NotSupportedException">
		/// The property is set and this collection is read-only.</exception>
		public T this[int index]
		{
			get => items[index];
			set => throw new NotSupportedException();
		}
	}
}
