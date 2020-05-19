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

namespace Codon.ComponentModel
{
	/// <summary>
	/// A class implementing this interface is able to accept
	/// an object, via its <see cref="AttachObject"/> method,
	/// and retrieve and detach the object.
	/// This interface is useful for attaching commands
	/// to UI elements. See also the <c>AdaptiveCollection</c>
	/// class in the Extras library.
	/// </summary>
	/// <typeparam name="T">The type of object 
	/// that can be attached.</typeparam>
	public interface IAttachObject<T>
	{
		/// <summary>
		/// Attach the specified item
		/// so that it is hosted by another component.
		/// For example, a menu item might know how to 
		/// host a command object.
		/// </summary>
		/// <param name="item">The item to attach.</param>
		void AttachObject(T item);

		/// <summary>
		/// Detach the object so that it is no longer referenced
		/// by the instance.
		/// </summary>
		/// <returns></returns>
		T DetachObject();

		/// <summary>
		/// Gets the attached object.
		/// The object remains attached.
		/// </summary>
		/// <returns>The attached object,
		/// or <c>null</c> if no object is attached.</returns>
		T GetObject();
	}
}
