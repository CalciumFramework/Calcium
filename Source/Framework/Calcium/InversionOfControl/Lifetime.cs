#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-07-16 17:47:02Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.InversionOfControl
{
	/// <summary>
	/// Specifies the lifetime of an instance managed
	/// by an inversion-of-control container.
	/// </summary>
	public enum Lifetime
	{
		/// <summary>
		/// A new instance is created for each resolution.
		/// </summary>
		Transient,

		/// <summary>
		/// A single instance is created and reused by the container.
		/// </summary>
		Singleton
	}
}
