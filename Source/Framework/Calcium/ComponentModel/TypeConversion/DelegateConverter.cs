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

using System;

namespace Calcium.ComponentModel
{
	/// <summary>
	/// This class provides a Func based implementation
	/// of the <see cref="IConverter"/> interface.
	/// </summary>
	public class DelegateConverter : IConverter
	{
		readonly Func<object, object> convertFunc;

		public DelegateConverter(Func<object, object> convertFunc)
		{
			this.convertFunc = AssertArg.IsNotNull(convertFunc, nameof(convertFunc));
		}

		public object Convert(object fromValue)
		{
			var result = convertFunc(fromValue);
			return result;
		}
	}
}
