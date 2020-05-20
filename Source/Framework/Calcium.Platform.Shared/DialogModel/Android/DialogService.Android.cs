#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Calcium.InversionOfControl;
using Java.Lang;
using Calcium.Logging;
using Calcium.MissingTypes.System.Windows.Input;
using Calcium.ResourcesModel.Extensions;
using Calcium.Services;
using Exception = System.Exception;

namespace Calcium.DialogModel
{
	/// <summary>
	/// Android implementation of <see cref="Services.IDialogService"/>.
	/// </summary>
	public class DialogService : DialogServiceBase
	{
		public override Task<int?> ShowDialogAsync(
			object body, 
			IEnumerable<object> buttons, 
			string caption = null, 
			int defaultAcceptButtonIndex = -1,
			DialogController dialogController = null)
		{
			Context context = ResolveContext();

			AlertDialog.Builder builder = CreateAlertDialogBuilder(context, DialogStyle);

			if (dialogController != null && !dialogController.Cancellable)
			{
				builder.SetCancelable(false);
			}

			if (!string.IsNullOrWhiteSpace(caption))
			{
				var stringParserService = Dependency.Resolve<IStringParserService>();
				var parsedText = stringParserService.Parse(caption);
				builder.SetTitle(parsedText);
			}

			var bodyView = body as View;
			if (bodyView != null)
			{
				builder.SetView(bodyView);
			}
			else
			{
				var sequence = body as ICharSequence;
				if (sequence != null)
				{
					builder.SetMessage(sequence);
				}
				else
				{
					string bodyText = body?.ToString();

					if (!string.IsNullOrWhiteSpace(bodyText))
					{
						var stringParserService = Dependency.Resolve<IStringParserService>();
						var parsedText = stringParserService.Parse(bodyText);
;						builder.SetMessage(parsedText);
					}
				}
			}

			List<string> labels = null;
			int labelCount = 0;

			if (buttons != null)
			{
				labels = new List<string>();
				foreach (var button in buttons)
				{
					string buttonText = button.ToString();
					labels.Add(buttonText);
				}

				labelCount = labels.Count;
			}

			var resultSource = new TaskCompletionSource<int?>();

			if (labelCount >= 2)
			{
				builder.SetNegativeButton(labels[0],
					(dialog, whichButton) =>
					{
						resultSource.TrySetResult(0);
					});

				for (int i = 1; i < labelCount - 1; i++)
				{
					int iClosureCopy = i;
					builder.SetNeutralButton(labels[i],
					(dialog, whichButton) =>
					{
						resultSource.TrySetResult(iClosureCopy);
					});
				}
				
				builder.SetPositiveButton(labels[labelCount - 1],
					(dialog, whichButton) =>
					{
						int selectedIndex = labelCount - 1;
						resultSource.TrySetResult(selectedIndex);
					});
			}
			else
			{
				if (labelCount == 1)
				{
					string buttonLabel = labels[0];

					builder.SetPositiveButton(buttonLabel,
					(dialog, whichButton) =>
					{
						resultSource.TrySetResult(0);
					});
				}
			}

			builder.NothingSelected += (sender, e) => resultSource.TrySetResult(-1);

			Android.App.Application.SynchronizationContext.Post((object state) =>
			{
				try
				{
					/* If the dialog controllers Close method has been called
					 * before having a chance to show the dialog, do nothing. */
					if (dialogController != null && dialogController.CloseCalled)
					{
						resultSource.TrySetResult(-1);
						return;
					}

					Interlocked.Increment(ref openDialogCount);

					AlertDialog alertDialog = builder.Show();

					alertDialog.SetCanceledOnTouchOutside(false);

					var dialogStyles = dialogController?.DialogStyles;

					if (dialogStyles.HasValue)
					{
						var styles = dialogStyles.Value;
						var lp = new WindowManagerLayoutParams();
						Window window = alertDialog.Window;
						lp.CopyFrom(window.Attributes);

						var stretchHorizontal = (styles & DialogStyles.StretchHorizontal) == DialogStyles.StretchHorizontal;
						var stretchVertical = (styles & DialogStyles.StretchVertical) == DialogStyles.StretchVertical;
						lp.Width = stretchHorizontal ? ViewGroup.LayoutParams.MatchParent : lp.Width;
						lp.Height = stretchVertical ? ViewGroup.LayoutParams.MatchParent : lp.Height;//ViewGroup.LayoutParams.WrapContent;
						window.Attributes = lp;
					}

					//var backgroundImage = dialogController?.BackgroundImage;
					//					
					//if (backgroundImage != null)
					//{
					//	//Window window = alertDialog.Window;
					//	//window.SetBackgroundDrawable(backgroundImage);;
					//}

					alertDialog.CancelEvent += delegate
					{
						resultSource.TrySetResult(-1);
					};

					if (dialogController != null)
					{
						dialogController.CloseRequested += delegate
						{
							/* DialogController uses UI thread. */
							if (alertDialog.IsShowing)
							{
								alertDialog.Cancel();
							}
						};
					}

					/* Subscribing to the DismissEvent to set the result source 
					 * is unnecessary as other events are always raised.
					 * The DismissEvent is, however, always raised and thus
					 * we place the bodyView removal code here. */
					alertDialog.DismissEvent += (sender, args) =>
					{
						Interlocked.Decrement(ref openDialogCount);
						builder.SetView(null);

						try
						{
							(bodyView?.Parent as ViewGroup)?.RemoveView(bodyView);
						}
						catch (ObjectDisposedException)
						{
							/* View was already disposed in user code. */
						}
						catch (Exception ex)
						{
							var log = Dependency.Resolve<ILog>();
							log.Debug("Exception raised when removing view from alert.", ex);
						}
					};

					if (AlertDialogDividerColor.HasValue)
					{
						var resources = context.Resources;
						int id = resources.GetIdentifier("titleDivider", "id", "android");
						View titleDivider = alertDialog.FindViewById(id);
						if (titleDivider != null)
						{
							var color = AlertDialogDividerColor.Value;
							if (color == Color.Transparent)
							{
								titleDivider.Visibility = ViewStates.Gone;
							}

							titleDivider.SetBackgroundColor(color);
						}
					}

					if (AlertDialogTitleColor.HasValue)
					{
						var resources = context.Resources;
						int id = resources.GetIdentifier("alertTitle", "id", "android");
						var textView = alertDialog.FindViewById<TextView>(id);
						if (textView != null)
						{
							var color = AlertDialogTitleColor.Value;
							textView.SetTextColor(color);
						}
					}

					if (AlertDialogBackgroundColor.HasValue)
					{
						var v = bodyView ?? alertDialog.ListView;
						v.SetBackgroundColor(AlertDialogBackgroundColor.Value);
					}
				}
				catch (WindowManagerBadTokenException ex)
				{
					/* See http://stackoverflow.com/questions/2634991/android-1-6-android-view-windowmanagerbadtokenexception-unable-to-add-window*/
					resultSource.SetException(new Exception(
						"Unable to use the Application.Context object to create a dialog. Please either set the Context property of this DialogService or register the current activity using Dependency.Register<Activity>(myActivity)", ex));
				}
			}, null);

			return resultSource.Task;
		}

