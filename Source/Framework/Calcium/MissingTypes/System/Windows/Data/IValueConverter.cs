#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Globalization;

namespace Calcium.MissingTypes.System.Windows.Data
{
	/// <summary>
	/// Converts a value, normally supplied via a data binding,
	/// to another value. This interface corresponds to the
	/// <c>IValueConverter</c> that exists in UWP and WPF.
	/// This interface is present in this library and not
	/// in a platform specific library due to the dependence
	/// on it by the UI.Data library and the Framework platform
	/// libraries.
	/// </summary>
	public interface IValueConverter
	{
		object Convert(
			object value, 
			Type targetType, 
			object parameter, 
			CultureInfo culture);

		object ConvertBack(
			object value, 
			Type targetType, 
			object parameter, 
			CultureInfo culture);
	}
}
