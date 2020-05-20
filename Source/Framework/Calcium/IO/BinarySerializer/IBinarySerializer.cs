#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System.IO;
using Calcium.InversionOfControl;

namespace Calcium.IO
{
	/// <summary>
	/// This interface defines the required capabilities
	/// of a binary serializer that is used, for example,
	/// by the framework to persist objects as settings. 
	/// </summary>
	[DefaultType(typeof(BinarySerializer), Singleton = true)]
	public interface IBinarySerializer
	{
		/// <summary>
		/// Converts an <c>object</c> to a <c>byte[]</c>.
		/// This method performs the inverse operation 
		/// of <see cref="Deserialize"/>.
		/// </summary>
		/// <param name="item">The object to convert.</param>
		/// <returns>A <c>byte[]</c> representing 
		/// the specified item.</returns>
		byte[] Serialize(object item);

		/// <summary>
		/// Converts an <c>object</c> to a <c>byte[]</c>
		/// and writes it to the specified output stream.
		/// This method performs the inverse operation 
		/// of <see cref="Deserialize"/>.
		/// </summary>
		/// <param name="item">The object to convert.</param>
		/// <param name="outputStream">
		/// The stream to write the specified item.</param>
		void Serialize(object item, Stream outputStream);

		/// <summary>
		/// Converts the specified <c>byte[]</c>
		/// to an object of type <c>T</c>.
		/// This is the inverse operation to 
		/// <see cref="Serialize(object)"/>
		/// </summary>
		/// <typeparam name="T">The type of the object
		/// to be returned.</typeparam>
		/// <param name="array">The serialized object array.</param>
		/// <returns>The object represented by the specified array.
		/// </returns>
		/// <exception cref="System.InvalidCastException">
		/// Occurs if the object represented by the specified array
		/// is not of the type specified by the type parameter <c>T</c>.
		/// </exception>
		T Deserialize<T>(byte[] array) where T : class;

		/// <summary>
		/// Converts the specified <c>Stream</c>
		/// to an object of type <c>T</c>.
		/// This is the inverse operation to 
		/// <see cref="Serialize(object, Stream)"/>
		/// </summary>
		/// <typeparam name="T">The type of the object
		/// to be returned.</typeparam>
		/// <param name="stream">The serialized object stream.</param>
		/// <returns>The object represented by the specified stream.
		/// </returns>
		/// <exception cref="System.InvalidCastException">
		/// Occurs if the object represented by the specified array
		/// is not of the type specified by the type parameter <c>T</c>.
		/// </exception>
		T Deserialize<T>(Stream stream) where T : class;

		/// <summary>
		/// Converts the specified <c>Stream</c>
		/// to an object of type <c>T</c>.
		/// This is the inverse operation to 
		/// <see cref="Serialize(object, Stream)"/>
		/// </summary>
		/// <param name="inputStream">
		/// The serialized object stream.</param>
		/// <returns>The object represented by the specified stream.
		/// </returns>
		object Deserialize(Stream inputStream/*, object instance = null*/);
	}
}
