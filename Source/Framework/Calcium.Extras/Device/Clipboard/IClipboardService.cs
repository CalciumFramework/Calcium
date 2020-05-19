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

using System.Threading.Tasks;
using Calcium.InversionOfControl;

namespace Calcium.Services
{
	/// <summary>
	/// This interface allows the abstraction of the platform
	/// specific clipboard APIs.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." + 
		nameof(Device) + ".ClipboardService, " + 
		AssemblyConstants.ExtrasPlatformAssembly, Singleton = true)]
	public interface IClipboardService
	{
		/// <summary>
		/// Copy the specified text to the clipboard.
		/// A description may be used if the platform supports it.
		/// </summary>
		/// <param name="content">
		/// The text or object to copy to the clipboard.</param>
		/// <param name="description">
		/// A description of the origin or purpose of the text.
		/// Can be <c>null</c>.</param>
		void CopyToClipboard(object content, string description);

		/// <summary>
		/// Gets the contents of the clipboard.
		/// </summary>
		/// <returns>The clipboard contents.</returns>
		Task<object> GetClipboardContentsAsync();
	}
}
