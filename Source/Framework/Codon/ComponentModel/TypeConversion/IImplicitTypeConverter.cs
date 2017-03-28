using System;
using Codon.InversionOfControl;

namespace Codon.ComponentModel
{
	/// <summary>
	/// A class implementing this interface is designed to convert
	/// an object to another object of a specified type.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(ComponentModel) 
		+ ".ImplicitTypeConverter, " + AssemblyConstants.PlatformAssembly, Singleton = true)]
	[DefaultType(typeof(DefaultImplicitTypeConverter), Singleton = true)]
	public interface IImplicitTypeConverter
    {
		/// <summary>
		/// Attempts to convert an object to the specified type.
		/// </summary>
		/// <param name="value">The value that requires conversion.</param>
		/// <param name="type">The type of object 
		/// to change the object to.</param>
		/// <returns>The converted object, or <c>null</c>.</returns>
	    object ConvertToType(object value, Type type);
    }
}
