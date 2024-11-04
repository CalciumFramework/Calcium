#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-04 11:27:13Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Calcium.ComponentModel.Experimental
{
	public interface IRegistry<TKey, TValue>
	{
		void SetValue(TKey key, TValue value);
		bool TryGetValue(TKey key, out TValue? value);
		bool TryRemove(TKey key, out TValue? value);

		IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary { get; }
	}

	public class Registry<TKey, TValue> : IRegistry<TKey, TValue>
	{
		readonly ConcurrentDictionary<TKey, TValue> converters = new();

		readonly ReadOnlyDictionary<TKey, TValue> readOnlyDictionary;
		public IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary => readOnlyDictionary;

		public TValue this[TKey tagName] => converters[tagName];

		public Registry()
		{
			readOnlyDictionary = new(converters);
		}

		public virtual void SetValue(TKey key, TValue value)
		{
			converters[key] = value;
		}

		public virtual bool TryGetValue(TKey key, out TValue? value)
		{
			return converters.TryGetValue(key, out value);
		}

		public virtual bool TryRemove(TKey key, out TValue? value)
		{
			return converters.TryRemove(key, out value);
		}
	}
}
