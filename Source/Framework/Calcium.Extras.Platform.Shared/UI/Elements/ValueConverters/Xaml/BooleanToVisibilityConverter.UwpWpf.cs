#if WPF || WINDOWS_UWP
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-24 18:30:02Z</CreationDate>
</File>
*/
#endregion

using System;

#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Calcium.UI.Elements
{
    partial class BooleanToVisibilityConverter
    {
	    Visibility ConvertToPlatformValue(VisibilityValue visibilityValue)
	    {
		    switch (visibilityValue)
		    {
			    case VisibilityValue.Visible:
				    return Visibility.Visible;
			    case VisibilityValue.Collapsed:
				    return Visibility.Collapsed;
				case VisibilityValue.Hidden:
					return Visibility.Collapsed;
		    }

			throw new ArgumentOutOfRangeException();
	    }
    }
}
#endif
