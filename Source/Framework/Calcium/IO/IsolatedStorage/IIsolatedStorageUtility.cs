#region File and License Information
/*
<File>
	<License>
		Copyright � 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-06-06 10:15:12Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using Calcium.InversionOfControl;

namespace Calcium.IO
{
	/// <summary>
	/// Defines utility methods for an application's isolated storage.
	/// </summary>
	[DefaultType(typeof(IsolatedStorageUtility))]
	public interface IIsolatedStorageUtility
	{
		/// <summary>
		/// Asynchronously copies a resource to a specified
		/// isolated storage folder.
		/// </summary>
		/// <param name="inResourceName"></param>
		/// <param name="outFilename"></param>
		Task CopyApplicationResourceToIsolatedStorageAsync(
			string inResourceName, string outFilename);

		/// <summary>
		/// Asynchronously copies a set of files to a new location 
		/// in isolated storage.
		/// </summary>
		/// <param name="sourceToDestinationList">
		/// The from/to mappings.</param>
		Task CopyApplicationResourcesToIsolatedStorageAsync(
			IEnumerable<KeyValuePair<string, string>> sourceToDestinationList);

		//Task<Stream> GetApplicationResourceStreamAsync(Uri resourceUri);

		/// <summary>
		/// Determines if a file exists in isolated storage.
		/// </summary>
		/// <param name="path">The relative path to the file.</param>
		/// <returns><c>true</c> if the file exists; 
		/// <c>false</c> otherwise.</returns>
		bool FileExists(string path);
	}
}
