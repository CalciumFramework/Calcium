#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-06-06 10:15:12Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Codon.IO
{
	/// <summary>
	/// The default implementation of the <see cref="IIsolatedStorageUtility"/>
	/// interface.
	/// </summary>
	public class IsolatedStorageUtility : IIsolatedStorageUtility
	{
		public async Task CopyApplicationResourceToIsolatedStorageAsync(
							string inResourceName, string outFilename)
		{
			AssertArg.IsNotNull(inResourceName, nameof(inResourceName));
			AssertArg.IsNotNull(outFilename, nameof(outFilename));

			Uri uri = new Uri(inResourceName, UriKind.Relative);

			using (Stream resourceStream = await GetApplicationResourceStreamAsync(uri))//Application.GetResourceStream(uri).Stream)
			{
				using (IsolatedStorageFile isolatedStorageFile
							= IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (IsolatedStorageFileStream outStream
						= isolatedStorageFile.CreateFile(outFilename))
					{
						resourceStream.CopyTo(outStream);
					}
				}
			}
		}

		public async Task CopyApplicationResourcesToIsolatedStorageAsync(
							IEnumerable<KeyValuePair<string, string>> sourceToDestinationList)
		{
			AssertArg.IsNotNull(sourceToDestinationList, nameof(sourceToDestinationList));

			using (IsolatedStorageFile isolatedStorageFile
						= IsolatedStorageFile.GetUserStoreForApplication())
			{
				foreach (var sourceAndDestination in sourceToDestinationList)
				{
					Uri uri = new Uri(sourceAndDestination.Key, UriKind.Relative);
					using (Stream resourceStream = await GetApplicationResourceStreamAsync(uri))//Application.GetResourceStream(uri).Stream)
					{
						string destination = sourceAndDestination.Value;
						if (string.IsNullOrWhiteSpace(destination))
						{
							throw new ArgumentException($"Key '{sourceAndDestination.Key}' has null pair Value. A destination must be specified.");
						}

						int separatorIndex = destination.LastIndexOf("/");
						if (separatorIndex == destination.Length - 1)
						{
							throw new InvalidOperationException(
								$"Destination '{destination}' should not end with '/'");
						}
						string directory = null;
						if (separatorIndex != -1)
						{
							directory = destination.Substring(0, separatorIndex);
						}

						if (!string.IsNullOrWhiteSpace(directory)
							&& !isolatedStorageFile.DirectoryExists(directory))
						{
							isolatedStorageFile.CreateDirectory(directory);
						}

						//						if (isolatedStorageFile.FileExists(destination))
						//						{
						//							isolatedStorageFile.DeleteFile(destination);
						//						}

						using (IsolatedStorageFileStream outStream
									= isolatedStorageFile.CreateFile(destination))
						{
							resourceStream.CopyTo(outStream);
						}
					}
				}
			}
		}

#if WINDOWS_UWP || NETFX_CORE
		async Task<Stream> GetApplicationResourceStreamAsync(Uri resourceUri)
		{
			StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(resourceUri);
			Stream sr = await file.OpenStreamForReadAsync();
			return sr;
		}
#else
		Task<Stream> GetApplicationResourceStreamAsync(Uri resourceUri)
		{
#if NETSTANDARD
			throw new NotImplementedException("Not implemented yet for .NET Core.");
#else
			Stream resourceStream = Application.GetResourceStream(resourceUri).Stream;
			return resourceStream;
#endif
		}
#endif

		public bool FileExists(string path)
		{
			AssertArg.IsNotNull(path, nameof(path));

			using (IsolatedStorageFile isolatedStorageFile 
				= IsolatedStorageFile.GetUserStoreForApplication())
			{
				return isolatedStorageFile.FileExists(path);
			}
		}
	}

}
