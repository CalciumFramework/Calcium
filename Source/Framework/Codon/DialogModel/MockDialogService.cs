#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-28 21:07:13Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codon.DialogModel
{
	/// <summary>
	/// A simple implementation of IDialogService
	/// that is used during unit testing.
	/// </summary>
	public class MockDialogService : DialogServiceBase
	{
		public object Content { get; set; }
		public string Caption { get; set; }
		public DialogButton DialogButton { get; set; }
		public DialogImage DialogImage { get; set; }
		
		DialogResult showCustomDialogResult = DialogResult.OK;

		public DialogResult ShowCustomDialogResult
		{
			get => showCustomDialogResult;
			set => showCustomDialogResult = value;
		}

		public override Task<DialogResult> ShowDialogAsync(
			object content, string caption, DialogButton dialogButton,
			DialogImage dialogImage = DialogImage.None,
			DialogController dialogController = null)
		{
			Content = content;
			Caption = caption;
			DialogButton = dialogButton;
			DialogImage = dialogImage;

			return Task.FromResult(showCustomDialogResult);
		}

		TextResponse askQuestionWithTextResponseResult 
			= new TextResponse(OkCancelQuestionResult.OK, "MockDialogService response.");

		public TextResponse AskQuestionWithTextResponseResult
		{
			get => askQuestionWithTextResponseResult;
			set => askQuestionWithTextResponseResult = value;
		}

		public object TextQuestion { get; set; }

		public object MockResponseResult { get; set; }

		public TextResponse TextResponse { get; set; }

		public int AskQuestionMockResult { get; set; }

		public override Task<int?> ShowDialogAsync(
			object body, 
			IEnumerable<object> buttons,
			string caption = null,
			int defaultAcceptButtonIndex = -1,
			DialogController dialogController = null)
		{
			int? result = AskQuestionMockResult;
			return Task.FromResult(result);
		}
	
		public object AskQuestionMockQuestionResult { get; set; }

		public override Task<QuestionResponse<TResponse>> AskQuestionAsync<TResponse>(
			IQuestion<TResponse> question)
		{
			return (Task<QuestionResponse<TResponse>>)AskQuestionMockQuestionResult;
		}

		public override Task<object> ShowToastAsync(
			ToastParameters toastParameters)
		{
			return Task.FromResult((object)null);
		}

		public object AskMultipleChoiceQuestion_Response { get; set; }

		public override Task<MultipleChoiceResponse<TSelectableItem>> AskMultipleChoiceQuestionAsync<TSelectableItem>(MultipleChoiceQuestion<TSelectableItem> question)
		{
			return Task.FromResult((MultipleChoiceResponse<TSelectableItem>)AskMultipleChoiceQuestion_Response);
		}
	}
}
