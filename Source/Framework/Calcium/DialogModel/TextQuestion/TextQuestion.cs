#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-10 13:05:43Z</CreationDate>
</File>
*/
#endregion

using Calcium.MissingTypes.System.Windows.Input;

namespace Calcium.DialogModel
{
	/// <summary>
	/// Represents a question that requires a text
	/// response from the user.
	/// </summary>
	public class TextQuestion : IQuestion<TextResponse>, ITextDialogParameters
	{
		/// <summary>
		/// The body text of the question.
		/// </summary>
		public string Question { get; set; }

		string ITextDialogParameters.Body => Question;

		/// <summary>
		/// A regular expression that restricts the text entered. 
		/// </summary>
		public string RestrictionExpression { get; }

		/// <summary>
		/// The type of software keyboard to display.
		/// </summary>
		public InputScopeNameValue InputScope { get; set; }

		/// <summary>
		/// The caption displayed in the dialog.
		/// </summary>
		public string Caption { get; set; }

		/// <summary>
		/// A regular expression that is used to validate 
		/// the user's response.
		/// </summary>
		public string ValidationExpression { get; set; }

		/// <summary>
		/// The message to display to the user if the value
		/// does not validate. <seealso cref="ValidationExpression"/>
		/// </summary>
		public string ValidationFailedMessage { get; set; }

		/// <summary>
		/// Text that is automatically populated in the text box.
		/// </summary>
		public string DefaultResponse { get; set; }

		/// <summary>
		/// Indicates whether the user should be able 
		/// to enter text on multiple lines.
		/// </summary>
		public bool MultiLine { get; set; }

		/// <summary>
		/// Indicates whether text in the edit box should be spell checked,
		/// which may result in red squiggles beneath unrecognized words.
		/// </summary>
		public bool SpellCheckEnabled { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="question">The text of the question.</param>
		/// <param name="inputScope">
		/// The type of software keyboard to display (if applicable).
		/// </param>
		public TextQuestion(
			string question,
			InputScopeNameValue inputScope = InputScopeNameValue.Default)
		{
			Question = question;
			InputScope = inputScope;
		}

		internal TextQuestion()
		{
			/* Intentionally left blank. */
		}
	}
}
