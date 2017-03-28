using System;

namespace Codon.InversionOfControl
{
	/// <summary>
	/// This class is used to specify a default
	/// concrete type name for the <see cref="FrameworkContainer"/>
	/// to resolve types that do not have an existing type registration.
	/// The <see cref="FrameworkContainer"/> attempts to resolve
	/// the type from referenced assemblies depending on the platform.
	/// This attribute has higher precedence than the 
	/// <seealso cref="DefaultTypeNameAttribute"/>.
	/// <seealso cref="Platform.PlatformDetector"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public class DefaultTypeNameAttribute : Attribute
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="typeName">
		/// The type name of a concrete class that implements
		/// the interface on which this attribute is placed.</param>
		public DefaultTypeNameAttribute(string typeName)
		{
			TypeName = typeName;
		}

		/// <summary>
		/// The type name of a concrete class that implements
		/// the interface on which this attribute is placed.
		/// </summary>
		public string TypeName { get; }

		/// <summary>
		/// If <c>true</c> the object resolved by the 
		/// <see cref="FrameworkContainer"/> is registered as 
		/// a singleton.
		/// </summary>
		public bool Singleton { get; set; }
	}
}