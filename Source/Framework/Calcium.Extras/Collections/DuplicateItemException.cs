#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-03-23 12:09:19Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.Collections
{
	/// <summary>
	/// Occurs when an attempt is made to add 
	/// a duplicate item to a set of items.
	/// </summary>
	/// <typeparam name="TItem">
	/// The type of the collection item.</typeparam>
	public class DuplicateItemException<TItem> : Exception
	{
		public TItem ExistingItem { get; private set; }
		public TItem AddedItem { get; private set; }

		public DuplicateItemException(
			TItem existingItem, TItem addedItem, string message = null)
			: base(message)
		{
			ExistingItem = existingItem;
			AddedItem = addedItem;
		}
	}

	/// <summary>
	/// Occurs when an attempt is made to add 
	/// a duplicate item to a set of items.
	/// </summary>
	public class DuplicateItemException : DuplicateItemException<object>
	{
		public DuplicateItemException(
			object existingItem, object addedItem, string message = null)
			: base(existingItem, addedItem, message)
		{
			/* Intentionally left blank. */
		}
	}
}
