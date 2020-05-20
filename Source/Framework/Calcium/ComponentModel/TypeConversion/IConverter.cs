#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-03-25 20:41:44Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.ComponentModel
{
	/// <summary>
	/// Converts a value of type <c>TFrom</c> 
	/// to a value of type <c>TTo</c>.
	/// </summary>
	/// <typeparam name="TFrom">The type converted from.</typeparam>
	/// <typeparam name="TTo">The type converted to.</typeparam>
	public interface IConverter<TFrom, TTo>
	{
		TTo Convert(TFrom fromValue);
	}

	/// <summary>
	/// Simple implementation of <c>IConverter</c>, using the object type.
	/// </summary>
	public interface IConverter : IConverter<object, object>
	{
		/* Intentionally left blank. */
	}
}
