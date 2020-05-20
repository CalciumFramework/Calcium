#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-05 20:32:00Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Globalization;

using Calcium.Reflection;

namespace Calcium.ComponentModel
{
	/// <summary>
	/// Default implementation of the <see cref="IImplicitTypeConverter"/>
	/// interface. This is a simplified version 
	/// of the WPF and Mono ImplicitTypeConverter. 
	/// The WPF and Mono version makes use 
	/// of the FCL's TypeConverter APIs, which are not present 
	/// in .NET Standard.
	/// </summary>
	public class DefaultImplicitTypeConverter : IImplicitTypeConverter
	{
		public object ConvertToType(object value, Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));
			AssertArg.IsNotNull(value, nameof(value));

			if (value == null || type.IsAssignableFromEx(value.GetType()))
			{
				return value;
			}

			Type valueType = value.GetType();

			if (valueType == typeof(string))
			{
				var fromStringConverter = new FromStringConverter(type);
				return fromStringConverter.ConvertFrom(CultureInfo.CurrentCulture, value);
			}

			return null;
		}
	}

	/// <summary>
	/// This class is able to convert a string
	/// to various built-in types.
	/// </summary>
	public class FromStringConverter
	{
		readonly Type type;

		public FromStringConverter(Type type)
		{
			this.type = type;
		}

		public bool CanConvertFrom(Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public object ConvertFrom(CultureInfo culture, object value)
		{
			var stringValue = value as string;
			if (stringValue != null)
			{
				if (type == typeof(bool))
				{
					return bool.Parse(stringValue);
				}

				if (type.IsEnum())
				{
					return Enum.Parse(type, stringValue, false);
				}

				if (type == typeof(Guid))
				{
					return new Guid(stringValue);
				}

				if (type == typeof(double))
				{
					return double.Parse(stringValue);
				}

				if (type == typeof(int))
				{
					return int.Parse(stringValue);
				}

				if (type == typeof(float))
				{
					return float.Parse(stringValue);
				}

				if (type == typeof(decimal))
				{
					return decimal.Parse(stringValue);
				}
			}

			return null;
		}
	}
}
