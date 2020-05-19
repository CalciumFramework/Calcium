#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2020, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2020-01-02 22:36:07Z</CreationDate>
</File>
*/
#endregion

using System.Runtime.CompilerServices;

namespace Codon.Collections
{
	static class ConditionalWeakTableExtensions
	{
		public static void SetValue<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key, TValue value) 
			where TKey : class where TValue : class
		{
			AssertArg.IsNotNull(table, nameof(table));
			table.Remove(key);
			table.Add(key, value);
		}
	}
}
