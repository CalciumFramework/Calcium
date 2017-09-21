#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-02-18 13:17:48Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Codon.Collections
{
	/// <summary>
	/// This class is used to group a set of objects within, 
	/// for example, a <c>LongListSelector</c>.
	/// </summary>
	/// <typeparam name="TElement">The type of the element 
	/// that is group by an object key.</typeparam>
	public class GroupedList<TElement> : GroupedList<object, TElement>
	{
		public GroupedList(object key, IEnumerable<TElement> items) : base(key, items)
		{
		}

		public GroupedList(object key, IEnumerable<TElement> items, Func<object> titleFunc) 
			: base(key, items, titleFunc)
		{
		}

		public GroupedList(object key, IEnumerable<TElement> items, object title) 
			: base(key, items, title)
		{
		}
	}

	/// <summary>
	/// This class is used to group a set of objects within, for example, a LongListSelector.
	/// </summary>
	/// <typeparam name="TElement">The type of the element 
	/// that is group by an object key.</typeparam>
	/// <typeparam name="TKey">The type of the key object.</typeparam>
	public class GroupedList<TKey, TElement> : ObservableCollection<TElement>, IGrouping<TKey, TElement>
	{
		readonly TKey key;
		readonly object title;

		public long ListId { get; set; }

		public GroupedList(TKey key, IEnumerable<TElement> items) : base(items)
		{
			this.key = key;
		}

		public GroupedList(TKey key, IEnumerable<TElement> items, Func<object> titleFunc) 
			: this(key, items)
		{
			this.titleFunc = titleFunc;
		}

		public GroupedList(TKey key, IEnumerable<TElement> items, object title)
			: this(key, items)
		{
			this.title = title;
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			/* [DV] I need to see if this is called by the base class for Count property. */
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
		}

		public TKey Key => key;

		public object Title
		{
			get
			{
				if (titleFunc != null)
				{
					return titleFunc();
				}
				else if (title != null)
				{
					return title;
				}
				return key;
			}
		}

		readonly Func<object> titleFunc;

		public Func<object> TitleFunc => titleFunc;

		public void RaiseOnTitlePropertyChanged()
		{
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(Title)));
		}

		public bool HasItems => this.Any();

		public override bool Equals(object obj)
		{
			var otherGrouping = obj as GroupedList<TKey, TElement>;
			return otherGrouping != null && Key.Equals(otherGrouping.Key);
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}
	}

	public interface IObservableMultiList : INotifyCollectionChanged
	{
		event EventHandler<NotifyChildCollectionChangedEventArgs> ChildCollectionChanged;
	}

	public class ObservableMultiList<TCollection> : ObservableCollection<TCollection>, IObservableMultiList
		where TCollection : INotifyCollectionChanged
	{
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (INotifyCollectionChanged newItem in e.NewItems)
				{
					newItem.CollectionChanged -= HandleChildCollectionChanged;
					newItem.CollectionChanged += HandleChildCollectionChanged;
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (INotifyCollectionChanged newItem in e.OldItems)
				{
					newItem.CollectionChanged -= HandleChildCollectionChanged;
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (INotifyCollectionChanged oldItem in e.OldItems)
				{
					oldItem.CollectionChanged -= HandleChildCollectionChanged;
				}

				foreach (INotifyCollectionChanged newItem in e.NewItems)
				{
					newItem.CollectionChanged -= HandleChildCollectionChanged;
					newItem.CollectionChanged += HandleChildCollectionChanged;
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				var oldItems = e.OldItems;
				if (oldItems != null)
				{
					foreach (INotifyCollectionChanged oldItem in oldItems)
					{
						oldItem.CollectionChanged -= HandleChildCollectionChanged;
					}
				}

				var newItems = Items;
				if (newItems != null)
				{
					foreach (var newItem in newItems)
					{
						newItem.CollectionChanged -= HandleChildCollectionChanged;
						newItem.CollectionChanged += HandleChildCollectionChanged;
					}
				}
			}
		}

		void HandleChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnChildCollectionChanged(new NotifyChildCollectionChangedEventArgs(sender, e));
		}

		public event EventHandler<NotifyChildCollectionChangedEventArgs> ChildCollectionChanged;

		protected virtual void OnChildCollectionChanged(NotifyChildCollectionChangedEventArgs e)
		{
			ChildCollectionChanged?.Invoke(this, e);
		}
	}

	public class NotifyChildCollectionChangedEventArgs : EventArgs
	{
		public object ChildCollection { get; private set; }
		public NotifyCollectionChangedEventArgs ChildCollectionEventArgs { get; private set; }

		public NotifyChildCollectionChangedEventArgs(
			object childCollection, NotifyCollectionChangedEventArgs childCollectionEventArgs)
		{
			ChildCollection = childCollection;
			ChildCollectionEventArgs = childCollectionEventArgs;
		}
	}
}