		Context ResolveContext()
		{
			Context context = Context;

			if (context == null)
			{
				if (Dependency.TryResolve(out Activity activity))
				{
					context = activity;
				}
				else if (Dependency.TryResolve<Context>(out context))
				{
					/* Nothing to do. */
				}
				else
				{
					context = Application.Context;
				}
			}

			return context;
		}

//		public string DefaultOkaySymbol = "Ok";// '\u2714' + "";
//		public string DefaultCancelSymbol = "Cancel";// '\u2718' + "";

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
				buttons.Add(Strings.Okay);
			}
			else if (dialogButton == DialogButton.OKCancel)
			{
				buttons.Add(Strings.Cancel);
				buttons.Add(Strings.Okay);
			}
			else if (dialogButton == DialogButton.YesNo)
			{
				buttons.Add(Strings.No);
				buttons.Add(Strings.Yes);
			}
			else if (dialogButton == DialogButton.YesNoCancel)
			{
				buttons.Add(Strings.No);
				buttons.Add(Strings.Cancel);
				buttons.Add(Strings.Yes);
			}

			var selectedIndex = await ShowDialogAsync(
											content, 
											buttons, 
											caption, 
											0, 
											dialogController);
			
			if (selectedIndex < 0)
			{
				return DialogResult.Cancel;
			}

