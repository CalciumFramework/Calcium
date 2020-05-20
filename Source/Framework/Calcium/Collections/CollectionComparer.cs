#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Calcium.Collections
{
	/// <summary>
	/// Comparison related extension methods for collection types.
	/// </summary>
	public static class ComparerExtensions
	{
		/// <summary>
		/// Compares the objects in one dictionary with another.
		/// </summary>
		/// <param name="dictionary1">
		/// The dictionary that is compared with dictionary2.</param>
		/// <param name="dictionary2">
		/// The dictionary that is compared with dictionary1.</param>
		/// <param name="equalityFunc">An optional <c>Func</c> 
		/// to test for dictionary item equality. 
		/// If <c>null</c>, <c>object.Equals</c> is used.</param>
		/// <returns><c>true</c> if the collections 
		/// have the same set of keys and values.</returns>
		public static bool IsEqualDictionary(
			this IDictionary dictionary1,
			IDictionary dictionary2,
			Func<object, object, bool> equalityFunc = null)
		{
			AssertArg.IsNotNull(dictionary1, nameof(dictionary1));

			return CollectionComparer.AreEqualDictionaries(
						dictionary1, dictionary2, equalityFunc);
		}

		/// <summary>
		/// Compares two lists for equivalence.
		/// Objects in both lists must be the same
		/// and in the same position.
		/// </summary>
		/// <param name="list1">The first list to compare.
		/// Can not be null.</param>
		/// <param name="list2">The second list to compare.
		/// Can be null.</param>
		/// <returns><c>true</c> if the collections 
		/// are equal; <c>false</c> otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// Raised if <c>list1</c> is <c>null</c>.</exception>
		public static bool IsEqualList(
			this IList list1,
			IList list2)
		{
			AssertArg.IsNotNull(list1, nameof(list1));

			return CollectionComparer.AreEqualLists(list1, list2);
		}
	}

	/// <summary>
	/// This class is used to compare multiple collections
	/// for equality. It is used to iterate over collections
	/// comparing individual items within the collections.
	/// </summary>
	public static class CollectionComparer
	{
		/// <summary>
		/// Compares the objects in one dictionary with another.
		/// </summary>
		/// <param name="dictionary1">
		/// The dictionary that is compared with dictionary2.</param>
		/// <param name="dictionary2">
		/// The dictionary that is compared with dictionary1.</param>
		/// <returns><c>true</c> if the collections 
		/// have the same set of keys and values.</returns>
		public static bool AreEqualDictionariesGeneric<TKey, TValue>(
			IDictionary<TKey, TValue> dictionary1, 
			IDictionary<TKey, TValue> dictionary2)
		{
			if (dictionary1 == dictionary2)
			{
				return true;
			}

			if (dictionary1 == null || dictionary2 == null)
			{
				return false;
			}

			if (dictionary1.Count != dictionary2.Count)
			{
				return false;
			}

			var valueComparer = EqualityComparer<TValue>.Default;

			foreach (var pair in dictionary1)
			{
				if (!dictionary2.TryGetValue(pair.Key, out TValue value2))
				{
					return false;
				}

				if (!valueComparer.Equals(pair.Value, value2))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Compares the objects in one dictionary with another.
		/// </summary>
		/// <param name="dictionary1">
		/// The dictionary that is compared with dictionary2.</param>
		/// <param name="dictionary2">
		/// The dictionary that is compared with dictionary1.</param>
		/// <param name="equalityFunc">An optional <c>Func</c> 
		/// to test for dictionary item equality. 
		/// If <c>null</c>, <c>object.Equals</c> is used.</param>
		/// <returns><c>true</c> if the collections 
		/// have the same set of keys and values.</returns>
		public static bool AreEqualDictionaries(
			IDictionary dictionary1, IDictionary dictionary2, 
			Func<object, object, bool> equalityFunc = null)
		{
			if (dictionary1 == dictionary2)
			{
				return true;
			}

			if (dictionary1 == null || dictionary2 == null)
			{
				return false;
			}

			if (dictionary1.Count != dictionary2.Count)
			{
				return false;
			}

			foreach (object key in dictionary1.Keys)
			{
				if (!dictionary2.Contains(key))
				{
					return false;
				}

				var value1 = dictionary1[key];
				var value2 = dictionary2[key];

				if (equalityFunc == null)
				{
					if (!Equals(value1, value2))
					{
						return false;
					}
				}
				else
				{
					if (!equalityFunc(value1, value2))
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Compares two collections for equivalence while
		/// ignoring the order of items.
		/// </summary>
		/// <typeparam name="TValue">The generic type of 
		/// both collections.</typeparam>
		/// <param name="collection1"></param>
		/// <param name="collection2"></param>
		/// <returns><c>true</c> if the collections
		/// contain the same elements; <c>false</c> otherwise.
		/// </returns>
		public static bool HaveSameElements<TValue>(
			ICollection<TValue> collection1, 
			ICollection<TValue> collection2)
		{
			if (collection1 == collection2)
			{
				return true;
			}

			if (collection1 == null || collection2 == null)
			{
				return false;
			}

			if (collection1.Count != collection2.Count)
			{
				return false;
			}

			foreach (var item in collection1)
			{
				if (!collection2.Contains(item))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Compares two lists for equivalence.
		/// Objects in both lists must be the same
		/// and in the same position.
		/// </summary>
		/// <param name="list1">The first list to compare.
		/// Can be null.</param>
		/// <param name="list2">The second list to compare.
		/// Can be null.</param>
		/// <returns><c>true</c> if the collections 
		/// are equal; <c>false</c> otherwise.</returns>
		public static bool AreEqualLists(
			IList list1, IList list2)
		{
			if (list1 == list2)
			{
				return true;
			}

			if (list1 == null || list2 == null)
			{
				return false;
			}

			int listSize = list1.Count;

			if (list1.Count != list2.Count)
			{
				return false;
			}

			for (int i = 0; i < listSize; i++)
			{
				if (!Equals(list1[i], list2[i]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
