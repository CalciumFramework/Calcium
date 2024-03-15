using System;
using System.Diagnostics.CodeAnalysis;

namespace Calcium.InversionOfControl
{
	/// <summary>
	/// This class is used to specify a default
	/// concrete type for the <see cref="FrameworkContainer"/>
	/// to resolve types that do not have an existing type registration.
	/// This attribute has lower precedence than the 
	/// <see cref="DefaultTypeNameAttribute"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public class DefaultTypeAttribute : Attribute
	{
		public DefaultTypeAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
		{
			Type = type;
		}

		/// <summary>
		/// The concrete type that is resolved by the 
		/// <see cref="FrameworkContainer"/>.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// If <c>true</c> the object resolved by the 
		/// <see cref="FrameworkContainer"/> is registered as 
		/// a singleton.
		/// </summary>
		public bool Singleton { get; set; }
	}
}
