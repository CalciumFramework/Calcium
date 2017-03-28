#if __FORMS__ || __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 10:33:28Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Globalization;
using Codon.MissingTypes.System.Windows.Data;

namespace Codon.UI.Elements
{
	/// <summary>
	/// This class is used to convert a string value to an object 
	/// whose type is defined using the supplied object parameter.
	/// </summary>
	public class StringToTypedObjectConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}

			string valueString = value.ToString();

			string typeString = parameter?.ToString();

			if (typeString != null)
			{
				Type type = Type.GetType(typeString, false);

				if (type != null)
				{
					if (type == typeof(int))
					{
						return int.Parse(valueString);
					}

					if (type == typeof(double))
					{
						return double.Parse(valueString);
					}

					if (type == typeof(float))
					{
						return float.Parse(valueString);
					}

					if (type == typeof(Uri))
					{
						return new Uri(valueString);
					}

					var result = Activator.CreateInstance(type, new object[] {valueString});
					return result;
				}
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
#endif