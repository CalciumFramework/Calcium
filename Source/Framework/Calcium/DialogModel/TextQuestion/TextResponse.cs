#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-23 02:19:17Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;

namespace Calcium.DialogModel
{
	/// <summary>
	/// Represents a user response to a question. 
	/// <seealso cref="TextQuestion"/>
	/// </summary>
	//[Serializable]
	public class TextResponse
	{
		/// <summary>
		/// The user's text response.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Indicates whether the text was submitted,
		/// or the dialog cancelled.
		/// </summary>
		public OkCancelQuestionResult OkCancelQuestionResult { get; set; }

		public TextResponse(OkCancelQuestionResult result, string answer)
		{
			OkCancelQuestionResult = result;
			Text = answer;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public TextResponse()
		{
			/* Intentionally left blank. */
		}

		/// <summary>
		/// A utility property that checks if the user
		/// provided a text response and didn't dismiss 
		/// the dialog.
		/// </summary>
		public bool HasValidAnswer => 
			OkCancelQuestionResult == OkCancelQuestionResult.OK 
			&& Text != null;
	}
}
