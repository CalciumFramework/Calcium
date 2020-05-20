#if WPF || WINDOWS_UWP || __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-15 13:23:30Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
#elif __ANDROID__
using System.Globalization;
using Calcium.MissingTypes.System.Windows.Data;
#else
using System.Globalization;
using System.Windows;
using System.Windows.Data;
#endif

namespace Calcium.UI.Elements
{
	/// <summary>
	/// This class allows the visibility of an element
	/// to be set according to a bound value.
	/// The visibility value is determined by the 
	/// parameter and the value. 
	/// The parameter is a combination of three values, 
	/// and appears like this:
	/// VisibilityWhenTrue, VisibilityWhenFalse, VisibilityWhenNull
	/// For example, if you wish to set the visibility of an element
	/// such that it is collapsed when the value is true, 
	/// visible when the value is false, 
	/// and hidden when the value is null;
	/// you would use the following parameter:
	/// "Collapsed, Visible, Collapsed"
	/// 
	/// For Android, use the Android <c>ViewState</c> enum values:
	/// "Gone, Visible, Invisible"
	/// </summary>
	public partial class BooleanToVisibilityConverter : IValueConverter
	{
		enum VisibilityValue
		{
			Visible,
			Hidden,
			Collapsed
		}

		class VisibilitySet
		{
			public VisibilityValue TrueVisibility { get; }
			public VisibilityValue FalseVisibility { get; }
			public VisibilityValue NullVisibility { get; }

			public VisibilitySet(
				VisibilityValue trueVisibility, VisibilityValue falseVisibility, VisibilityValue nullVisibility)
			{
				TrueVisibility = trueVisibility;
				FalseVisibility = falseVisibility;
				NullVisibility = nullVisibility;
			}
		}

		static Dictionary<string, VisibilitySet> GenerateVisibilityDictionary()
		{
			VisibilityValue[] values =
			{
				VisibilityValue.Visible,
				VisibilityValue.Collapsed,
				VisibilityValue.Hidden
			};

			var dictionary = new Dictionary<string, VisibilitySet>();

			foreach (var trueValue in values)
			{
				foreach (var falseValue in values)
				{
					foreach (var nullValue in values)
					{
						string key = $"{trueValue},{falseValue},{nullValue}";
						var set = new VisibilitySet(trueValue, falseValue, nullValue);
						dictionary.Add(key, set);
					}
				}
			}

			foreach (var trueValue in values)
			{
				foreach (var falseValue in values)
				{
					foreach (var nullValue in values)
					{
						string key = $"{GetAndroidAlias(trueValue)},{GetAndroidAlias(falseValue)},{GetAndroidAlias(nullValue)}";
						var set = new VisibilitySet(trueValue, falseValue, nullValue);
						dictionary[key] = set;
					}
				}
			}

			return dictionary;
		}

		static string GetAndroidAlias(VisibilityValue visibilityValue)
		{
			if (visibilityValue == VisibilityValue.Visible)
			{
				return visibilityValue.ToString();
			}
			else if (visibilityValue == VisibilityValue.Hidden)
			{
				return "Invisible";
			}
			else if (visibilityValue == VisibilityValue.Collapsed)
			{
				return "Gone";
			}

			throw new ArgumentException("Unknown value: " + visibilityValue);
		}

		static readonly Dictionary<string, VisibilitySet> visibilityDictionary
			= GenerateVisibilityDictionary();

		object Convert(object value, Type targetType, object parameter)
		{
			var stringParam = parameter as string;
			if (string.IsNullOrWhiteSpace(stringParam))
			{
				if (value == null || (bool)value)
				{
					return ConvertToPlatformValue(VisibilityValue.Visible);
				}

				return ConvertToPlatformValue(VisibilityValue.Hidden);
			}

			stringParam = stringParam.Replace(" ", "").Trim();

			VisibilitySet set;
			if (!visibilityDictionary.TryGetValue(stringParam, out set))
			{
				throw new ArgumentOutOfRangeException(nameof(parameter),
					$"Visibility set not found with value: '{stringParam}'.");
			}

			if (value == null)
			{
				return ConvertToPlatformValue(set.NullVisibility);
			}

			if ((bool)value)
			{
				return ConvertToPlatformValue(set.TrueVisibility);
			}

			return ConvertToPlatformValue(set.FalseVisibility);
		}

		/// <summary>
		/// This class allows the visibility of an element
		/// to be set according to a bound value.
		/// The visibility value is determined by the 
		/// parameter and the value. 
		/// The parameter is a combination of three values, 
		/// and appears like this:
		/// VisibilityWhenTrue, VisibilityWhenFalse, VisibilityWhenNull
		/// For example, if you wish to set the visibility of an element
		/// such that it is collapsed when the value is true, 
		/// visible when the value is false, 
		/// and hidden when the value is null;
		/// you would use the following parameter:
		/// "Collapsed, Visible, Collapsed"
		/// 
		/// For Android, use the Android <c>ViewState</c> enum values:
		/// "Gone, Visible, Invisible"
		/// </summary>
#if !NETFX_CORE

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
#else

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return Convert(value, targetType, parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
#endif

	}
}
#endif
