#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-08-21 16:16:19Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calcium.Text
{
	/// <summary>
	/// Extension methods for the <c>string</c> class.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Determines whether the specified text contains 
		/// the specified substring.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="substring">The substring.</param>
		/// <returns>
		/// <c>true</c> if the specified text contains 
		/// the specified substring; 
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if either parameter is <c>null</c>.</exception>
		public static bool ContainsIgnoreCase(this string text, string substring)
		{
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(substring, nameof(substring));

			return text.IndexOf(substring, StringComparison.OrdinalIgnoreCase) > -1;
		}

		/// <summary>
		/// Converts the specified string to a byte array using 
		/// its raw character values.
		/// This is the companion method of <see cref="ConvertToUnencodedString"/>.
		/// </summary>
		/// <param name="text">The string value. Cannot be null.</param>
		/// <returns>The bytes of the characters in the string.</returns>
		public static byte[] ConvertToUnencodedBytes(this string text)
		{
			byte[] bytes = new byte[text.Length * sizeof(char)];
			Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		/// <summary>
		/// Converts the specified byte array to a string.
		/// This is the companion method of <see cref="ConvertToUnencodedBytes"/>.
		/// Do not use this if you wish to store the value in an XML file. 
		/// It may result in high-end character values that are incompatible 
		/// with XML. Instead use <see cref="ConvertToUtf8String"/>.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns>The string containing character made up of the byte array values.</returns>
		public static string ConvertToUnencodedString(this byte[] bytes)
		{
			char[] characters = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, characters, 0, bytes.Length);
			return new string(characters);
		}

		/// <summary>
		/// Converts the specified string to a byte array 
		/// using UTF8 character encoding.
		/// This is the companion method of <see cref="ConvertToUtf8String"/>.
		/// </summary>
		/// <param name="text">The string value. Cannot be null.</param>
		/// <returns>The bytes of the characters in the string.</returns>
		public static byte[] ConvertUtf8StringToBytes(this string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return bytes;
		}

		/// <summary>
		/// Converts a byte array to a UTF8 encoded string.
		/// This is the companion method of <see cref="ConvertUtf8StringToBytes"/>.
		/// </summary>
		/// <param name="bytes">
		/// The bytes containing UTF8 encoded character values.</param>
		/// <returns>A UTF8 encoded string.</returns>
		public static string ConvertToUtf8String(this byte[] bytes)
		{
			string result = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			return result;
		}

		/// <summary>
		/// Converts the specified string to a byte array 
		/// using UTF16 character encoding.
		/// This is the companion method of <see cref="ConvertToUtf16String"/>.
		/// </summary>
		/// <param name="text">The string value. Cannot be null.</param>
		/// <returns>The bytes of the characters in the string.</returns>
		public static byte[] ConvertUtf16StringToBytes(this string text)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(text);
			return bytes;
		}

		/// <summary>
		/// Converts a byte array to a UTF16 encoded string.
		/// This is the companion method 
		/// of <see cref="ConvertUtf16StringToBytes"/>.
		/// </summary>
		/// <param name="bytes">
		/// The bytes containing UTF16 encoded character values.</param>
		/// <returns>A UTF16 encoded string.</returns>
		public static string ConvertToUtf16String(this byte[] bytes)
		{
			string result = Encoding.Unicode.GetString(
												bytes, 0, bytes.Length);
			return result;
		}

		/// <summary>
		/// Indicates whether a specified string is <c>null</c>, empty, 
		/// or consists only of white-space characters.
		/// </summary>
		/// <param name="text">The string to test.</param>
		/// <returns><c>true</c> if the specified string is <c>null</c>, empty, 
		/// or consists only of white-space characters; <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrWhiteSpace(this string text)
		{
			return string.IsNullOrWhiteSpace(text);
		}

		/// <summary>
		/// Indicates whether the specified string is <c>null</c> 
		/// or has a length of zero.
		/// </summary>
		/// <param name="text">The string to test.</param>
		/// <returns><c>true</c> if the specified string is <c>null</c> or empty; 
		/// <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrEmpty(this string text)
		{
			return string.IsNullOrEmpty(text);
		}
	}
}
