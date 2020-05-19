#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-08-02 19:37:42Z</CreationDate>
</File>
*/
#endregion

using System;
using System.IO;
using System.Threading.Tasks;
using Codon.Cryptography.Implementation;
using Codon.InversionOfControl;

namespace Codon.Cryptography
{
	/// <summary>
	/// A class implementing this interface is used to generate
	/// cryptographic signatures for byte streams, using a private key.
	/// A signature can then be verified using a public key.
	/// </summary>
#if NETSTANDARD2_0 || (!NETSTANDARD1_4 && !NETSTANDARD1_6)
	[DefaultType(typeof(RsaSigner), Singleton = true)]
#endif
	public interface ISigner
	{
		/// <summary>
		/// Creates a signature for the specified stream
		/// using the provided private key. The signature
		/// can be later verified using the corresponding public
		/// key. To obtain a private and public key pair for use
		/// with this method, use the <c>GenerateKeyPair()</c> method.
		/// </summary>
		/// <param name="inputStream">The data for which to generate
		/// a verifiable signature.</param>
		/// <param name="privateKey">The signature is generated using
		/// the specified private key, and can later be verified using
		/// a corresponding public key.</param>
		/// <returns>The signature that should accompany the contents
		/// of the specified input stream.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		Task<byte[]> CreateSignatureAsync(Stream inputStream, string privateKey);

		/// <summary>
		/// Verifies that the signature corresponds to the content
		/// of the specified data stream.
		/// </summary>
		/// <param name="dataStream">
		/// A stream containing data that was signed.</param>
		/// <param name="signatureBytes">
		/// The signature of the data in the specified data stream.</param>
		/// <param name="publicKey">The public key that corresponds
		/// to the private key that was used to generate the signature.</param>
		/// <returns><c>true</c> if the signature is valid;
		/// <c>false</c> otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		Task<bool> VerifySignatureAsync(
			Stream dataStream, byte[] signatureBytes, string publicKey);

		/// <summary>
		/// Generates a public and private key for use with signature
		/// creation and verification.
		/// </summary>
		/// <returns>A pair of keys.</returns>
		KeyPair GenerateKeyPair();
	}
}