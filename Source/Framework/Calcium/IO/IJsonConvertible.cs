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

namespace Codon.IO
{
	/// <summary>
	/// Provides for conversion and population to and from JSON.
	/// Allows an object instance to be converted 
	/// to JSON and perhaps serialized.
	/// </summary>
	public interface IJsonConvertible
	{
		/// <summary>
		/// Populates the instance from the values 
		/// in the specified JSON string.
		/// This is the reverse process of <see cref="ToJson"/>.
		/// </summary>
		/// <param name="json">The json text.</param>
		void FromJson(string json);

		/// <summary>
		/// Convert to a JSON string. 
		/// This is the reverse process of <see cref="FromJson"/>.
		/// </summary>
		/// <returns>A string representing the current 
		/// object instance in JSON format.</returns>
		string ToJson();
	}
}
