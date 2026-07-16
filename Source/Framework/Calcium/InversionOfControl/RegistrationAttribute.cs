#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-07-16 17:49:43Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.InversionOfControl
{
	/// <summary>
	/// Specifies how a concrete type should be registered when it is
	/// implicitly resolved by an inversion-of-control container.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class,
					AllowMultiple = false,
					Inherited = false)]
	public sealed class RegistrationAttribute : Attribute
	{
		/// <summary>
		/// Initialises a new instance of the
		/// <see cref="RegistrationAttribute"/> class.
		/// </summary>
		/// <param name="lifetime">
		/// The lifetime of instances created by the container.
		/// </param>
		public RegistrationAttribute(Lifetime lifetime)
		{
			Lifetime = lifetime;
		}

		/// <summary>
		/// Gets the lifetime of instances created by the container.
		/// </summary>
		public Lifetime Lifetime { get; }
	}
}