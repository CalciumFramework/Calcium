#if WPF || WPF_CORE
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-04-25 14:52:16Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Windows;

namespace Codon.DialogModel
{
	/// <summary>
	/// This class is used to translate System.Windows MessageBox enum
	/// values to Framework enum values.
	/// </summary>
	public static class MessageBoxEnumTranslator
	{
		public static MessageBoxButton TranslateToMessageBoxButton(this DialogButton messageBoxButton)
		{
			return Translate(messageBoxButton);
		}

		public static MessageBoxButton Translate(DialogButton button)
		{
			switch (button)
			{
				case DialogButton.OK:
					return MessageBoxButton.OK;
				case DialogButton.OKCancel:
					return MessageBoxButton.OKCancel;
				case DialogButton.YesNo:
					return MessageBoxButton.YesNo;
				case DialogButton.YesNoCancel:
					return MessageBoxButton.YesNoCancel;
			}
			throw new InvalidOperationException("Unknown button type: " + button);
		}

				public static MessageBoxImage TranslateToMessageBoxButton(this DialogImage image)
		{
			return Translate(image);
		}

		public static MessageBoxImage Translate(DialogImage image)
		{
			switch (image)
			{
				case DialogImage.Asterisk:
					return MessageBoxImage.Asterisk;
				case DialogImage.Error:
					return MessageBoxImage.Error;
				case DialogImage.Exclamation:
					return MessageBoxImage.Exclamation;
				case DialogImage.Hand:
					return MessageBoxImage.Hand;
				case DialogImage.Information:
					return MessageBoxImage.Information;
				case DialogImage.None:
					return MessageBoxImage.None;
				case DialogImage.Question:
					return MessageBoxImage.Question;
				case DialogImage.Stop:
					return MessageBoxImage.Stop;
				case DialogImage.Warning:
					return MessageBoxImage.Warning;
			}
			throw new InvalidOperationException("Unknown image type: " + image);
		}

		public static MessageBoxResult TranslateToMessageBoxResult(this DialogResult messageResult)
		{
			return Translate(messageResult);
		}

		public static MessageBoxResult Translate(DialogResult messageResult)
		{
			switch (messageResult)
			{
				case DialogResult.Cancel:
					return MessageBoxResult.Cancel;
				case DialogResult.No:
					return MessageBoxResult.No;
				case DialogResult.None:
					return MessageBoxResult.None;
				case DialogResult.OK:
					return MessageBoxResult.OK;
				case DialogResult.Yes:
					return MessageBoxResult.Yes;
			}
			throw new InvalidOperationException("Unknown messageResult type: " + messageResult);
		}

		public static DialogResult TranslateToMessageBoxResult(this MessageBoxResult messageBoxResult)
		{
			return Translate(messageBoxResult);
		}

		public static DialogResult Translate(MessageBoxResult messageBoxResult)
		{
			switch (messageBoxResult)
			{
				case MessageBoxResult.Cancel:
					return DialogResult.Cancel;
				case MessageBoxResult.No:
					return DialogResult.No;
				case MessageBoxResult.None:
					return DialogResult.None;
				case MessageBoxResult.OK:
					return DialogResult.OK;
				case MessageBoxResult.Yes:
					return DialogResult.Yes;
			}
			throw new InvalidOperationException("Unknown messageBoxResult type: " + messageBoxResult);
		}
	}
}
#endif