﻿#if __ANDROID__

using System;
using System.Collections;
using System.Globalization;

#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using Android.Views;
using Codon.MissingTypes.System.Windows.Data;

namespace Codon.UI.Elements
{
	/// <summary>
	/// Converts a boolean value to a <see cref="ViewStates"/> value, 
	/// and vice versa.
	/// </summary>
	public class AbsentValueToVisibiltyConverter : IValueConverter
	{
		object Convert(object value, Type targetType, object parameter)
		{
			string paramValue = (string)parameter ?? string.Empty;
			bool makeVisible = string.Compare(
				paramValue.Trim(), "visible", StringComparison.OrdinalIgnoreCase) == 0;

			var absentVisibility = makeVisible ? ViewStates.Visible : ViewStates.Gone;
			var presentVisibility = makeVisible ? ViewStates.Gone : ViewStates.Visible;

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
	}
}

#endif
