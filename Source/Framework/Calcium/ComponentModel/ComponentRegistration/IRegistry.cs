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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Calcium.ComponentModel.Experimental
{
	/// <summary>
	/// Represents a registry interface for managing key-value pairs with thread-safe operations.
	/// </summary>
	/// <typeparam name="TKey">The type of the key for entries in the registry.</typeparam>
	/// <typeparam name="TValue">The type of the entry value.</typeparam>
	public interface IRegistry<TKey, TValue>
	{
		/// <summary>
		/// Adds or updates the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key to add or update.</param>
		/// <param name="value">The value to associate with the key.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <c>null</c>.</exception>
		void SetValue(TKey key, TValue value);

		/// <summary>
		/// Attempts to get the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key whose value to retrieve.</param>
		/// <param name="value">When this method returns, contains the value associated with the specified key, if found; otherwise, the default value for the type of the value parameter.</param>
		/// <returns><c>true</c> if the key was found; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <c>null</c>.</exception>
		bool TryGetValue(TKey key, out TValue? value);

		/// <summary>
		/// Attempts to remove the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key whose value to remove.</param>
		/// <param name="value">When this method returns, contains the value removed, if found; otherwise, the default value for the type of the value parameter.</param>
		/// <returns><c>true</c> if the key was found and removed; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <c>null</c>.</exception>
		bool TryRemove(TKey key, out TValue? value);

		/// <summary>
		/// Gets a read-only view of the key-value pairs in the registry.
		/// </summary>
		IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary { get; }
	}

	/// <summary>
	/// Default implementation of the <see cref="IRegistry{TKey, TValue}"/> interface,
	/// providing thread-safe operations for managing key-value pairs.
	/// </summary>
	public class Registry<TKey, TValue> : IRegistry<TKey, TValue>
	{
		readonly ConcurrentDictionary<TKey, TValue> converters = new();

		readonly ReadOnlyDictionary<TKey, TValue> readOnlyDictionary;

		/// <inheritdoc />
		public IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary => readOnlyDictionary;

		public TValue this[TKey tagName] => converters[tagName];

		public Registry()
		{
			readOnlyDictionary = new(converters);
		}

		/// <inheritdoc />
		public virtual void SetValue(TKey key, TValue value)
		{
			converters[key] = value;
		}

		/// <inheritdoc />
		public virtual bool TryGetValue(TKey key, out TValue? value)
		{
			return converters.TryGetValue(key, out value);
		}

		/// <inheritdoc />
		public virtual bool TryRemove(TKey key, out TValue? value)
		{
			return converters.TryRemove(key, out value);
		}
	}
}
