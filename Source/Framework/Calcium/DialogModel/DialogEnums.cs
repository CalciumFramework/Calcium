#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.DialogModel
{
	/// <summary>
	/// The result from a user when prompted with a question 
	/// that has three options: Yes, No, and Cancel.
	/// </summary>
	public enum YesNoCancelQuestionResult
	{
		/// <summary>
		/// The user selected 'yes.'
		/// </summary>
		Yes,
		/// <summary>
		/// The user selected 'no.'
		/// </summary>
		No,
		/// <summary>
		/// The user selected 'cancel.'
		/// </summary>
		Cancel
	}

	/// <summary>
	/// The result from a user when prompted with a question 
	/// that has two options: Ok and Cancel.
	/// </summary>
	public enum OkCancelQuestionResult
	{
		/// <summary>
		/// The user selected 'OK.'
		/// </summary>
		OK,
		/// <summary>
		/// The user selected 'Cancel.'
		/// </summary>
		Cancel
	}

	public enum DialogImage
	{
		None,
		Asterisk,
		Error,
		Exclamation,
		Hand,
		Information,
		Question,
		Stop,
		Warning
	}

	public enum DialogButton
	{
		OK = 0,
		OKCancel = 1,
		YesNo = 4,
		YesNoCancel = 3
	}

	public enum DialogResult
	{
		None = 0,
		OK = 1,
		Cancel = 2,
		Yes = 6,
		No = 7
	}
}
