#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-04-11 19:51:16Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using Codon.DialogModel;
using Codon.InversionOfControl;

namespace Codon.Services
{
	/// <summary>
	/// This service is used to display messages to the user
	/// or to seek feedback from the user.
	/// In the implementation this is most often performed using 
	/// a modal dialog box or a toast.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(DialogModel)
		+ ".DialogService, " + AssemblyConstants.PlatformAssembly, Singleton = true)]
	public interface IDialogService
	{
		/// <summary>
		/// Shows a message to a user that must be dismissed 
		/// before continuing.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="caption">
		/// The caption to display on the dialog.</param>
		Task ShowMessageAsync(string message, string caption = null);

		/// <summary>
		/// Shows a warning message to a user 
		/// that must be dismissed before continuing.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="caption">
		/// The caption to display on the dialog.</param>
		Task ShowWarningAsync(string message, string caption = null);

		/// <summary>
		/// Shows an error message to a user 
		/// that must be dismissed before continuing.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="caption">
		/// The caption to display on the dialog.</param>
		Task ShowErrorAsync(string message, string caption = null);

		/// <summary>
		/// Presents a dialog to the user with the specified buttons.
		/// </summary>
		/// <param name="body">The body text or an object 
		/// to be displayed in the dialog, such as a custom view.</param>
		/// <param name="buttons">
		/// A list of objects that represent buttons.</param>
		/// <param name="caption">
		/// The caption to display on the dialog.</param>
		/// <param name="defaultAcceptButtonIndex">
		/// The index to return if the user 
		/// uses the platform specific acceptance gesture.</param>
		/// <param name="dialogController">
		/// An instance of the DialogController class that can 
		/// be used to dismiss the dialog from calling code.</param>
		/// <returns>The index of the selected button.</returns>
		Task<int?> ShowDialogAsync(
			object body, 
			IEnumerable<object> buttons,
			string caption = null,
			int defaultAcceptButtonIndex = -1,
			DialogController dialogController = null);

		/// <summary>
		/// Presents a dialog to the user with the specified buttons.
		/// </summary>
		/// <param name="content">
		/// The body text. Ordinarily this is text,
		/// but it may be a platform specific control.</param>
		/// <param name="caption">
		/// The caption to display on the dialog.</param>
		/// <param name="dialogButton">
		/// The dialog button to display.</param>
		/// <param name="dialogImage">
		/// The dialog image to display. Default is none.</param>
		/// <param name="dialogController">
		/// An instance of the DialogController class that can 
		/// be used to dismiss the dialog from calling code.</param>
		/// <returns>A <see cref="DialogResult"/> reflecting 
		/// the users response.</returns>
		Task<DialogResult> ShowDialogAsync(
			object content,
			string caption,
			DialogButton dialogButton,
			DialogImage dialogImage = DialogImage.None,
			DialogController dialogController = null);

		/// <summary>
		/// Asks a question to a user that must be dismissed 
		/// before continuing.
		/// </summary>
		/// <typeparam name="TResponse"></typeparam>
		/// <param name="question"></param>
		/// <returns>A question result.</returns>
		Task<QuestionResponse<TResponse>> AskQuestionAsync<TResponse>(
			IQuestion<TResponse> question);

		/// <summary>
		/// Asks a question to a user that 
		/// must be dismissed before continuing.
		/// </summary>
		/// <param name="question"></param>
		/// <param name="caption"></param>
		/// <returns><c>true</c> if the user selected the OK button;
		/// <c>false</c> otherwise.</returns>
		Task<bool> AskOkCancelQuestionAsync(
			string question, 
			string caption = null);

		/// <summary>
		/// Asks a question to a user that must be dismissed 
		/// before continuing.
		/// </summary>
		/// <param name="question"></param>
		/// <param name="caption"></param>
		/// <returns><c>true</c> if the user selected the Yes button;
		/// <c>false</c> otherwise.</returns>
		Task<bool> AskYesNoQuestionAsync(
			string question, 
			string caption = null);

		/// <summary>
		/// Asks a question to a user that must be dismissed 
		/// before continuing.
		/// </summary>
		/// <param name="question">
		/// The question text displayed to the user.</param>
		/// <param name="caption">
		/// The caption of the dialog displayed to the user.</param>
		/// <returns>A <see cref="YesNoCancelQuestionResult"/> value
		/// reflecting the user response.</returns>
		Task<YesNoCancelQuestionResult> AskYesNoCancelQuestionAsync(
			string question, 
			string caption = null);

		/// <summary>
		/// Shows a toast message to the user.
		/// </summary>
		/// <param name="toastParameters">
		/// Determines how the toast appears.</param>
		/// <returns></returns>
		Task<object> ShowToastAsync(ToastParameters toastParameters);

		/// <summary>
		/// Shows a toast message to the user.
		/// </summary>
		/// <param name="caption">The main heading of the toast.</param>
		/// <param name="body">The text content of the toast.</param>
		/// <returns></returns>
		Task<object> ShowToastAsync(string caption, string body = null);

		/// <summary>
		/// Indicates whether the dialog service currently 
		/// has an open dialog.
		/// </summary>
		bool DialogOpen { get; }
	}
}
