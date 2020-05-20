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
	<CreationDate>2017-03-24 18:30:02Z</CreationDate>
</File>
*/
#endregion

using System;
using Android.Views;

namespace Calcium.UI.Elements
{
    partial class BooleanToVisibilityConverter
    {
	    ViewStates ConvertToPlatformValue(VisibilityValue visibilityValue)
	    {
		    switch (visibilityValue)
		    {
			    case VisibilityValue.Visible:
				    return ViewStates.Visible;
			    case VisibilityValue.Collapsed:
				    return ViewStates.Gone;
				case VisibilityValue.Hidden:
					return ViewStates.Invisible;
		    }

			throw new ArgumentOutOfRangeException();
	    }
    }
}
#endif
