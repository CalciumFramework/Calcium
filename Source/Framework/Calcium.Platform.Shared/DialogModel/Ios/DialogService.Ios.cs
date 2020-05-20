#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calcium.Services;
using UIKit;

namespace Calcium.DialogModel
{
	/// <summary>
	/// iOS implementation of <see cref="Services.IDialogService"/>.
	/// </summary>
	public partial class DialogService : DialogServiceBase
	{
		public override Task<int?> ShowDialogAsync(
			object body, 
			IEnumerable<object> buttons, 
			string caption = null, 
			int defaultAcceptButtonIndex = -1,
			DialogController dialogController = null)
		{
			int clickedButtonIndex = -1;

			string[] buttonLabelArray;
			string cancelButtonLabel = null;

			if (buttons != null)
			{
				object[] tempArray = buttons.ToArray();

				if (tempArray.Length > 0)
				{
					cancelButtonLabel = tempArray[0] + string.Empty;
					buttonLabelArray = new string[tempArray.Length - 1];
					Array.Copy(tempArray, 1, buttonLabelArray, 0, tempArray.Length - 1);
				}
				else
				{
					List<string> temp = new List<string>();
					foreach (var o in tempArray)
					{
						string arrayValue = o?.ToString() ?? string.Empty;
						temp.Add(arrayValue);
					}
					buttonLabelArray = temp.ToArray();
				}
			}
			else
			{
				buttonLabelArray = new string[0];
			}

			if (cancelButtonLabel == null)
			{
				cancelButtonLabel = DefaultOkaySymbol;
			}

			string bodyText = body?.ToString() ?? string.Empty;

			var stringParserService = Dependency.Resolve<IStringParserService>();

			if (!string.IsNullOrWhiteSpace(bodyText))
			{
				bodyText = stringParserService.Parse(bodyText);
			}
			
			var source = new TaskCompletionSource<int?>();

			UIAlertView alert;

			if (caption == null)
			{
				alert = new UIAlertView {Message = bodyText};
				int cancelButtonIndex = 0;

				foreach (var buttonLabel in buttonLabelArray)
				{
					alert.AddButton(buttonLabel);
					cancelButtonIndex++;
				}

				alert.AddButton(cancelButtonLabel);
				alert.CancelButtonIndex = cancelButtonIndex;
			}
			else
			{
				caption = stringParserService.Parse(caption);

				IUIAlertViewDelegate viewDelegate = new UIAlertViewDelegate();
				
				alert = new UIAlertView(
								caption, 
								bodyText,
								viewDelegate, 
								cancelButtonLabel, 
								buttonLabelArray);
			}

			alert.Clicked += (sender, buttonArgs) =>
				{
					clickedButtonIndex = (int)buttonArgs.ButtonIndex;
					source.SetResult(clickedButtonIndex);
				};

			alert.Canceled += delegate { source.SetResult(null); };

			try
			{
				alert.Show();
			}
			catch (Exception ex)
			{
				source.SetException(ex);
			}

			return source.Task;
		}

		public string DefaultOkaySymbol = '\u2714' + string.Empty;
		public string DefaultCancelSymbol = '\u2718' + string.Empty;

		public override async Task<DialogResult> ShowDialogAsync(
			object content, 
			string caption, 
			DialogButton dialogButton, 
			DialogImage dialogImage = DialogImage.None,
			DialogController dialogController = null)
		{
			List<string> buttons = new List<string>();

			if (dialogButton == DialogButton.OK)
			{
				buttons.Add(DefaultOkaySymbol);
			}
			else if (dialogButton == DialogButton.OKCancel 
				|| dialogButton == DialogButton.YesNo
				|| dialogButton == DialogButton.YesNoCancel)
			{
				buttons.Add(DefaultOkaySymbol);
				buttons.Add(DefaultCancelSymbol);
			}

			var selectedIndex = await ShowDialogAsync(content, buttons, caption, 0, dialogController: dialogController);

			if (selectedIndex == null || selectedIndex < 0)
			{
				return DialogResult.Cancel;
			}

			if (selectedIndex == 0)
			{
				return dialogButton == DialogButton.OKCancel 
							? DialogResult.OK : DialogResult.Yes;
			}

			if (selectedIndex == 1)
			{
				return DialogResult.Cancel;
			}

			throw new IndexOutOfRangeException(
				"Index returned from dialog is out of range. " 
				+ selectedIndex);
		}

		public override Task<QuestionResponse<TResponse>> AskQuestionAsync<TResponse>(
			IQuestion<TResponse> question)
		{
			throw new NotImplementedException();
		}

		public override Task<object> ShowToastAsync(ToastParameters toastParameters)
		{
			UIAlertController alert = UIAlertController.Create(
				toastParameters.Caption?.ToString(), 
				toastParameters.Body?.ToString(), UIAlertControllerStyle.Alert);

			var source = new TaskCompletionSource<object>();

			// Configure the alert
			alert.AddAction(UIAlertAction.Create(DefaultOkaySymbol, UIAlertActionStyle.Default, action =>
			{
				source.SetResult(null);
			}));

			var controller = UIApplication.SharedApplication.KeyWindow.RootViewController;

			// Display the alert
			controller.PresentViewController(alert, true, null);

			return source.Task;
		}

		public override Task<MultipleChoiceResponse<TSelectableItem>> AskMultipleChoiceQuestionAsync<TSelectableItem>(
			MultipleChoiceQuestion<TSelectableItem> question)
		{
			throw new NotImplementedException();
		}
	}
}
#endif
