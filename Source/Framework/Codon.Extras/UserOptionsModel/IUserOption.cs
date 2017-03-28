#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-23 17:06:31Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;

namespace Codon.UserOptionsModel
{
	public interface IUserOption
	{
		string Title
		{
			get;
		}

		string ImagePath
		{
			get;
		}

		string Description
		{
			get;
		}

		string SettingKey
		{
			get;
		}

		string TemplateName
		{
			get;
		}

		object DefaultValue
		{
			get;
		}

		IUserOptionReaderWriter ReaderWriter { get; set; }

		Task Refresh(bool reload = false);

		Type SettingType { get; }

//		void Save();

//		bool CanSaveSelf
//		{
//			get;
//		}
	}
}