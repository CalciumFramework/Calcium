#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-21 19:29:16Z</CreationDate>
</File>
*/
#endregion

namespace Codon.UserOptionsModel
{
	public interface IOptionCategory
	{
		object Id
		{
			get;
		}

		object Title
		{
			get;
		}

		void Refresh();
	}
}