			switch (selectedIndex)
			{
				case 0:
					switch (dialogButton)
					{
						case DialogButton.OK:
							return DialogResult.OK;
						case DialogButton.OKCancel:
							return DialogResult.Cancel;
						case DialogButton.YesNo:
							return DialogResult.No;
						case DialogButton.YesNoCancel:
							return DialogResult.No;
					}
					break;
				case 1:
					switch (dialogButton)
					{
						case DialogButton.OKCancel:
							return DialogResult.OK;
						case DialogButton.YesNo:
							return DialogResult.Yes;
						case DialogButton.YesNoCancel:
							return DialogResult.Cancel;
					}
					break;
				case 2:
					switch (dialogButton)
					{
						case DialogButton.YesNoCancel:
							return DialogResult.Yes;
					}
					break;
			}

			throw new IndexOutOfRangeException(
				"Index returned from dialog is out of range. " 
				+ selectedIndex);
		}

		public override Task<QuestionResponse<TResponse>> AskQuestionAsync<TResponse>(
			IQuestion<TResponse> question)
		{
			Context context = ResolveContext();
			AlertDialog.Builder builder = new AlertDialog.Builder(context);

			var trq = question as TextQuestion;
			if (trq == null)
			{
				throw new NotSupportedException(
					"Only TextQuestion is supported at this time.");
			}

			var caption = trq.Caption;
			if (!string.IsNullOrWhiteSpace(caption))
			{
				builder.SetTitle(caption.Parse());
			}

			var message = trq.Question;
			if (!string.IsNullOrWhiteSpace(message))
			{
				builder.SetMessage(message.Parse());
			}

			EditText editText = new EditText(context);

			if (trq.InputScope != InputScopeNameValue.Default)
			{
				var converter = Dependency.Resolve<IAndroidInputScopeConverter>();
				var platformValue = converter.ToNativeType(trq.InputScope);
				editText.InputType = platformValue;
			}

			if (!trq.MultiLine)
			{
				editText.SetSingleLine(true);
				editText.SetMaxLines(1);
			}

			if (!trq.SpellCheckEnabled)
			{
				editText.InputType = editText.InputType | InputTypes.TextFlagNoSuggestions;
			}

			if (trq.InputScope == InputScopeNameValue.Password
				|| trq.InputScope == InputScopeNameValue.NumericPassword)
			{
				editText.TransformationMethod = new PasswordTransformationMethod();
			}

			//var color = context.Resources.GetColor(Resources.Color.dialog_textcolor);
			//textBox.SetTextColor(Color.Black);
			editText.Text = trq.DefaultResponse;
			builder.SetView(editText);

			var manager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);

			var source = new TaskCompletionSource<QuestionResponse<TResponse>>();

			builder.SetPositiveButton(Strings.Okay,
				(s, e) =>
				{
					Interlocked.Decrement(ref openDialogCount);

					var textReponse = new TextResponse(OkCancelQuestionResult.OK, editText.Text);
					var result = new QuestionResponse<TResponse>((TResponse)(object)textReponse, question);

					manager.HideSoftInputFromWindow(editText.WindowToken, HideSoftInputFlags.None);

					source.TrySetResult(result);
				});

			builder.SetNegativeButton(Strings.Cancel, (s, e) =>
			{
				Interlocked.Decrement(ref openDialogCount);

				var textReponse = new TextResponse {OkCancelQuestionResult = OkCancelQuestionResult.Cancel};
				var result = new QuestionResponse<TResponse>((TResponse)(object)textReponse, question);

				manager.HideSoftInputFromWindow(editText.WindowToken, HideSoftInputFlags.None);

				source.TrySetResult(result);
			});

			Interlocked.Increment(ref openDialogCount);

			var dialog = builder.Show();
			dialog.SetCanceledOnTouchOutside(false);

			/* Focussing the EditText and showing the keyboard, 
			 * must be done after the alert is show, else it has no effect. */
			var looper = context.MainLooper;
			var handler = new Handler(looper);
			handler.Post(() => 
			{ 
				editText.RequestFocus();
			
				manager.ShowSoftInput(editText, ShowFlags.Forced);
			});

			return source.Task;
		}

		public override Task<object> ShowToastAsync(ToastParameters toastParameters)
		{
			Context context = ResolveContext();
			var toastLength = toastParameters.MillisecondsUntilHidden > 3000 ? ToastLength.Long : ToastLength.Short;
			string newLineCharacter = Java.Lang.JavaSystem.LineSeparator();
			
			var content = toastParameters.Caption?.ToString().Parse();

			View toastView = toastParameters.Body as View;

			if (toastView == null)
			{
				if (string.IsNullOrWhiteSpace(content))
				{
					content = toastParameters.Body?.ToString().Parse();
				}
				else
				{
					string bodyString = toastParameters.Body?.ToString();
					if (!string.IsNullOrWhiteSpace(bodyString))
					{
						content = content + newLineCharacter + bodyString.Parse();
					}
				}
			}

			Toast toast;

			if (toastView == null)
			{
				var body = new Java.Lang.String(content);
				toast = Toast.MakeText(context, body, toastLength);
			}
			else
			{
				toast = new Toast(context) {View = toastView};
			}

			if (toastParameters.VerticalOrientation.HasValue)
			{
				var verticalOrientation = toastParameters.VerticalOrientation.Value;
				if (verticalOrientation == ToastVerticalOrientation.Top)
				{
					toast.SetGravity(GravityFlags.Top, 0, 0);
				}
				else if (verticalOrientation == ToastVerticalOrientation.Center)
				{
					toast.SetGravity(GravityFlags.CenterVertical, 0, 0);
				}
				else if (verticalOrientation == ToastVerticalOrientation.Bottom)
				{
					toast.SetGravity(GravityFlags.Bottom, 0, 0);
				}
			}
			
			var source = new TaskCompletionSource<object>();

			UIContext.Instance.Send(() =>
			{
				try
				{
					toast.Show();
				}
				catch (Exception ex)
				{
					source.SetException(ex);
					return;
				}

				source.SetResult(null);
			});
			
			return source.Task;
		}

		public override Task<MultipleChoiceResponse<TSelectableItem>> AskMultipleChoiceQuestionAsync<TSelectableItem>(
			MultipleChoiceQuestion<TSelectableItem> question)
		{
			throw new NotImplementedException();
		}

		protected virtual AlertDialog.Builder CreateAlertDialogBuilder(
			Context context, int? themeResourceId)
		{
			if (themeResourceId.HasValue && Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
			{
				ContextThemeWrapper themedContext = new ContextThemeWrapper(context, themeResourceId.Value);
				return new AlertDialog.Builder(themedContext);
			}
			else
			{
				return new AlertDialog.Builder(context);
			}
		}

		public Color? AlertDialogDividerColor { get; set; }
		public Color? AlertDialogTitleColor { get; set; }

		public Color? AlertDialogBackgroundColor { get; set; }

		public Android.Content.Context Context { protected get; set; }

		public int DialogStyle { get; set; } = 
			Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop
					? Android.Resource.Style.ThemeMaterialDialogAlert 
					: -1;
		//Theme_Material_Light_Dialog_Alert
	}

	/// <summary>
	/// Converts an <see cref="InputScopeNameValue"/> 
	/// to a corresponding Android specific
	/// <see cref="InputTypes"/> value.
	/// <seealso cref="TextQuestion.InputScope"/>
	/// </summary>
	[DefaultType(typeof(AndroidInputScopeConverter), Singleton = true)]
	public interface IAndroidInputScopeConverter
	{
		/// <summary>
		/// Converts an <see cref="InputScopeNameValue"/> 
		/// to a corresponding Android specific
		/// <see cref="InputTypes"/> value.
		/// </summary>
		/// <param name="value">The framework value, 
		/// which is ordinarily supplied 
		/// with a <see cref="TextQuestion"/>.</param>
		/// <returns>The Android specific value.</returns>
		InputTypes ToNativeType(InputScopeNameValue value);
	}

	class AndroidInputScopeConverter : IAndroidInputScopeConverter
	{
		readonly Dictionary<InputScopeNameValue, Android.Text.InputTypes> lookup
			= new Dictionary<InputScopeNameValue, InputTypes>
			{
				{InputScopeNameValue.Default, InputTypes.TextVariationNormal},
				{InputScopeNameValue.Password, InputTypes.TextVariationPassword},
				{InputScopeNameValue.EmailSmtpAddress, InputTypes.TextVariationEmailAddress},
				{InputScopeNameValue.Url, InputTypes.TextVariationUri},
				{InputScopeNameValue.Text, InputTypes.TextVariationNormal},
				{InputScopeNameValue.NameOrPhoneNumber, InputTypes.ClassPhone},
				{ InputScopeNameValue.Digits, InputTypes.ClassNumber},
				{ InputScopeNameValue.Chat, InputTypes.TextVariationShortMessage},
				{InputScopeNameValue.PostalAddress, InputTypes.TextVariationPostalAddress},
			};

		public InputTypes ToNativeType(InputScopeNameValue value)
		{
			if (lookup.TryGetValue(value, out InputTypes result))
			{
				return result;
			}

			return InputTypes.TextVariationNormal;
		}
	}
}
#endif
