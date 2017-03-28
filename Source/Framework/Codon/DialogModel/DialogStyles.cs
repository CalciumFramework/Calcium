#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-09 22:49:38Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.DialogModel
{
	/// <summary>
	/// Indicates how a dialog should be displayed.
	/// </summary>
	[Flags]
	public enum DialogStyles
	{
		/// <summary>
		/// Make the dialog stretch to encompass the available
		/// screen width.
		/// </summary>
		StretchHorizontal,

		/// <summary>
		/// Make the dialog stretch to encompass the available
		/// screen height.
		/// </summary>
		StretchVertical
	}
}