#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-21 19:13:08Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;

namespace Calcium.UserOptionsModel
{
	public interface IUserOptionReaderWriter
	{
		IUserOption UserOption
		{
			get;
			set;
		}
		
		bool Dirty
		{
			get;
		}

		/// <summary>
		///	The use of this method should be prefered over the Setting property
		/// in cases where data-binding is not being performed.
		/// The <see cref="Setting"/> property may require an asynchronous call
		/// to retrieve the value, which then must raised the OnPropertyChanged
		/// event to cause re-reading of the property.
		/// </summary>
		/// <returns></returns>
		Task<object> GetSettingAsync();

		object Setting { get; set; }

		Task<SaveOptionResult> Save();

		Task Refresh(bool reload = false);
	}
}
