#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-18 22:15:37Z</CreationDate>
</File>
*/
#endregion

using Android.App;
using Android.Content;

namespace Codon.ApplicationModel
{
	public class ActivityResultMessage
	{
		public int RequestCode { get; set; }
		public Result ResultCode { get; set; }
		public Intent Intent { get; set; }

		public ActivityResultMessage(
			int requestCode, Result resultCode, Intent intent)
		{
			RequestCode = requestCode;
			ResultCode = resultCode;
			Intent = intent;
		}
	}
}