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

namespace Calcium.Collections
{
	/// <summary>
	/// This class provides an efficient reverse lookup mechanism.
	/// </summary>
	/// <typeparam name="TKey1">First key type.</typeparam>
	/// <typeparam name="TKey2">Second key type.</typeparam>
	public class Map<TKey1, TKey2>
	{
		readonly Dictionary<TKey1, TKey2> forwardDictionary 
			= new Dictionary<TKey1, TKey2>();
		readonly Dictionary<TKey2, TKey1> reverseDictionary 
			= new Dictionary<TKey2, TKey1>();

		public Map()
		{
			Key1Index = new Index<TKey1, TKey2>(forwardDictionary);
			Key2Index = new Index<TKey2, TKey1>(reverseDictionary);
		}

		public class Index<TKey, TValue>
		{
			readonly Dictionary<TKey, TValue> dictionary;

			public Index(Dictionary<TKey, TValue> dictionary)
			{
				this.dictionary = dictionary;
			}

			public TValue this[TKey index]
			{
				get => dictionary[index];
				set => dictionary[index] = value;
			}
		}

		public void Add(TKey1 t1, TKey2 t2)
		{
			forwardDictionary.Add(t1, t2);
			try
			{
				reverseDictionary.Add(t2, t1);
			}
			catch (Exception)
			{
				try
				{
					forwardDictionary.Remove(t1);
				}
				catch (Exception)
				{
					/* Ignore. */
				}

				throw;
			}
		}

		public Index<TKey1, TKey2> Key1Index { get; private set; }

		public Index<TKey2, TKey1> Key2Index { get; private set; }
	}
}
