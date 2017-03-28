using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codon.UI.Elements
{
	public class ActionDialogArguments
	{
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

		public void SetResult(int result)
		{
			Result.TrySetResult(result);
		}

		public IEnumerable<string> Buttons { get; private set; }

		public string CancelTitle { get; private set; }

		public string DestructionTitle { get; private set; }

		public TaskCompletionSource<int> Result { get; private set; }

		public string ActionSheetTitle { get; private set; }
	}
}