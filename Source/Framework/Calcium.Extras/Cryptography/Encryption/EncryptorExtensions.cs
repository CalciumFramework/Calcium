#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-08-01 20:25:40Z</CreationDate>
</File>
*/
#endregion

using System.IO;
using System.Threading.Tasks;

namespace Calcium.Cryptography
{
	/// <summary>
	/// Extension methods for the <see cref="IEncryptor"/> implementation.
	/// </summary>
	public static class EncryptorExtensions
	{
		/// <summary>
		/// Encryptes a <c>byte</c> array using
		/// the specified password. The encrypted data
		/// is placed in the <c>outputStream</c>.
		/// </summary>
		/// <param name="encryptor">An <c>IEncryptor</c> instance.</param>
		/// <param name="plainBytes">The bytes to be encrypted.
		/// It is not modified.</param>
		/// <param name="password">A password to use for encryption.</param>
		/// <param name="outputStream">A stream to copy the encrypted bytes.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		public static async Task EncryptAsync(this IEncryptor encryptor,
			byte[] plainBytes, string password, Stream outputStream)
		{
			AssertArg.IsNotNull(encryptor, nameof(encryptor));
			AssertArg.IsNotNull(plainBytes, nameof(plainBytes));
			AssertArg.IsNotNull(password, nameof(password));
			AssertArg.IsNotNull(outputStream, nameof(outputStream));

			await EncryptCoreAsync(encryptor, plainBytes, password, outputStream);
		}

		/// <summary>
		/// Encryptes a <c>byte</c> array using
		/// the specified password. The encrypted data
		/// is returned as a byte array.
		/// </summary>
		/// <param name="encryptor">An <c>IEncryptor</c> instance.</param>
		/// <param name="plainBytes">The bytes to be encrypted.
		/// It is not modified.</param>
		/// <param name="password">A password to use for encryption.</param>
		/// <returns>An array of encrypted bytes.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		public static async Task<byte[]> EncryptAsync(this IEncryptor encryptor,
			byte[] plainBytes, string password)
		{
			AssertArg.IsNotNull(encryptor, nameof(encryptor));
			AssertArg.IsNotNull(plainBytes, nameof(plainBytes));
			AssertArg.IsNotNull(password, nameof(password));

			return await EncryptCoreAsync(encryptor, plainBytes, password);
		}

		static async Task<byte[]> EncryptCoreAsync(IEncryptor encryptor,
			byte[] plainBytes, string password)
		{
			using (var outputStream = new MemoryStream())
			{
				using (var inputStream = new MemoryStream(plainBytes))
				{
					await encryptor.EncryptAsync(inputStream, password, outputStream);

				}

				byte[] result = outputStream.ToArray();
				return result;
			}
		}

		static async Task EncryptCoreAsync(IEncryptor encryptor,
			byte[] plainBytes, string password, Stream outputStream)
		{
			using (var inputStream = new MemoryStream(plainBytes))
			{
				await encryptor.EncryptAsync(inputStream, password, outputStream);
			}
		}

		/// <summary>
		/// Decrypts a <c>byte</c> array using the specified password. 
		/// </summary>
		/// <param name="encryptor">The <c>IEncryptor</c> instance.</param>
		/// <param name="encryptedBytes">The encrpted bytes that are to be decrypted.
		/// This is not modified.</param>
		/// <param name="password">A password to use for encryption.</param>
		/// <returns>The unencrypted bytes.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		/// <returns>The decrypted bytes.</returns>
		public static async Task<byte[]> DecryptAsync(this IEncryptor encryptor,
			byte[] encryptedBytes, string password)
		{
			AssertArg.IsNotNull(encryptor, nameof(encryptor));
			AssertArg.IsNotNull(encryptedBytes, nameof(encryptedBytes));
			AssertArg.IsNotNull(password, nameof(password));

			return await DecryptCoreAsync(encryptor, encryptedBytes, password);
		}

		/// <summary>
		/// Decrypts a <c>byte</c> array using the specified password. 
		/// </summary>
		/// <param name="encryptor">The <c>IEncryptor</c> instance.</param>
		/// <param name="encryptedBytes">
		/// The encrpted bytes that are to be decrypted.
		/// This is not modified.</param>
		/// <param name="password">
		/// A password to use for encryption.</param>
		/// <param name="outputStream">
		/// Decrypted data is copied to this stream.</param>
		/// <returns>The unencrypted bytes.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		public static async Task DecryptAsync(this IEncryptor encryptor,
			byte[] encryptedBytes, string password, Stream outputStream)
		{
			AssertArg.IsNotNull(encryptor, nameof(encryptor));
			AssertArg.IsNotNull(encryptedBytes, nameof(encryptedBytes));
			AssertArg.IsNotNull(password, nameof(password));
			AssertArg.IsNotNull(outputStream, nameof(outputStream));

			await DecryptCoreAsync(encryptor, encryptedBytes, password, outputStream);
		}

		static async Task<byte[]> DecryptCoreAsync(IEncryptor encryptor,
			byte[] encryptedBytes, string password)
		{
			using (var outputStream = new MemoryStream())
			{
				await DecryptCoreAsync(encryptor, encryptedBytes, password, outputStream);

				byte[] decryptedBytes = outputStream.ToArray();
				return decryptedBytes;
			}
		}

		static async Task DecryptCoreAsync(IEncryptor encryptor,
			byte[] encryptedBytes, string password, Stream outputStream)
		{
			using (var encryptedStream = new MemoryStream(encryptedBytes))
			{
				await encryptor.DecryptAsync(encryptedStream, password, outputStream);
			}
		}
	}
}
