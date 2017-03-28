using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Codon.UI.Elements
{
	public class ActionDialog : Dialog, View.IOnClickListener
	{
		readonly ActionDialogArguments arguments;
		readonly LinearLayout layout;

		public ActionDialog(ActionDialogArguments actionArguments, Context context)
			: base(context)
		{
			arguments = actionArguments;
			layout = new LinearLayout(Context);
		}

		Button AddButton(string name)
		{
			Button child = new Button(Context)
				{
					Text = name
				};
			child.SetOnClickListener(this);
			layout.AddView(child);
			return child;
		}

		void View.IOnClickListener.OnClick(View view)
		{
			int result = -1;

			try
			{
				if (view != null)
				{
					result = layout.IndexOfChild(view);
				}
			}
			catch (Exception ex)
			{
				arguments.Result.SetException(ex);
			}
			finally
			{
				Hide();
			}

			arguments.SetResult(result);
		}

		public override void Cancel()
		{
			base.Cancel();

			arguments.SetResult(-1);
		}

		public override void Dismiss()
		{
			base.Dismiss();

			arguments.SetResult(-1);
		}

		public override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();
			Window.SetGravity(GravityFlags.CenterVertical);
			Window.SetLayout(-1, -2);
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			string actionSheetTitle = arguments.ActionSheetTitle;
			bool showTitle = !string.IsNullOrWhiteSpace(actionSheetTitle);
			if (!showTitle)
			{
				RequestWindowFeature((int)WindowFeatures.NoTitle);
			}

			base.OnCreate(savedInstanceState);

			SetCanceledOnTouchOutside(true);
			layout.Orientation = Orientation.Vertical;

			using (ViewGroup.LayoutParams layoutParams = new ViewGroup.LayoutParams(-1, -1))
			{
				SetContentView(layout, layoutParams);
			}

			if (showTitle)
			{
				SetTitle(actionSheetTitle);
			}
			
			string destructionTitle = arguments.DestructionTitle;
			if (destructionTitle != null)
			{
				Button button = AddButton(destructionTitle);
				var color = new Android.Graphics.Color((byte)255, (byte)0, (byte)0, (byte)255);
				button.Background.SetColorFilter(color, PorterDuff.Mode.Multiply);
			}

			foreach (string buttonText in arguments.Buttons)
			{
				AddButton(buttonText);
			}

			string cancelTitle = arguments.CancelTitle;
			if (cancelTitle != null)
			{
				Button button = AddButton(cancelTitle);
				var color = new Color((byte)127, (byte)127, (byte)127, (byte)255);
				button.Background.SetColorFilter(color, PorterDuff.Mode.Multiply);
			}
		}
	}
}