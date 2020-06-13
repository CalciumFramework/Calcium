#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-01-31 17:53:55Z</CreationDate>
</File>
*/
#endregion

using System;

#if __ANDROID__ || __IOS__
using System.Globalization;
using Calcium.MissingTypes.System.Windows.Data;
#elif NETFX_CORE
using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
using System.Globalization;
#endif

namespace Calcium.UI.Elements
{
	/// <summary>
	/// This <see cref="IValueConverter"/>
	/// calls through to <see cref="string.Format(string,object)"/>.
	/// The parameter is the format, or second parameter, 
	/// while the converter value is the first parameter.
	/// </summary>
	public class StringFormatConverter : IValueConverter
	{
		object Convert(object value, Type targetType, object parameter)
		{
			if (parameter == null || value == null)
			{
				return value;
			}
			string format = parameter.ToString();

			return string.Format(format, value);
		}

		object ConvertBack(object value, Type targetType, object parameter)
		{
			throw new NotSupportedException();
		}

#if __ANDROID__ || __IOS__

		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter);
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertBack(value, targetType, parameter);
		}
#elif !NETFX_CORE

		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter);
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertBack(value, targetType, parameter);
		}
#else
		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return Convert(value, targetType, parameter);
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return ConvertBack(value, targetType, parameter);
		}
#endif
	}
}
