#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Calcium.UI.Data;

namespace Calcium.UI.Adapters
{
	public interface IBindableListAdapter
	{
		int GetItemPosition(object item);
		object GetItem(int position);
	}

	public class BindableListAdapter<TItem> : BaseAdapter<TItem>, 
		IBindableListAdapter
	{
		readonly IList<TItem> list;
		readonly int layoutId;
		readonly INotifyCollectionChanged observableCollection;
		readonly LayoutInflater inflater;

		public BindableListAdapter(IList<TItem> list, int layoutId)
		{
			this.list = list;
			this.layoutId = layoutId;

			observableCollection = list as INotifyCollectionChanged;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged += HandleCollectionChanged;
			}

			Context context = ApplicationContextHolder.Context;
			//var context = Dependency.Resolve<Activity>();
					
			inflater = LayoutInflater.From(context);
		}

		public BindableListAdapter(System.IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base(handle, transfer)
		{
		}

		int IBindableListAdapter.GetItemPosition(object item)
		{
			int result = list?.IndexOf((TItem)item) ?? -1;
			return result;
		}

		object IBindableListAdapter.GetItem(int position)
		{
			return list[position];
		}

		void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			NotifyDataSetChanged();
		}

		public override int Count => list.Count;

		public override long GetItemId(int position)
		{
			return position;
			//			var item = list[position];
			//			/* TODO: Provide interface for this. */
			//			return item.GetHashCode();
		}

		public override TItem this[int index] => list[index];

		readonly Dictionary<View, XmlBindingApplicator> bindingsDictionary 
					= new Dictionary<View, XmlBindingApplicator>();

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			//View view = convertView ?? inflater.Inflate(layoutId, parent, false);
			View view;
			try
			{
				view = convertView ?? inflater.Inflate(layoutId, parent, false);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}

			TItem item = this[position];

			XmlBindingApplicator applicator;
			if (convertView == null 
				|| !bindingsDictionary.TryGetValue(view, out applicator))
			{
				applicator = new XmlBindingApplicator();
			}
			else
			{
				applicator.RemoveBindings();
			}

			applicator.ApplyBindings(view, item, layoutId);

			bindingsDictionary[view] = applicator;

			return view;
		}

		protected override void Dispose(bool disposing)
		{
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged -= HandleCollectionChanged;
			}
			
			foreach (var operation in bindingsDictionary.Values)
			{
				operation.RemoveBindings();
			}

			bindingsDictionary.Clear();

			base.Dispose(disposing);
		}

		public override bool AreAllItemsEnabled()
		{
			return false;
		}

		public override bool IsEnabled(int position)
		{
			var item = this[position] as IBindableListItem;
			return item == null || item.Enabled;
		}
	}
}
