#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-04-08 19:12:37Z</CreationDate>
</File>
*/
#endregion

using Calcium.InversionOfControl;

namespace Calcium.UI.Data
{
	[DefaultType(typeof(StaticResourceRegistry), Singleton = true)]
	public interface IStaticResourceRegistry
	{
		object this[string key]
		{
			get;
			set;
		}

		void Clear();
	}
}
