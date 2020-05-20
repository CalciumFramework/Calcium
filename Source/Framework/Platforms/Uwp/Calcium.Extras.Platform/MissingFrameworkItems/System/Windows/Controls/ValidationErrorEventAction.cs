#if WINDOWS_UWP || NETFX_CORE
using Windows.UI.Xaml;

namespace System.Windows.Controls
{
	public enum ValidationErrorEventAction
	{
		Added,
		Removed
	}

	public class ValidationErrorEventArgs : RoutedEventArgs
	{
		// Methods
		internal ValidationErrorEventArgs(
			ValidationErrorEventAction action, ValidationError error)
		{
			Action = action;
			Error = error;
			Handled = false;
		}

		// Properties
		public ValidationErrorEventAction Action { get; internal set; }

		public ValidationError Error { get; internal set; }

		public bool Handled { get; set; }
	}

	public class ValidationError
	{
		// Methods
		internal ValidationError(object errorContent)
		{
			ErrorContent = errorContent;
		}

		internal ValidationError(Exception exception, bool useDefaultString)
		{
			Exception = exception;
			if (useDefaultString)
			{
				/* TODO: Make localizable resource. */
				ErrorContent = "Invalid";
			}
			else
			{
				ErrorContent = exception.Message;
			}
		}

		// Properties
		public object ErrorContent { get; private set; }

		public Exception Exception { get; private set; }
	}
}
#endif