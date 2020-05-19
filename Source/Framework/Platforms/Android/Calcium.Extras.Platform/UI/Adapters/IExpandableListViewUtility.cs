#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-21 23:31:22Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.UI.Adapters
{
	public interface IExpandableListViewUtility
	{
		int GetViewType(object listItemDataContext, int groupIndex, int childPosition);
		int ChildTypeCount { get; }
		int GroupTypeCount { get; }
		bool HasStableIds { get; }

		long GetViewId(object listItemDataContext, int groupIndex, int childPosition);

		long GetGroupId/*<TItem>*/(/*GroupedList<TItem> group, */int groupIndex);
		int GetGroupType(int groupIndex);
	}
}
