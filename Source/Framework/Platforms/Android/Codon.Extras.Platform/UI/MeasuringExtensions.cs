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
using Android.Widget;

namespace Codon.UI
{
	public static class MeasuringExtensions
	{
		static DipsPixelTranslator dipsPixelTranslator;

		public static int MeasureHeightInPixels(this ListView listView)
		{
			if (dipsPixelTranslator == null)
			{
				dipsPixelTranslator = new DipsPixelTranslator(listView.Resources.DisplayMetrics.Density);
			}

			var adapter = listView.Adapter;

			int totalHeight = 0;
			int itemCount = adapter.Count;

			for (int i = 0; i < itemCount; i++)
			{
				var view = adapter.GetView(i, null, listView);
				var measureSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);

				view.Measure(measureSpec, measureSpec);

				totalHeight += view.MeasuredHeight;
			}

			totalHeight += listView.PaddingTop + listView.PaddingBottom;

			var dividerHeightPixels = listView.DividerHeight;
			totalHeight += dividerHeightPixels * (adapter.Count - 1);

			return totalHeight;
		}
	}
}