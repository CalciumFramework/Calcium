#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-08-21 16:16:19Z</CreationDate>
</File>
*/
#endregion

using Android.Text;

namespace Calcium.Text
{
	/// <summary>
	/// Extension methods for the <c>string</c> class.
	/// </summary>
	public static class AndroidStringExtensions
	{
		public static ISpanned ToHtml(this string text)
		{
			ISpanned result;
			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
			{
				result = Html.FromHtml(text, FromHtmlOptions.ModeLegacy);
			}
			else
			{
#pragma warning disable 618
				result = Html.FromHtml(text);
#pragma warning restore 618
			}
			return result;
		}

	}
}
#endif
