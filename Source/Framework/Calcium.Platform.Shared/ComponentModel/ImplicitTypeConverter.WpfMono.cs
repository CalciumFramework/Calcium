#if WPF || __ANDROID__ || __IOS__
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

using Calcium.Reflection;

namespace Calcium.ComponentModel
{
	/// <summary>
	/// Advanced implementation of the <see cref="IImplicitTypeConverter"/>
	/// interface. See the interface for API documentation.
	/// </summary>
    public class ImplicitTypeConverter : IImplicitTypeConverter
    {
		public object ConvertToType(object value, Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));
			AssertArg.IsNotNull(value, nameof(value));

			if (value == null || type.IsAssignableFrom(value.GetType()))

			{
				return value;
			}

			Type valueType = value.GetType();

			TypeConverter converter = GetTypeConverter(valueType);
			if (converter != null && converter.CanConvertTo(type))
			{
				value = converter.ConvertTo(value, type);
				return value;
			}

			converter = GetTypeConverter(type);
			if (converter != null && converter.CanConvertFrom(valueType))
			{
				value = converter.ConvertFrom(value);
				return value;
			}

			return null;
		}

		public TypeConverter GetTypeConverter(Type type)
		{
			var attribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(
								type, typeof(TypeConverterAttribute), false);

			if (attribute != null)
			{
				try
				{
					Type converterType = Type.GetType(attribute.ConverterTypeName, false);
					if (converterType != null)
					{
						return Activator.CreateInstance(converterType) as TypeConverter;
					}
				}
				catch (Exception)
				{
					/* Suppress. */
				}
			}

			var converter = TypeDescriptor.GetConverter(type);
			if (converter != null)
			{
				return converter;
			}

			return new FromStringTypeConverter(type);
		}
	}

	public class FromStringTypeConverter : TypeConverter
	{
		readonly Type type;

		public FromStringTypeConverter(Type type)
		{
			this.type = type;
		}

		public override bool CanConvertFrom(
			ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string)
					|| base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
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

#if WPF
				var stringBuilder = new StringBuilder();

				Assembly assembly = type.Assembly;
				stringBuilder.Append("<ContentControl xmlns='http://schemas.microsoft.com/client/2007' xmlns:c='"
									 + ("clr-namespace:" + type.Namespace + ";assembly=" + assembly.FullName.Split(new[] { ',' })[0]) + "'>\n");
				stringBuilder.Append("<c:" + type.Name + ">\n");
				stringBuilder.Append(stringValue);
				stringBuilder.Append("</c:" + type.Name + ">\n");
				stringBuilder.Append("</ContentControl>");

				var stringReader = new System.IO.StringReader(stringBuilder.ToString());
				System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
				var instance = System.Windows.Markup.XamlReader.Load(xmlReader) as System.Windows.Controls.ContentControl;

				if (instance != null)
				{
					return instance.Content;
				}
#endif
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}
#endif
