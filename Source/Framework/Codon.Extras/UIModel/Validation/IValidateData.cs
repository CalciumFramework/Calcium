#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-09-21 17:24:28Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;

namespace Codon.UIModel.Validation
{
	/// <summary>
	/// A class that implements this method is able to validate
	/// a member.
	/// </summary>
	public interface IValidateData
	{
		/// <summary>
		/// Validate the class member with the specified name.
		/// </summary>
		/// <param name="propertyName">
		/// The name of the property to be validated.</param>
		/// <param name="value">
		/// The proposed new value for the member.</param>
		/// <returns>A <see cref="ValidationCompleteEventArgs"/> 
		/// indicating whether the property has validation errors.
		/// </returns>
		Task<ValidationCompleteEventArgs> ValidateAsync(
			string propertyName, object value);
	}
}