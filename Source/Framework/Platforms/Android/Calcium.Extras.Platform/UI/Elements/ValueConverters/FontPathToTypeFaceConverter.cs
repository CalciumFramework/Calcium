using System;
using System.Collections.Generic;
using System.Globalization;

using Android.Content;
using Android.Graphics;
using Calcium.MissingTypes.System.Windows.Data;

namespace Calcium.UI.Elements
{
	public class FontPathToTypefaceConverter : IValueConverter
	{
		public static Func<Context> ContextFunc { get; set; }

		static readonly Dictionary<string, Typeface> typeFaceDictionary 
			= new Dictionary<string, Typeface>();

		public static void Clear()
		{
			typeFaceDictionary.Clear();
		}

		/// <summary>
		/// Resolves a font using a path within the Assets folder. 
		/// For example "MaterialIcons-Regular.ttf"
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns>A TypeFace object or <c>null</c> if value is null or whitespace.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value);
		}

		public Typeface Convert(object value)
		{
			string fontName = value?.ToString();
			if (string.IsNullOrWhiteSpace(fontName))
			{
				return null;
			}

			Typeface result;

			if (typeFaceDictionary.TryGetValue(fontName, out result))
			{
				return result;
			}

			var contextFunc = ContextFunc;
			if (contextFunc == null)
			{
				throw new Exception("ContextFunc must be set prior to calling Convert.");
			}

			var context = ContextFunc();

			if (context == null)
			{
				throw new Exception("ContextFunc returned a null value.");
			}

			result = Typeface.CreateFromAsset(context.Assets, fontName);

			typeFaceDictionary[fontName] = result;

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
