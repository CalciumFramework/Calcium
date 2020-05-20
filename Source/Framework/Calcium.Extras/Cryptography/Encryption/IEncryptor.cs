#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-08-01 20:25:50Z</CreationDate>
</File>
*/
#endregion

using System.IO;
using System.Threading.Tasks;
using Calcium.InversionOfControl;

namespace Calcium.Cryptography
{
	/// <summary>
	/// An <c>IEncryptor</c> is used to encrypt and decrypt byte streams.
	/// </summary>
	[DefaultType(typeof(Implementation.Encryptor), Singleton = true)]
	public interface IEncryptor
	{
		/// <summary>
		/// Encrypt data from <c>plainStream</c> to <c>outputStream</c>
		/// using the specified password.
		/// </summary>
		/// <param name="plainStream">The unencrypted data stream
		/// that requires encrypting.</param>
		/// <param name="password">A password that is used to encrypt the data
		/// and can later be used to unencrypt the data.</param>
		/// <param name="outputStream">The resulting encrypted bytes are placed
		/// in the <c>outputStream</c>.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		Task EncryptAsync(Stream plainStream, string password, Stream outputStream);

		/// <summary>
		/// Decrypts data from <c>encryptedStream</c> to <c>outputStream</c>
		/// using the specified password.
		/// </summary>
		/// <param name="encryptedStream">The encrypted data stream
		/// that requires decryption.</param>
		/// <param name="password">A password that is used to decrypt the data.</param>
		/// <param name="outputStream">The resulting decrypted bytes are placed
		/// in the <c>outputStream</c>.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		Task DecryptAsync(Stream encryptedStream, string password, Stream outputStream);
	}
}
