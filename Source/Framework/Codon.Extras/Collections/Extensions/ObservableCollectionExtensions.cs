#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-19 22:53:15Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Codon.Collections
{
	/// <summary>
	/// This class provides extension methods for 
	/// <see cref="ObservableCollection{T}"/>.
	/// </summary>
	public static class ObservableCollectionExtensions
	{
		public static int RemoveAll<T>(
			this ObservableCollection<T> collection, Func<T, bool> condition)
		{
			var itemsToRemove = collection.Where(condition).ToList();

			collection.RemoveRange(itemsToRemove);

			return itemsToRemove.Count;
		}
	}
}