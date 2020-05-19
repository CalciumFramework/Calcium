#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using Codon.InversionOfControl;

namespace Codon.UI.Data
{
	public abstract class MarkupExtensionBase : IMarkupExtension
	{
		public abstract object ProvideValue(IContainer iocContainer/*, object[] parameters*/);
	}
}