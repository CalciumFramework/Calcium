#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-04-25 14:43:57Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codon.DialogModel
{
	/// <summary>
	/// Base implementation of <see cref="Services.IDialogService"/>.
	/// </summary>
	public abstract class DialogServiceBase : Services.IDialogService
	{
		public Func<string> DefaultMessageCaptionFunc { get; set; }
		public Func<string> DefaultQuestionCaptionFunc { get; set; }
		public Func<string> DefaultWarningCaptionFunc { get; set; }
		public Func<string> DefaultErrorCaptionFunc { get; set; }

		protected int openDialogCount;

		public bool DialogOpen => openDialogCount > 0;

		/// <summary>
		/// Shows a dialogue using a known set of buttons and images. 
		/// Override this message in test mocks.
		/// This is the only place where a MessageBox is shown.
		/// </summary>
		/// <param name="content">The message text to display.</param>
		/// <param name="caption">The caption to display at the top of the dialog.</param>
		/// <param name="dialogButton">The message box button enum value, 
		/// which determines what buttons are shown.</param>
		/// <param name="dialogImage">The message box image, 
		/// which determines what icon is used.</param>
		/// <param name="dialogController">An instance of the DialogController class 
		/// that can be used to dismiss the dialog from calling code.</param>
		/// <returns>The result of showing the message box.</returns>
		public abstract Task<DialogResult> ShowDialogAsync(
								object content, 
								string caption, 
								DialogButton dialogButton,
								DialogImage dialogImage = DialogImage.None,
								DialogController dialogController = null);
		#region Messages

		protected string GetDefaultMessageCaption()
		{
			/* TODO: Make localizable resource. */
			string defaultCaption = DefaultMessageCaptionFunc != null
										? DefaultMessageCaptionFunc() 
										: "Message";
			return defaultCaption;
		}

		public async Task ShowMessageAsync(
			string message, string caption = null)
		{
			await ShowDialogAsync(
					message, 
					caption,
					DialogButton.OK, 
					DialogImage.Information);
		}
		#endregion

		#region Warnings
		string GetDefaultWarningCaption()
		{
			/* TODO: Make localizable resource. */
			string defaultCaption = DefaultWarningCaptionFunc != null
										? DefaultWarningCaptionFunc() 
										: "Warning";
			return defaultCaption;
		}

		public async Task ShowWarningAsync(
			string message, string caption = null)
		{
			await ShowDialogAsync(
					message, 
					caption ?? GetDefaultWarningCaption(),
					DialogButton.OK, 
					DialogImage.Warning);
		}
		#endregion

		#region Errors
		string GetDefaultErrorCaption()
		{
			/* TODO: Make localizable resource. */
			string defaultCaption = DefaultErrorCaptionFunc != null
										? DefaultErrorCaptionFunc() 
										: "Error";
			return defaultCaption;
		}

		public async Task ShowErrorAsync(
			string message, string caption = null)
		{
			await ShowDialogAsync(
				message, 
				caption ?? GetDefaultErrorCaption(),
				DialogButton.OK, 
				DialogImage.Error);
		}
		#endregion

		#region Questions

		string GetDefaultQuestionCaption()
		{
			/* TODO: Make localizable resource. */
			string result = DefaultQuestionCaptionFunc != null
								? DefaultQuestionCaptionFunc() 
								: "Question";
			return result;
		}

		public async Task<bool> AskYesNoQuestionAsync(
			string question, string caption = null)
		{
			var result = await ShowDialogAsync(
							question, 
							caption ?? GetDefaultQuestionCaption(),
							DialogButton.YesNo, 
							DialogImage.Question, 
							null);

			return result == DialogResult.Yes 
				|| result == DialogResult.OK;
		}

		public async Task<bool> AskOkCancelQuestionAsync(
			string question, string caption = null)
		{
			var result = await ShowDialogAsync(
								question, 
								caption ?? GetDefaultQuestionCaption(),
								DialogButton.OKCancel, 
								DialogImage.Question, 
								null);

			return result == DialogResult.OK
				|| result == DialogResult.Yes;
		}
		
		public abstract Task<QuestionResponse<TResponse>> AskQuestionAsync<TResponse>(
			IQuestion<TResponse> question);

		public abstract Task<int?> ShowDialogAsync(
										object body, 
										IEnumerable<object> buttons, 
										string caption = null,
										int defaultAcceptButtonIndex = -1,
										DialogController dialogController = null);
		
		public async Task<YesNoCancelQuestionResult> AskYesNoCancelQuestionAsync(
			string question, string caption = null)
		{
			DialogResult dialogResult = 
				await ShowDialogAsync(
							question, 
							caption ?? GetDefaultQuestionCaption(),
							DialogButton.YesNoCancel, 
							DialogImage.Question);
			
			switch (dialogResult)
			{
				case DialogResult.Yes:
					return YesNoCancelQuestionResult.Yes;
				case DialogResult.No:
					return YesNoCancelQuestionResult.No;
				case DialogResult.Cancel:
					return YesNoCancelQuestionResult.Cancel;
				default:
					/* Should never get here. */
					throw new InvalidOperationException(
						"Invalid dialog MessageBoxResult.");
			}
		}

		#endregion

		public abstract Task<object> ShowToastAsync(ToastParameters toastParameters);

		public async Task<object> ShowToastAsync(
			string caption, string body = null)
		{
			return await ShowToastAsync(
								new ToastParameters
								{
									Caption = caption,
									Body = body
								});
		}

		public abstract Task<MultipleChoiceResponse<TSelectableItem>> AskMultipleChoiceQuestionAsync<TSelectableItem>(
			MultipleChoiceQuestion<TSelectableItem> question);

		public static class Strings
		{
			public static string OK = "OK";
			public static string Cancel = "Cancel";
			public static string Yes = "Yes";
			public static string No = "No";
		}
	}
}
