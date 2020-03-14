#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-24 21:16:49Z</CreationDate>
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
using System.Globalization;
using System.Windows.Data;
#endif

namespace Codon.UI.Elements
{
	/// <inheritdoc />
	public class CaseValueConverter : IValueConverter
	{
		object Convert(object value, Type targetType, object parameter)
		{
			if (value == null)
			{
				return null;
			}

			if (parameter == null || parameter.ToString().ToLowerInvariant().Contains("lower"))
			{
				return value.ToString().ToLower();
			}

			return value.ToString().ToUpper();
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
