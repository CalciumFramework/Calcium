#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-04-08 19:12:29Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;

namespace Calcium.UI.Data
{
	public class StaticResourceRegistry : IStaticResourceRegistry
	{
		readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

		public object this[string key]
		{
			get
			{
				object result;
				dictionary.TryGetValue(key, out result);
				return result;
			}
			set => dictionary[key] = value;
		}

		public void Clear() => dictionary.Clear();
	}
}
