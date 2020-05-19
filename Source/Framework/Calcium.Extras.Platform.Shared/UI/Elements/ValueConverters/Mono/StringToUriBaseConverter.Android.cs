#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 10:33:28Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Globalization;

using Calcium.MissingTypes.System.Windows.Data;

namespace Calcium.UI.Elements
{
	public class StringToUriBaseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}

			string url = value.ToString();
			return new Uri(value.ToString(), UriKind.Relative);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
#endif
