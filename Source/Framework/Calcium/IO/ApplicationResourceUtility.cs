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

using System;
using System.IO;
using System.Threading.Tasks;

namespace Calcium.IO
{
	/// <summary>
	/// This class is used to retrieve files from the application package.
	/// </summary>
	public static class ApplicationResourceUtility
	{
#if NETSTANDARD
		/// <summary>
		/// Not yet implemented.
		/// </summary>
		/// <param name="resourceUri"></param>
		/// <returns></returns>
		public static Task<Stream> GetApplicationResourceStreamAsync(Uri resourceUri)
		{
			throw new NotImplementedException("It is expected that this method will be available with .NET Standard 2.0");
			//System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream();
			//StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(resourceUri);
			//Stream resourceStream = await file.OpenStreamForReadAsync();
			//return resourceStream;
		}
#elif WINDOWS_UWP || NETFX_CORE
		public static async Task<Stream> GetApplicationResourceStreamAsync(Uri resourceUri)
		{
			StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(resourceUri);
			Stream resourceStream = await file.OpenStreamForReadAsync();
			return resourceStream;
		}
#else
		public static async Task<Stream> GetApplicationResourceStreamAsync(Uri resourceUri)
		{
			Stream resourceStream = Application.GetResourceStream(resourceUri).Stream;
			return resourceStream;
		}
#endif
	}
}
