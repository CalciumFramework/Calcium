#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-03 23:41:18Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Calcium.ComponentModel;
using Calcium.InversionOfControl;

namespace Calcium.ResourcesModel.Experimental
{
	[DefaultType(typeof(ConverterRegistry), Singleton = true)]
	public interface IConverterRegistry
	{
		void SetConverter(string tagName, IConverter converter);
		bool TryGetConverter(string tagName, out IConverter? converter);
		bool TryRemoveConverter(string tagName, out IConverter? converter);
	}

	public class ConverterRegistry : IConverterRegistry
	{
		readonly ConcurrentDictionary<string, IConverter> converters = new();

		public IConverter this[string tagName] => converters[tagName];

		readonly ReadOnlyDictionary<string, IConverter> readOnlyDictionary;

		public ConverterRegistry()
		{
			readOnlyDictionary = new(converters);
		}

		public IReadOnlyDictionary<string, IConverter> ReadOnlyDictionary => readOnlyDictionary;

		public void SetConverter(string tagName, IConverter converter)
		{
			converters[tagName] = converter;
		}

		public bool TryGetConverter(string tagName, out IConverter? converter)
		{
			return converters.TryGetValue(tagName, out converter);
		}

		public bool TryRemoveConverter(string tagName, out IConverter? converter)
		{
			return converters.TryRemove(tagName, out converter);
		}
	}
}