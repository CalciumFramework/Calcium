using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codon.UI.Elements
{
	/// <summary>
	/// The arguments supplied when opening an ActionDialog.
	/// </summary>
	public class ActionDialogArguments
	{
		/// <inheritdoc />
		public ActionDialogArguments(
			string actionSheetTitle, 
			string cancelTitle, 
			string destructionTitle, 
			IEnumerable<string> buttons)
		{
			ActionSheetTitle = actionSheetTitle;
			CancelTitle = cancelTitle;
			DestructionTitle = destructionTitle;
			Buttons = buttons;

			Result = new TaskCompletionSource<int>();
		}

		/// <summary>
		/// Tries to set the result
		/// for the <see cref="TaskCompletionSource{TResult}"/> Result object.
		/// </summary>
		/// <param name="result"></param>
		public void SetResult(int result)
		{
			Result.TrySetResult(result);
		}

		/// <summary>
		/// Text for each button.
		/// </summary>
		public IEnumerable<string> Buttons { get; private set; }

		/// <summary>
		/// Text to display on the cancel button.
		/// </summary>
		public string CancelTitle { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string DestructionTitle { get; private set; }

		/// <summary>
		/// The result of the dialog.
		/// </summary>
		public TaskCompletionSource<int> Result { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string ActionSheetTitle { get; private set; }
	}
}