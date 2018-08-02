#if NETSTANDARD2_0
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-08-02 19:37:54Z</CreationDate>
</File>
*/
#endregion
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Codon.Cryptography.Implementation
{
	public class RsaSigner : ISigner
	{
		public Task<byte[]> CreateSignatureAsync(
			Stream inputStream, string rsaPrivateKeyXml)
		{
			AssertArg.IsNotNull(inputStream, nameof(inputStream));
			AssertArg.IsNotNull(rsaPrivateKeyXml, nameof(rsaPrivateKeyXml));

			using (var provider = RSA.Create())
			{
				provider.KeySize = KeySizeBits;
				provider.FromXmlString(rsaPrivateKeyXml);

				byte[] signatureBytes = provider.SignData(
											inputStream, 
											HashAlgorithmName.SHA256, 
											RSASignaturePadding.Pkcs1);
				return Task.FromResult(signatureBytes);
			}
		}

		public Task<bool> VerifySignatureAsync(
			Stream dataStream, byte[] signatureBytes, string rsaPublicKeyXml)
		{
			using (var provider = RSA.Create())
			{
				provider.KeySize = KeySizeBits;
				provider.FromXmlString(rsaPublicKeyXml);

				bool result = provider.VerifyData(
								dataStream, signatureBytes, HashAlgorithmName, Padding);
				return Task.FromResult(result);
			}
		}

		public HashAlgorithmName HashAlgorithmName { get; set; } = HashAlgorithmName.SHA256;

		public RSASignaturePadding Padding { get; set; } = RSASignaturePadding.Pkcs1;

		public int KeySizeBits { get; set; } = 1024;

		public KeyPair GenerateKeyPair()
		{
			using (var provider = RSA.Create())
			{
				provider.KeySize = KeySizeBits;

				string privateKey = provider.ToXmlString(true);
				string publicKey = provider.ToXmlString(false);

				return new KeyPair(privateKey, publicKey);
			}
		}
	}
}
#endif