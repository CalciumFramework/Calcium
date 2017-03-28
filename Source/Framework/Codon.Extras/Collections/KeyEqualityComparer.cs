#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-10-15 15:27:08Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Codon.Collections
{
	/// <summary>
	/// An implementation of <see cref="IEqualityComparer{T}"/>
	/// that allows comparison of two items using a Func.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class KeyEqualityComparer<T> : IEqualityComparer<T>
	{
		readonly Func<T, object> resolveKeyFunc;

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="KeyEqualityComparer&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="resolveKeyFunc">
		/// A Func to resolve the key of the instance.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified resolveKeyFunc is <c>null</c>.</exception>
		public KeyEqualityComparer(Func<T, object> resolveKeyFunc)
		{
			this.resolveKeyFunc = AssertArg.IsNotNull(
									resolveKeyFunc, nameof(resolveKeyFunc));;
		}

		public bool Equals(T x, T y)
		{
			return resolveKeyFunc(x).Equals(resolveKeyFunc(y));
		}

		public int GetHashCode(T obj)
		{
			var key = resolveKeyFunc(obj);
			return key?.GetHashCode() ?? 0;
		}
	}
}
