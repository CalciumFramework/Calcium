#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-21 23:31:06Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Android.Content;
using Android.Views;
using Android.Widget;

using Calcium.Collections;
using Calcium.Logging;
using Calcium.UI.Data;
using Object = Java.Lang.Object;

namespace Calcium.UI.Adapters
{
	public class BindableExpandableListAdapter<TItem> : BaseExpandableListAdapter
	{
		readonly IList<GroupedList<TItem>> groupings;
		readonly int groupHeaderLayoutId;
		readonly Func<TItem, int> getLayoutIdFunc;
		readonly Context context;
		readonly LayoutInflater inflater;

		class BoundViewData
		{
			public BoundViewData(
				XmlBindingApplicator applicator, 
				object dataContext)
			{
				Applicator = applicator;
				DataContext = dataContext;
			}

			public XmlBindingApplicator Applicator { get; private set; }

			public object DataContext { get; private set; }
		}

		readonly Dictionary<View, BoundViewData> bindingsDictionary
					= new Dictionary<View, BoundViewData>();

		readonly Dictionary<object, View> reverseLookup
					= new Dictionary<object, View>(); 

		readonly IObservableMultiList observableGroupings;
		readonly INotifyCollectionChanged observableCollection;

		public BindableExpandableListAdapter(
			IList<GroupedList<TItem>> groupings, 
			int groupHeaderLayoutId, 
			Func<TItem, int> getLayoutIdFunc)
		{
			this.groupings = groupings;
			this.groupHeaderLayoutId = groupHeaderLayoutId;
			this.getLayoutIdFunc = AssertArg.IsNotNull(getLayoutIdFunc, nameof(getLayoutIdFunc));

			observableGroupings = groupings as IObservableMultiList;
			if (observableGroupings != null)
			{
				observableGroupings.ChildCollectionChanged += HandleGroupingCollectionChanged;
			}

			observableCollection = groupings as INotifyCollectionChanged;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged += HandleCollectionChanged;
			}

			context = ApplicationContextHolder.Context;
			inflater = LayoutInflater.From(context);
		}

		protected override void Dispose(bool disposing)
		{
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged -= HandleCollectionChanged;
			}

			if (observableGroupings != null)
			{
				observableGroupings.ChildCollectionChanged -= HandleGroupingCollectionChanged;
			}

