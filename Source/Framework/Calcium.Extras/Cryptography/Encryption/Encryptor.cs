#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-08-01 20:25:22Z</CreationDate>
</File>
*/
#endregion

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Calcium.Cryptography.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IEncryptor"/>.
	/// </summary>
	public class Encryptor : IEncryptor
	{
		/// <summary>
		/// Override this method to change the way salts are generated.
		/// </summary>
		/// <returns>A <c>byte[]</c> that is appended
		/// to encrypted data.</returns>
		protected virtual byte[] CreateSalt()
		{
			return Guid.NewGuid().ToByteArray();
		}

		/// <summary>
		/// Override this method to change the length of the salt
		/// data that is appended to encrypted data. This value
		/// must be equal to, or less than, the array returned
		/// from <see cref="CreateSalt"/>.
		/// </summary>
		protected virtual int SaltLength => 16;

		public async Task EncryptAsync(
			Stream plainStream, string password, Stream outputStream)
		{

			AssertArg.IsNotNull(plainStream, nameof(plainStream));
			AssertArg.IsNotNull(password, nameof(password));

			byte[] salt = CreateSalt();

			using (Aes aes = Aes.Create())
			{
				var deriveBytes = new Rfc2898DeriveBytes(password, salt);
				aes.Key = deriveBytes.GetBytes(128 / 8);
				aes.IV = aes.Key;
				await outputStream.WriteAsync(salt, 0, SaltLength);

				/* A temporary stream is used because CryptoStream automatically closes
				 * the output stream. This behaviour is changable in .NET 4.7.2,
				 * which includes a new CryptoStream constructor
				 * with a leaveOpen parameter. */
				using (var tempStream = new MemoryStream())
				{
					using (var cryptoStream = new CryptoStream(
						tempStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
					{
						await plainStream.CopyToAsync(cryptoStream);
						cryptoStream.FlushFinalBlock();
						tempStream.Position = 0;
						await tempStream.CopyToAsync(outputStream);
					}
				}
			}
		}

		public async Task DecryptAsync(
			Stream encryptedStream, string password, Stream outputStream)
		{
			AssertArg.IsNotNull(encryptedStream, nameof(encryptedStream));
			AssertArg.IsNotNull(password, nameof(password));

			byte[] salt = new byte[SaltLength];
			await encryptedStream.ReadAsync(salt, 0, SaltLength);

			using (Aes aes = Aes.Create())
			{
				var deriveBytes = new Rfc2898DeriveBytes(password, salt);
				aes.Key = deriveBytes.GetBytes(128 / 8);
				aes.IV = aes.Key;

				/* A temporary stream is used because CryptoStream automatically closes
				 * the output stream. This behaviour is changable in .NET 4.7.2,
				 * which includes a new CryptoStream constructor
				 * with a leaveOpen parameter. */
				using (var tempStream = new MemoryStream())
				{
					using (var cryptoStream = new CryptoStream(
						tempStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
					{
						await encryptedStream.CopyToAsync(cryptoStream);
						cryptoStream.FlushFinalBlock();
						tempStream.Position = 0;
						await tempStream.CopyToAsync(outputStream);
					}
				}
			}
		}
	}
}
