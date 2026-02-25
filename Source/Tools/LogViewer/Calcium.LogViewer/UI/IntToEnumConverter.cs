using System;
using System.Globalization;

using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Calcium.LogViewer.UI
{
	public sealed class IntToEnumConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			Type? enumType = GetEnumType(parameter, targetType);
			if (enumType is null)
			{
				return BindingOperations.DoNothing;
			}

			if (value is null)
			{
				return CreateDefaultEnumValue(enumType);
			}

			if (TryConvertToInt64(value, out long numericValue) is false)
			{
				return BindingOperations.DoNothing;
			}

			Type underlyingType = Enum.GetUnderlyingType(enumType);

			object boxedUnderlyingValue;
			try
			{
				boxedUnderlyingValue = System.Convert.ChangeType(numericValue, underlyingType, culture);
			}
			catch
			{
				return BindingOperations.DoNothing;
			}

			object enumValue = Enum.ToObject(enumType, boxedUnderlyingValue);

			if (IsFlagsEnum(enumType))
			{
				return enumValue;
			}

			if (Enum.IsDefined(enumType, enumValue))
			{
				return enumValue;
			}

			return BindingOperations.DoNothing;
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			Type? enumType = GetEnumType(parameter, value?.GetType());
			if (enumType is null)
			{
				return BindingOperations.DoNothing;
			}

			if (value is null)
			{
				return BindingOperations.DoNothing;
			}

			if (enumType.IsInstanceOfType(value) is false)
			{
				return BindingOperations.DoNothing;
			}

			try
			{
				object underlyingValue = System.Convert.ChangeType(value, Enum.GetUnderlyingType(enumType), culture);
				return System.Convert.ToInt32(underlyingValue, culture);
			}
			catch
			{
				return BindingOperations.DoNothing;
			}
		}

		static Type? GetEnumType(object? parameter, Type? fallbackType)
		{
			if (parameter is Type parameterType)
			{
				return parameterType.IsEnum ? parameterType : null;
			}

			if (parameter is string typeName && string.IsNullOrWhiteSpace(typeName) is false)
			{
				Type? resolved = Type.GetType(typeName, throwOnError: false);
				return resolved?.IsEnum == true ? resolved : null;
			}

			if (fallbackType?.IsEnum == true)
			{
				return fallbackType;
			}

			return null;
		}

		static bool TryConvertToInt64(object value, out long numericValue)
		{
			try
			{
				numericValue = System.Convert.ToInt64(value, CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
				numericValue = 0;
				return false;
			}
		}

		static bool IsFlagsEnum(Type enumType)
		{
			return enumType.IsDefined(typeof(FlagsAttribute), inherit: false);
		}

		static object CreateDefaultEnumValue(Type enumType)
		{
			Type underlyingType = Enum.GetUnderlyingType(enumType);
			object zero = System.Convert.ChangeType(0, underlyingType, CultureInfo.InvariantCulture);
			return Enum.ToObject(enumType, zero);
		}
	}
}