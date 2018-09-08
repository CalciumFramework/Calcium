#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-09-07 23:25:02Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Codon.DialogModel
{
	public class MultipleChoiceResponse
	{
		public IEnumerable<object> SelectedItems { get; }

		public object SelectedItem => SelectedItems?.FirstOrDefault();

		//public DialogResult DialogResult { get; internal set; }
		public bool UserPressedOK { get; set; }

		public MultipleChoiceResponse()
		{
			SelectedItems = new object[0];
		}

		public MultipleChoiceResponse(IEnumerable<object> selectedItems)
		{
			this.SelectedItems = selectedItems;
			UserPressedOK = true;
		}
	}
}