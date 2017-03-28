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

using System;

namespace Codon.UI.Data
{
	public interface IMarkupExtensionRegistry
	{
		void RegisterExtension<T>(string xmlName) where T : IMarkupExtension;

		void RegisterExtension<T>(string xmlName, Func<object[], IMarkupExtension> createExtensionFunc) 
			where T : IMarkupExtension;

		bool TryGetExtensionCreationFunc(
			string xmlName, out Func<object[], IMarkupExtension> resultFunc);

		bool GetExtensionCreationFunc<T>(
			string xmlName, out Func<object[], IMarkupExtension> resultFunc);
	}
}