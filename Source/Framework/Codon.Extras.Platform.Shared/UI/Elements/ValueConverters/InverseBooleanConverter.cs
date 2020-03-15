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

#if __ANDROID__ || __IOS__
using System.Globalization;
using Codon.MissingTypes.System.Windows.Data;
#elif NETFX_CORE
using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
using System.Globalization;
#endif

namespace Codon.UI.Elements
{
	/// <summary>
	/// Negates a boolean value, or its string representation.
	/// </summary>
#if WPF
	[ValueConversion(typeof(bool), typeof(bool))]
#endif
	public class InverseBooleanConverter : IValueConverter
	{
		object Convert(object value, Type targetType, object parameter)
		{
			if (targetType != typeof(bool))
			{
				throw new InvalidOperationException(
					"The target must be of type bool.");
			}

			return !(bool)value;
		}

		object ConvertBack(object value, Type targetType, object parameter)
		{
			if (value == null)
			{
				throw new NotImplementedException();
			}

			if (targetType != typeof(bool))
			{
				throw new InvalidOperationException(
					"The target must be of type bool.");
			}

			return !(bool)value;
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
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return Convert(value, targetType, parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return ConvertBack(value, targetType, parameter);
		}
#endif
	}
}
