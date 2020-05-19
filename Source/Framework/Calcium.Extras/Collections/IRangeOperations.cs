#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-01-10 16:27:11Z</CreationDate>
</File>
*/
#endregion

using System.Collections;

namespace Codon.Collections
{
	/// <summary>
	/// Range operations allow you to add, remove, or replace
	/// items from a collection whilst not raising collection
	/// changed events.
	/// </summary>
	public interface IRangeOperations
	{
		void AddRange(IEnumerable items);
		void RemoveRange(IEnumerable items);

		void ReplaceRange(int startIndex, IEnumerable newItems);

		void ReplaceRange(IEnumerable oldItems, IEnumerable newItems);

		void InsertRange(IEnumerable fromList, int startIndex);
	}
}
