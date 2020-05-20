#if WPF || WPF_CORE
#region File and License Information
/*
<File>
	<License>
		Copyright � 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-04-11 20:01:19Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Calcium.ResourcesModel.Extensions;

namespace Calcium.DialogModel
{
	/// <summary>
	/// WPF implementation of <see cref="Calcium.Services.IDialogService"/>.
	/// </summary>
	public class DialogService : DialogServiceBase
	{
		/// <inheritdoc />
		public override Task<DialogResult> ShowDialogAsync(
			object content, string caption,
			DialogButton dialogButton, 
			DialogImage dialogImage = DialogImage.None,
			DialogController dialogController = null)
		{
			Window mainWindow = GetActiveWindow();
			Dispatcher dispatcher = mainWindow != null 
										? mainWindow.Dispatcher 
										: Dispatcher.CurrentDispatcher;

			string bodyString = content?.ToString().Parse() ?? string.Empty;
			string captionString = caption?.Parse() ?? string.Empty;

			if (dispatcher == null || dispatcher.CheckAccess())
			{
				MessageBoxResult messageBoxResult;

				try
				{
					Interlocked.Increment(ref openDialogCount);

					if (mainWindow != null)
					{
						/* We are on the UI thread, and hence no need to invoke the call.*/
						messageBoxResult = MessageBox.Show(
							mainWindow, 
							bodyString,
							captionString, 
							dialogButton.TranslateToMessageBoxButton(), 
							dialogImage.TranslateToMessageBoxButton());
					}
					else
					{
						messageBoxResult = MessageBox.Show(
							bodyString,
							captionString,
							dialogButton.TranslateToMessageBoxButton(),
							dialogImage.TranslateToMessageBoxButton());
					}
				}
				finally
				{
					Interlocked.Decrement(ref openDialogCount);
				}
				
				return Task.FromResult(
					messageBoxResult.TranslateToMessageBoxResult());
			}

			DialogResult result = DialogResult.OK; /* Satisfy compiler with default value. */
			dispatcher.Invoke((ThreadStart)delegate
			{
				MessageBoxResult messageBoxResult;

				try
				{
					Interlocked.Increment(ref openDialogCount);

					if (mainWindow != null)
					{
						/* We are on the UI thread, 
						 * and hence no need to invoke the call.*/
						messageBoxResult = MessageBox.Show(mainWindow, bodyString, captionString,
							dialogButton.TranslateToMessageBoxButton(),
							dialogImage.TranslateToMessageBoxButton());
					}
					else
					{
						messageBoxResult = MessageBox.Show(bodyString, captionString,
							dialogButton.TranslateToMessageBoxButton(),
							dialogImage.TranslateToMessageBoxButton());
					}
				}
				finally
				{
					Interlocked.Decrement(ref openDialogCount);
				}

				result = messageBoxResult.TranslateToMessageBoxResult();
			});

			return Task.FromResult(result);
		}

		/// <inheritdoc />
		public override Task<int?> ShowDialogAsync(
			object question, 
			IEnumerable<object> buttons,
			string caption = null,
			int defaultAcceptButtonIndex = -1,
			DialogController dialogController = null)
		{
			throw new NotImplementedException();
		}

		public override Task<QuestionResponse<TResponse>> AskQuestionAsync<TResponse>(
			IQuestion<TResponse> question)
		{
			throw new NotImplementedException();
		}

		static Window GetActiveWindow()
		{
			var result = Application.Current.Windows.OfType<Window>()
							.SingleOrDefault(x => x.IsActive);
			return result;
		}

		/// <inheritdoc />
		public override Task<object> ShowToastAsync(ToastParameters toastParameters)
		{
			throw new NotImplementedException();
		}

		public override Task<MultipleChoiceResponse<TSelectableItem>> AskMultipleChoiceQuestionAsync<TSelectableItem>(
			MultipleChoiceQuestion<TSelectableItem> question)
		{
			throw new NotImplementedException();
		}
	}
}
#endif