			base.Dispose(disposing);
		}

		void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var changeAction = e.Action;
			if (changeAction == NotifyCollectionChangedAction.Remove)
			{
				List<View> removeKeys = new List<View>();
				foreach (object oldItem in e.OldItems)
				{
					reverseLookup.TryGetValue(oldItem, out View view);
					reverseLookup.Remove(oldItem);

					if (view != null)
					{
						removeKeys.Add(view);
						continue;
					}
				}

				foreach (View removeKey in removeKeys)
				{
					bindingsDictionary.Remove(removeKey);
				}
			}
			else if (changeAction == NotifyCollectionChangedAction.Reset)
			{
				UnbindAll();
			}

			NotifyDataSetChanged();
		}

		void UnbindAll()
		{
			foreach (BoundViewData viewData in bindingsDictionary.Values)
			{
				try
				{
					viewData.Applicator.RemoveBindings();
				}
				catch (Exception ex)
				{
					var log = Dependency.Resolve<ILog>();
					log.Debug("Exception raised unbinding view.", ex);
				}
			}

			bindingsDictionary.Clear();
			reverseLookup.Clear();
		}

		void HandleGroupingCollectionChanged(
			object sender, NotifyChildCollectionChangedEventArgs e)
		{
			NotifyDataSetChanged();
		}

		public override int GetChildrenCount(int groupPosition)
		{
			if (groupings == null || groupings.Count < 1)
			{
				return 0;
			}

			return groupings[groupPosition].Count;
		}

		public override View GetChildView(
			int groupPosition, int childPosition, 
			bool isLastChild, View convertView, ViewGroup parent)
		{
			var view = convertView;

			var item = groupings[groupPosition][childPosition];
			int layoutId = getLayoutIdFunc(item);

			if (view == null)
			{
				view = inflater.Inflate(/*Resource.Layout.OptionTemplate_Boolean*/layoutId, null);
			}
			else
			{
				if (bindingsDictionary.TryGetValue(view, out BoundViewData viewData))
				{
					var currentDataContext = viewData.DataContext;

					if (currentDataContext != null && currentDataContext.Equals(item)
						&& viewData.Applicator.HasBindings)
					{
						return view;
					}

					viewData.Applicator.RemoveBindings();

					/* If the item is not of the same type, do not reuse the view. */
					if (viewData.DataContext.GetType() != item.GetType())
					{
						view = inflater.Inflate(layoutId, null);
					}
				}
			}

			var applicator = new XmlBindingApplicator();
			applicator.ApplyBindings(view, item, layoutId);
			bindingsDictionary[view] = new BoundViewData(applicator, item);
			reverseLookup[item] = view;
			
			return view;
		}
		
		public override Object GetGroup(int groupPosition)
		{
			return null;
		}

		public override long GetGroupId(int groupPosition)
		{
			if (ViewUtility != null)
			{
				return ViewUtility.GetGroupId(groupPosition);
			}

			return groupPosition * 10000;
		}

		public override View GetGroupView(
			int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			var view = convertView;

			var group = groupings[groupPosition];

			if (view == null)
			{
				view = inflater.Inflate(groupHeaderLayoutId, null);
			}
			else
			{
				if (bindingsDictionary.TryGetValue(
					view, out BoundViewData viewData))
				{
					var currentDataContext = viewData.DataContext;

					if (currentDataContext != null && currentDataContext.Equals(group)
						&& viewData.Applicator.HasBindings)
					{
						return view;
					}

					viewData.Applicator.RemoveBindings();

					if (viewData.DataContext.GetType() != group.GetType())
					{
						view = inflater.Inflate(groupHeaderLayoutId, null);
					}
				}
			}

			var applicator = new XmlBindingApplicator();
			applicator.ApplyBindings(view, group, groupHeaderLayoutId);

			bindingsDictionary[view] = new BoundViewData(applicator, group);
			reverseLookup[group] = view;

			return view;
		}

		public override bool IsChildSelectable(
			int groupPosition, int childPosition)
		{
			return true;
		}

		public override int GroupCount => groupings?.Count ?? 0;

		public override bool HasStableIds => ViewUtility?.HasStableIds ?? false;

		public override Object GetChild(int groupPosition, int childPosition)
		{
			return groupings[groupPosition][childPosition] as Object;
		}

		public object GetCollectionChild(int groupPosition, int childPosition)
		{
			return groupings[groupPosition][childPosition];
		}

		public override long GetChildId(int groupPosition, int childPosition)
		{
			if (ViewUtility != null)
			{
				object item = GetCollectionChild(groupPosition, childPosition);
				return ViewUtility.GetViewId(item, groupPosition, childPosition);
			}

			return groupPosition * 10000 + childPosition;
		}

		public override int GetChildType(int groupPosition, int childPosition)
		{
			if (ViewUtility != null)
			{
				object item = GetCollectionChild(groupPosition, childPosition);

				return ViewUtility.GetViewType(item, groupPosition, childPosition);
			}

			return base.GetChildType(groupPosition, childPosition);
		}

		public override int ChildTypeCount
		{
			get
			{
				if (ViewUtility != null)
				{
					return ViewUtility.ChildTypeCount;
				}

				return base.ChildTypeCount;
			}
		}

		public override int GetGroupType(int groupPosition)
		{
			if (ViewUtility != null)
			{
				return ViewUtility.GetGroupType(groupPosition);
			}

			return base.GetGroupType(groupPosition);
		}

		public override int GroupTypeCount
		{
			get
			{
				if (ViewUtility != null)
				{
					return ViewUtility.GroupTypeCount;
				}

				return base.GroupTypeCount;
			}
		}

		public IExpandableListViewUtility ViewUtility { get; set; }
	}
}
