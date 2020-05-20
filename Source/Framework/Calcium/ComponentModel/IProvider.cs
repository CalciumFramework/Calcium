#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-03-27 14:51:48Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.ComponentModel
{
	/// <summary>
	/// A class implementing this interface
	/// is able to resolve an object of the specified type.
	/// </summary>
	/// <typeparam name="T">The type of the provided item.</typeparam>
	public interface IProvider<out T> where T : class
	{
		/// <summary>
		/// The item that is provided.
		/// </summary>
		T ProvidedItem
		{
			get;
		}
	}
}
