#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-10 11:39:11Z</CreationDate>
</File>
*/
#endregion

using System;
using System.IO;

namespace Calcium.IO
{
	/// <summary>
	/// Extension methods for the <see cref="Stream"/> class.
	/// </summary>
	public static class StreamExtensions
	{
		/// <summary>
		/// Copies the bytes from one <c>Stream</c> to another.
		/// </summary>
		/// <param name="fromStream">
		/// The Stream containing the data to be copied.</param>
		/// <param name="toStream">
		/// The destination stream.</param>
		/// <param name="closeToStream">
		/// If <c>true</c> the specified <c>toStream</c>
		/// will be closed after the bytes are copied.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if either <c>Stream</c> is <c>null</c>.</exception>
		public static void CopyToStream(
			this Stream fromStream, 
			Stream toStream, 
			bool closeToStream = true)
		{
			AssertArg.IsNotNull(fromStream, nameof(fromStream));
			AssertArg.IsNotNull(toStream, nameof(toStream));

			CopyStreamBytes(fromStream, toStream, closeToStream);
		}

		static void CopyStreamBytes(
			Stream fromStream, 
			Stream toStream, 
			bool closeToStream = true)
		{
			if (toStream.CanWrite)
			{
				byte[] fileBytes = ReadStreamBytes(fromStream);
				toStream.Write(fileBytes, 0, fileBytes.Length);
				if (closeToStream)
				{
					toStream.Dispose();
				}
			}
		}

		/// <summary>
		/// Reads alls bytes from the specified <c>Stream</c>.
		/// </summary>
		/// <param name="stream">The stream to read.</param>
		/// <returns>All bytes in the specified <c>Stream</c>.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified <c>Stream</c> is <c>null</c>.</exception>
		public static byte[] ToBytes(this Stream stream)
		{
			AssertArg.IsNotNull(stream, nameof(stream));
			return ReadStreamBytes(stream);
		}

		static byte[] ReadStreamBytes(Stream fileStream)
		{
			/* Read the source file into a byte array. */
			byte[] bytes = new byte[fileStream.Length];
			int readLength = (int)fileStream.Length;
			int bytesRead = 0;
			while (readLength > 0)
			{
				/* Read may return anything from 0 to readLength. */
				int read = fileStream.Read(bytes, bytesRead, readLength);

				/* When no bytes left to read it is the end of the file. */
				if (read == 0)
				{
					break;
				}

				bytesRead += read;
				readLength -= read;
			}

			return bytes;
		}
	}
}
