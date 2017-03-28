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
	<CreationDate>2011-12-30 12:48:17Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Globalization;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;
#else
using System.Windows.Data;
using System.Windows.Media;
#endif

namespace Codon.UI.Elements
{
	public class ColorToBrushConverter : IValueConverter
	{
		object Convert(object value, Type targetType, object parameter)
		{
			if (value == null)
			{
				return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
			}

			if (value is Color)
			{
				return new SolidColorBrush((Color)value);
			}

			return new SolidColorBrush(ParseColorString(value.ToString()));
		}

		static Color ParseColorString(string colorString)
		{
			var offset = colorString.StartsWith("#") ? 1 : 0;

			var a = Byte.Parse(colorString.Substring(0 + offset, 2), NumberStyles.HexNumber);
			var r = Byte.Parse(colorString.Substring(2 + offset, 2), NumberStyles.HexNumber);
			var g = Byte.Parse(colorString.Substring(4 + offset, 2), NumberStyles.HexNumber);
			var b = Byte.Parse(colorString.Substring(6 + offset, 2), NumberStyles.HexNumber);

			return Color.FromArgb(a, r, g, b);
		}

		object ConvertBack(object value, Type targetType, object parameter)
		{
			throw new NotImplementedException();
		}

#if !NETFX_CORE
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter);
		}

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
#endif