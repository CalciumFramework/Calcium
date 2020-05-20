#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using Android.App;

namespace Calcium.UI
{
	public class DipsPixelTranslator
	{
		readonly float density;
		static readonly float staticDensity;

		static DipsPixelTranslator()
		{
			staticDensity = Application.Context.Resources.DisplayMetrics.Density;
		}

		public DipsPixelTranslator(float density)
		{
			this.density = density;
		}

		public DipsPixelTranslator() : this(Application.Context.Resources.DisplayMetrics.Density)
		{
		}

		public int ConvertPixelsToDips(float pixelValue)
		{
			var dp = (int)(pixelValue / density);
			return dp;
		}

		public int ConvertDipsToPixels(float dips)
		{
			var dp = (int)(dips * density);
			return dp;
		}

		public static int TranslatePixelsToDips(float pixelValue)
		{
			var dp = (int)(pixelValue / staticDensity);
			return dp;
		}

		public static int TranslateDipsToPixels(float dips)
		{
			var dp = (int)(dips * staticDensity);
			return dp;
		}
	}
}
