#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-01-30 19:39:31Z</CreationDate>
</File>
*/
#endregion

using System.Xml.Linq;

namespace Calcium.IO
{
	/// <summary>
	/// Provides for conversion and population to and from an XElement.
	/// Allows an object instance to be converted to XML.
	/// </summary>
	public interface IXmlConvertible
	{
		/// <summary>
		/// Populates the instance from the values 
		/// in the specified element.
		/// This is the reverse process of <see cref="ToXElement"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		void FromXElement(XElement element);

		/// <summary>
		/// Convert to an XElement. 
		/// This is the reverse process of <see cref="FromXElement"/>.
		/// </summary>
		/// <returns>An element representing 
		/// the current object instance.</returns>
		XElement ToXElement();
	}
}
