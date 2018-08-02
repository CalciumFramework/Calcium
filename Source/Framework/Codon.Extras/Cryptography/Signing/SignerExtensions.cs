#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-08-02 19:38:03Z</CreationDate>
</File>
*/
#endregion
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Codon.Cryptography
{
	public static class SignerExtensions
	{
		static readonly Encoding defaultEncoding = Encoding.UTF8;

		/// <summary>
		/// Generates a signature for the specified text.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/> instance,
		/// which is used to create the signature.</param>
		/// <param name="text">The text that requires a signature.</param>
		/// <param name="privateKey">A private key that is used
		/// to generate the signature.
		/// It should have a corresponding public key,
		/// which is used to verify the signature later.</param>
		/// <param name="encoding">The text encoding to use
		/// when converting the input text to bytes.
		/// By default UTF8 is used.</param>
		/// <returns>The signature that can accompany the text,
		/// and which is used later to verify that the text
		/// has not been modified.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments, apart from <c>encoding</c>,
		/// are null.</exception>
		public static async Task<string> CreateSignatureAsync(this ISigner signer, 
			string text, string privateKey, Encoding encoding = null)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(privateKey, nameof(privateKey));

			var signatureBytes = await CreateSignatureCoreAsync(signer, text, privateKey, encoding);
			if (encoding == null)
			{
				encoding = defaultEncoding;
			}
			string signatureText = encoding.GetString(signatureBytes);

			return signatureText;
		}

		/// <summary>
		/// A convenience method that generates
		/// a signature that is in Base64 format.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/> instance,
		/// which is used to create the signature.</param>
		/// <param name="text">The text that requires a signature.</param>
		/// <param name="privateKey">A private key that is used
		/// to generate the signature.
		/// It should have a corresponding public key,
		/// which is used to verify the signature later.</param>
		/// <param name="encoding">The text encoding to use
		/// when converting the input text to bytes.
		/// By default UTF8 is used.</param>
		/// <returns>The signature that can accompany the text,
		/// and which is used later to verify that the text
		/// has not been modified.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments, apart from <c>encoding</c>,
		/// are null.</exception>
		public static async Task<string> CreateBase64SignatureAsync(this ISigner signer, 
			string text, string privateKey, Encoding encoding = null)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(privateKey, nameof(privateKey));
			
			var signatureBytes = await CreateSignatureCoreAsync(signer, text, privateKey, encoding);
			string signatureText = Convert.ToBase64String(signatureBytes);

			return signatureText;
		}

		static async Task<byte[]> CreateSignatureCoreAsync(
			ISigner signer, string text, string privateKey, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = defaultEncoding;
			}

			var textBytes = encoding.GetBytes(text);
			byte[] signatureBytes;
			using (var ms = new MemoryStream(textBytes))
			{
				signatureBytes = await signer.CreateSignatureAsync(ms, privateKey);
			}

			return signatureBytes;
		}

		/// <summary>
		/// Verifies that the specified signature
		/// is a valid signature for the specified text,
		/// which indicates that the text has not been modified.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/> instance,
		/// which is used to create the signature.</param>
		/// <param name="text">The text that requires a signature.</param>
		/// <param name="signature">The signature of the text.</param>
		/// <param name="publicKey">A public key that is used
		/// to verify the signature.
		/// It should have a corresponding private key,
		/// which was used to generate the signature.</param>
		/// <param name="encoding">The text encoding to use
		/// when converting the input text to bytes.
		/// By default UTF8 is used.</param>
		/// <returns>The signature that can accompany the text,
		/// and which is used later to verify that the text
		/// has not been modified.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments, apart from <c>encoding</c>,
		/// are null.</exception>
		/// <returns><c>true</c> if the signature is valid, indicating
		/// that the text has not been modified; <c>false</c> otherwise.</returns>
		public static async Task<bool> VerifySignatureAsync(this ISigner signer, 
			string text, string signature, string publicKey, Encoding encoding = null)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(signature, nameof(signature));
			AssertArg.IsNotNull(publicKey, nameof(publicKey));

			if (encoding == null)
			{
				encoding = defaultEncoding;
			}

			var textBytes = encoding.GetBytes(text);
			var signatureBytes = encoding.GetBytes(signature);

			return await VerifySignatureCoreAsync(signer, textBytes, signatureBytes, publicKey);
		}

		/// <summary>
		/// Verifies that the specified signature in Base64
		/// is a valid signature for the specified text,
		/// which indicates that the text has not been modified.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/> instance,
		/// which is used to create the signature.</param>
		/// <param name="text">The text that requires a signature.</param>
		/// <param name="base64Signature">The signature of the text in Base64.</param>
		/// <param name="publicKey">A public key that is used
		/// to verify the signature.
		/// It should have a corresponding private key,
		/// which was used to generate the signature.</param>
		/// <param name="encoding">The text encoding to use
		/// when converting the input text to bytes.
		/// By default UTF8 is used.</param>
		/// <returns>The signature that can accompany the text,
		/// and which is used later to verify that the text
		/// has not been modified.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments, apart from <c>encoding</c>,
		/// are null.</exception>
		/// <returns><c>true</c> if the signature is valid, indicating
		/// that the text has not been modified; <c>false</c> otherwise.</returns>
		public static async Task<bool> VerifyBase64SignatureAsync(this ISigner signer, 
			string text, string base64Signature, string publicKey, Encoding encoding = null)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(base64Signature, nameof(base64Signature));
			AssertArg.IsNotNull(publicKey, nameof(publicKey));

			if (encoding == null)
			{
				encoding = defaultEncoding;
			}

			var textBytes = encoding.GetBytes(text);
			var signatureBytes = Convert.FromBase64String(base64Signature);

			return await VerifySignatureCoreAsync(signer, textBytes, signatureBytes, publicKey);
		}

		/// <summary>
		/// Verifies that the specified signature
		/// is a valid signature for the specified text,
		/// which indicates that the text has not been modified.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/> instance,
		/// which is used to create the signature.</param>
		/// <param name="text">The text that requires a signature.</param>
		/// <param name="signatureBytes">The signature of the text.</param>
		/// <param name="publicKey">A public key that is used
		/// to verify the signature.
		/// It should have a corresponding private key,
		/// which was used to generate the signature.</param>
		/// <param name="encoding">The text encoding to use
		/// when converting the input text to bytes.
		/// By default UTF8 is used.</param>
		/// <returns>The signature that can accompany the text,
		/// and which is used later to verify that the text
		/// has not been modified.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments, apart from <c>encoding</c>,
		/// are null.</exception>
		/// <returns><c>true</c> if the signature is valid, indicating
		/// that the text has not been modified; <c>false</c> otherwise.</returns>
		public static async Task<bool> VerifySignatureAsync(this ISigner signer, 
			string text, byte[] signatureBytes, string publicKey, Encoding encoding = null)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(signatureBytes, nameof(signatureBytes));
			AssertArg.IsNotNull(publicKey, nameof(publicKey));

			if (encoding == null)
			{
				encoding = defaultEncoding;
			}

			var textBytes = encoding.GetBytes(text);

			return await VerifySignatureCoreAsync(signer, textBytes, signatureBytes, publicKey);
		}

		/// <summary>
		/// Verifies that the specified signature
		/// is a valid signature for the specified text,
		/// which indicates that the text has not been modified.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/> instance,
		/// which is used to create the signature.</param>
		/// <param name="textBytes">The text that requires a signature.</param>
		/// <param name="signatureBytes">The signature of the text.</param>
		/// <param name="publicKey">A public key that is used
		/// to verify the signature.
		/// It should have a corresponding private key,
		/// which was used to generate the signature.</param>
		/// <returns>The signature that can accompany the text,
		/// and which is used later to verify that the text
		/// has not been modified.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments, apart from <c>encoding</c>,
		/// are null.</exception>
		/// <returns><c>true</c> if the signature is valid, indicating
		/// that the text has not been modified; <c>false</c> otherwise.</returns>
		public static async Task<bool> VerifySignatureAsync(this ISigner signer, 
			byte[] textBytes, byte[] signatureBytes, string publicKey)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(textBytes, nameof(textBytes));
			AssertArg.IsNotNull(signatureBytes, nameof(signatureBytes));
			AssertArg.IsNotNull(publicKey, nameof(publicKey));

			return await VerifySignatureCoreAsync(signer, textBytes, signatureBytes, publicKey);
		}

		static async Task<bool> VerifySignatureCoreAsync(
			ISigner signer, byte[] textBytes, byte[] signatureBytes, string publicKey)
		{
			using (var dataStream = new MemoryStream(textBytes))
			{
				bool result = await signer.VerifySignatureAsync(dataStream, signatureBytes, publicKey);
				return result;
			}
		}
	}
}