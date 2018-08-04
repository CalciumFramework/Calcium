#if WINDOWS_UWP || NETFX_CORE
#region File and License Information

/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Codon.ResourcesModel.Extensions;
using Codon.Services;

namespace Codon.DialogModel
{
	/// <summary>
	/// UWP implementation of <see cref="Services.IDialogService"/>.
	/// </summary>
	public class DialogService : DialogServiceBase
	{
		public override async Task<DialogResult> ShowDialogAsync(
			object message, 
			string caption, 
			DialogButton dialogButton,
			DialogImage dialogImage = DialogImage.None,
			DialogController dialogController = null)
		{
			int? result;

			/* TODO: Make localizable resource. */
			switch (dialogButton)
			{
				case DialogButton.OK:

					result = await ShowDialogAsync(
						message, 
						new List<object> { "OK" }, 
						caption);

					if (result == 0)
					{
						return DialogResult.OK;
					}

					return DialogResult.None;

				case DialogButton.OKCancel:

					result = await ShowDialogAsync(
						message, 
						new List<object> { "OK", "Cancel" }, 
						caption);

					if (result == 0)
					{
						return DialogResult.OK;
					}
					else if (result == 1)
					{
						return DialogResult.Cancel;
					}

					return DialogResult.None;

				case DialogButton.YesNo:

					result = await ShowDialogAsync(
						message, 
						new List<object> { "Yes", "No" }, 
						caption);

					if (result == 0)
					{
						return DialogResult.Yes;
					}
					else if (result == 1)
					{
						return DialogResult.No;
					}

					return DialogResult.None;

				case DialogButton.YesNoCancel:
					result = await ShowDialogAsync(
								message, 
								new List<object> { "Yes", "No", "Cancel" },
								caption);

					if (result == 0)
					{
						return DialogResult.Yes;
					}
					else if (result == 1)
					{
						return DialogResult.No;
					}
					else if (result == 2)
					{
						return DialogResult.Cancel;
					}

					return DialogResult.None;

				default:
					throw new InvalidOperationException(
						"Unknown DialogButton: " + dialogButton);
			}
		}

		public override async Task<int?> ShowDialogAsync(
			object question, 
			IEnumerable<object> buttons, 
			string caption = null,
			int defaultAcceptButton = -1,
			DialogController dialogController = null)
		{
			string questionText = AssertArg.IsNotNull(question, nameof(question)).ToString().Parse();
			
			if (caption == null)
			{
				caption = (DefaultMessageCaptionFunc != null
					? DefaultMessageCaptionFunc()
					: string.Empty);
			}
			else
			{
				caption = caption.Parse();
			}

			var messageDialog = 
				new MessageDialog(
						questionText,
						caption);

			var buttonList = buttons.ToList();
			int selectedIndex = -1;

			if (buttonList.Any())
			{
				for (int i = 0; i < buttonList.Count; i++)
				{
					int index = i;
					messageDialog.Commands.Add(new UICommand(buttonList[i].ToString(),
						cmd =>
						{
							selectedIndex = index;
						}));
				}
			}
			else
			{
				messageDialog.Commands.Add(new UICommand("OK"));
			}

			await messageDialog.ShowAsync();

			if (selectedIndex > -1)
			{
				return selectedIndex;
			}

			return null;
		}

		public override Task<object> ShowToastAsync(
			ToastParameters toastParameters)
		{
			AssertArg.IsNotNull(toastParameters, nameof(toastParameters));

			/* TODO: Implement image display. */

			var template = @"
<toast>
<visual>
<binding template=""ToastGeneric"">
  <text>{0}</text>
  <text>{1}</text>
</binding>
</visual>
</toast>
";
			string body = toastParameters.Body?.ToString()?.Parse();
			string caption = toastParameters.Caption?.ToString()?.Parse();

			var xml = string.Format(template, caption, body);
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			var toast = new ToastNotification(xmlDocument);
			
			if (toastParameters.MillisecondsUntilHidden.HasValue)
			{
				toast.ExpirationTime = DateTimeOffset.Now.AddMilliseconds(
										toastParameters.MillisecondsUntilHidden.Value);
			}

			var source = new TaskCompletionSource<object>();
			TypedEventHandler<ToastNotification, object> activatedHandler = null;
			TypedEventHandler<ToastNotification, ToastDismissedEventArgs> dismissedHandler = null;
			TypedEventHandler<ToastNotification, ToastFailedEventArgs> failedHandler = null;

			activatedHandler = (o, e) =>
			{
				toast.Activated -= activatedHandler;
				toast.Dismissed -= dismissedHandler;
				toast.Failed -= failedHandler;

				source.SetResult(e);
			};

			dismissedHandler = (o, e) =>
			{
				toast.Activated -= activatedHandler;
				toast.Dismissed -= dismissedHandler;
				toast.Failed -= failedHandler;

				source.SetResult(e.Reason);
			};

			failedHandler = (o, e) =>
			{
				toast.Activated -= activatedHandler;
				toast.Dismissed -= dismissedHandler;
				toast.Failed -= failedHandler;

				source.SetResult(e.ErrorCode);
			};

			toast.Activated += activatedHandler;
			toast.Dismissed += dismissedHandler;
			toast.Failed += failedHandler;

			ToastNotificationManager.CreateToastNotifier().Show(toast);

			return source.Task;
		}

		public override async Task<QuestionResponse<TResponse>> AskQuestionAsync<TResponse>(
			IQuestion<TResponse> question)
		{
			AssertArg.IsNotNull(question, nameof(question));

			var textQuestion = question as TextQuestion;
			if (textQuestion != null)
			{
				var response = await ShowTextDialogAsync(textQuestion.Question);
				if (response.Item1)
				{
					var r = new TextResponse(OkCancelQuestionResult.OK, response.Item2);
					return new QuestionResponse<TResponse>((TResponse)(object)r, question);
				}
				var noResponse = new TextResponse(OkCancelQuestionResult.Cancel, null);
				return new QuestionResponse<TResponse>((TResponse)(object)noResponse, question);
			}
			
			throw new NotSupportedException();
		}

		async Task<Tuple<bool, string>> ShowTextDialogAsync(string title)
		{
			var inputTextBox = new TextBox();
			inputTextBox.AcceptsReturn = false;
			inputTextBox.Height = 32;
			ContentDialog dialog = new ContentDialog
			{
				Content = inputTextBox,
				Title = title,
				IsSecondaryButtonEnabled = true,
				PrimaryButtonText = "OK",
				SecondaryButtonText = "Cancel"
			};
			var dialogResult = await dialog.ShowAsync();
			if (dialogResult == ContentDialogResult.Primary)
			{
				return new Tuple<bool, string>(true, inputTextBox.Text);
			}

			return new Tuple<bool, string>(false, null);
		}
	}
}
#endif