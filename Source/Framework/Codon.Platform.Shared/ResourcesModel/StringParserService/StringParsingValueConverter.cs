#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2015-04-04 16:50:57Z</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;
using System;
using Codon.Logging;
using Codon.Services;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#elif __ANDROID__ || __IOS__
using System.Globalization;
using Codon.MissingTypes.System.Windows.Data;
#elif WPF || WPF_CORE
using System.Globalization;
using System.Windows;
using System.Windows.Data;
#endif

namespace Codon.UI.Elements.ValueConverters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> that parses
	/// string using the <see cref="IStringParserService"/>,
	/// allowing you to leverage the String Parser service
	/// from data-bindings.
	/// </summary>
	public class StringParsingValueConverter : IValueConverter
	{
		IStringParserService stringParserService;

		object Convert(object value, Type targetType, object parameter)
		{
			if (value == null)
			{
				return null;
			}

			if (stringParserService == null)
			{
				stringParserService = Dependency.Resolve<IStringParserService>();
			}

			if (value is Enum)
			{
				string stringEnumValue = value.ToString();
				string keyWithLocalizeTag = string.Format("${{l:Enum_{0}_{1}}}", value.GetType().Name.Replace('.', '_'), stringEnumValue);
				var result = stringParserService.Parse(keyWithLocalizeTag);
				if (string.IsNullOrWhiteSpace(result) || result == keyWithLocalizeTag)
				{
#if DEBUG
					var log = Dependency.Resolve<ILog>();
					log.Debug("Localized resource not found: " + keyWithLocalizeTag);
#endif
					return value;
				}

				return result;
			}
			else
			{
				return stringParserService.Parse(value.ToString());
			}
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
