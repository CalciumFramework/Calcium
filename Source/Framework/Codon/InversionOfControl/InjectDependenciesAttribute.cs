#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-27 15:13:08Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.InversionOfControl
{
	/// <summary>
	/// This attribute is used to instruct 
	/// the <see cref="FrameworkContainer"/>
	/// to automatically populate properties 
	/// of an object during creation.
	/// It is also used to instruct the <c>FrameworkContainer</c>
	/// to use a specific constructor during object instantiation.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Constructor | 
		AttributeTargets.Property)]
	public class InjectDependenciesAttribute : Attribute
	{
	}
}