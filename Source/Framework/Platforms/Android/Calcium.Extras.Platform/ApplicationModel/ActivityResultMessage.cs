#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-18 22:15:37Z</CreationDate>
</File>
*/
#endregion

using Android.App;
using Android.Content;

namespace Calcium.ApplicationModel
{
	public class ActivityResultMessage
	{
		/// <summary>
		/// The Android request code that was used when launching the intent.
		/// </summary>
		public int RequestCode { get; set; }

		/// <summary>
		/// The Android result code which indicates the result of the intent.
		/// </summary>
		public Result ResultCode { get; set; }

		/// <summary>
		/// The Android intent that was used when launching the intent.
		/// </summary>
		public Intent Intent { get; set; }
		
		/// <inheritdoc />
		public ActivityResultMessage(
			int requestCode, Result resultCode, Intent intent)
		{
			RequestCode = requestCode;
			ResultCode = resultCode;
			Intent = intent;
		}
	}
}
