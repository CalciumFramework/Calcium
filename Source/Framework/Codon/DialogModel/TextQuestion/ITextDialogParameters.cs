#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-09-08 17:53:20Z</CreationDate>
</File>
*/
#endregion

using Codon.MissingTypes.System.Windows.Input;

namespace Codon.DialogModel
{
	public interface ITextDialogParameters
	{
		string Caption { get; }

		string ValidationExpression { get; }
		string ValidationFailedMessage { get; }
		string RestictionExpression { get; }
		InputScopeNameValue InputScope { get; }
	}
}