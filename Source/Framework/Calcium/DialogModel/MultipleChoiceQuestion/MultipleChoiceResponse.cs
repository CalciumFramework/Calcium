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
	/// <summary>
	/// Provides the result of a call to the <see cref="Services.IDialogService.AskMultipleChoiceQuestionAsync"/> method.
	/// </summary>
	/// <typeparam name="T">The type of objects in the items collection,
	/// which the user is able to select.</typeparam>
	public class MultipleChoiceResponse<T>
	{
		/// <summary>
		/// The items that the user selected if
		/// <see cref="MultipleChoiceQuestion{TSelectableItem}.MultiSelect"/> is <c>true</c>.
		/// </summary>
		public IEnumerable<T> SelectedItems { get; }

		/// <summary>
		/// The item that the user selected if
		/// <see cref="MultipleChoiceQuestion{TSelectableItem}.MultiSelect"/> is <c>false</c>.
		/// May be null.
		/// </summary>
		public T SelectedItem => SelectedItems != null ? SelectedItems.FirstOrDefault() : default;


		/// <summary>
		/// Indicates that the user confirmed his or her choice
		/// and did not cancel out of the dialog.
		/// </summary>
		public bool UserPressedOK { get; set; }

		public MultipleChoiceResponse()
		{
			SelectedItems = new T[0];
		}

		public MultipleChoiceResponse(IEnumerable<T> selectedItems)
		{
			this.SelectedItems = selectedItems;
			UserPressedOK = true;
		}
	}
}