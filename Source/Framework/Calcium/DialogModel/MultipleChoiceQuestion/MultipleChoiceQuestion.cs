#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-09-07 23:25:17Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Calcium.DialogModel
{
	/// <summary>
	/// Provides parameters for the <see cref="Services.IDialogService.AskMultipleChoiceQuestionAsync"/> method.
	/// </summary>
	/// <typeparam name="TSelectableItem">The type of objects in the items collection,
	/// which the user is able to select.</typeparam>
	public class MultipleChoiceQuestion<TSelectableItem> : IQuestion<MultipleChoiceResponse<TSelectableItem>>
	{
		/// <summary>
		/// This func is used to extract text from each item in the Items collection.
		/// If ItemTemplateName is provided, TextFunc is not used.
		/// </summary>
		public Func<TSelectableItem, string> TextFunc { get; }

		/// <summary>
		/// The name of a DataTemplate that is used to display each item in the collection.
		/// Providing a value for this property causes the TextFunc property to be disregarded.
		/// </summary>
		public string ItemTemplateName { get; set; }

		/// <summary>
		/// The text shown at the top of the dialog.
		/// </summary>
		public string Caption { get; }

		readonly List<TSelectableItem> items = new List<TSelectableItem>();

		/// <summary>
		/// A list of items that the user is to select from.
		/// </summary>
		public IEnumerable<TSelectableItem> Items => items;

		/// <summary>
		/// The initially selected item. Can be null.
		/// This property is applied if <see cref="MultiSelect"/> is <c>false</c>.
		/// </summary>
		public TSelectableItem SelectedItem { get; set; }

		/// <summary>
		/// The initially selected items. Can be null.
		/// This property is applied if <see cref="MultiSelect"/> is <c>true</c>.
		/// </summary>
		public IEnumerable<TSelectableItem> SelectedItems { get; set; }

		/// <summary>
		/// Allows the user to select multiple items.
		/// </summary>
		public bool MultiSelect { get; set; }

		public MultipleChoiceQuestion(IEnumerable<TSelectableItem> items, Func<TSelectableItem, string> textFunc, string caption)
		{
			Caption = caption;
			this.items.AddRange(AssertArg.IsNotNull(items, nameof(items)));
			TextFunc = AssertArg.IsNotNull(textFunc, nameof(textFunc));
		}

		public MultipleChoiceQuestion(IEnumerable<TSelectableItem> items, string itemTemplateName, string caption)
		{
			Caption = caption;
			this.items.AddRange(AssertArg.IsNotNull(items, nameof(items)));
			ItemTemplateName = AssertArg.IsNotNullOrWhiteSpace(itemTemplateName, nameof(itemTemplateName));
		}
	}
}
