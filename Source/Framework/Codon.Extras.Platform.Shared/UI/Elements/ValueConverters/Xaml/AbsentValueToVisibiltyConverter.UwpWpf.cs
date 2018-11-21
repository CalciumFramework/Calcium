#if WPF || WINDOWS_UWP
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-15 13:23:30Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
#if NETFX_CORE
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using System.Globalization;
#else
using System.Globalization;
using System.Windows;
using System.Windows.Data;
#endif

namespace Codon.UI.Elements
{
	/// <summary>
	/// Converts a boolean value to a <see cref="Visibility"/> value, 
	/// and vice versa.
	/// </summary>
	public class AbsentValueToVisibiltyConverter : IValueConverter
	{
		object Convert(object value, Type targetType, object parameter)
		{
			string paramValue = (string)parameter ?? string.Empty;
			bool makeVisible = string.Compare(
				paramValue.Trim(), "visible", StringComparison.OrdinalIgnoreCase) == 0;

			var absentVisibility = makeVisible ? Visibility.Visible : Visibility.Collapsed;
			var presentVisibility = makeVisible ? Visibility.Collapsed : Visibility.Visible;

			if (value == null)
			{
				return absentVisibility;
			}

			var collection = value as ICollection;
			if (collection != null)
			{
				if (collection.Count < 1)
				{
					return absentVisibility;
				}

				return presentVisibility;
			}

			if (Double.TryParse(System.Convert.ToString(value, CultureInfo.InvariantCulture),
				NumberStyles.Any,
				NumberFormatInfo.InvariantInfo,
				out double number))
			{
				return number > 0 ? presentVisibility : absentVisibility;
			}

			if (string.IsNullOrWhiteSpace(value.ToString()))
			{
				return absentVisibility;
			}

			return presentVisibility;
		}

		object ConvertBack(object value, Type targetType, object parameter)
		{
			throw new NotImplementedException();
		}

#if !NETFX_CORE
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
#endif