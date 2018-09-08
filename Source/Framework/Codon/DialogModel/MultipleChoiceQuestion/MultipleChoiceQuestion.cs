#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-09-07 23:25:17Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Codon.DialogModel
{
	public class MultipleChoiceQuestion : IQuestion<MultipleChoiceResponse>
	{
		public Func<object, string> TextFunc { get; }

		public string ItemTemplateName { get; set; }

		public string Caption { get; }

		readonly List<object> items = new List<object>();

		public IEnumerable<object> Items => items;

		public object SelectedItem { get; set; }

		public bool MultiSelect { get; set; }

		public MultipleChoiceQuestion(IEnumerable<object> items, Func<object, string> textFunc, string caption)
		{
			Caption = caption;
			this.items.AddRange(AssertArg.IsNotNull(items, nameof(items)));
			TextFunc = AssertArg.IsNotNull(textFunc, nameof(textFunc));
		}

		public MultipleChoiceQuestion(IEnumerable<object> items, string itemTemplateName, string caption)
		{
			Caption = caption;
			this.items.AddRange(AssertArg.IsNotNull(items, nameof(items)));
			ItemTemplateName = AssertArg.IsNotNullOrWhiteSpace(itemTemplateName, nameof(itemTemplateName));
		}
	}
}
