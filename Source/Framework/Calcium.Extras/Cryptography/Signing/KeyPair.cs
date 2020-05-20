#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-08-02 19:37:29Z</CreationDate>
</File>
*/
#endregion
using System;

namespace Calcium.Cryptography
{
	/// <summary>
	/// Represents a public and private key pair
	/// used in cryptographic operations.
	/// </summary>
	public sealed class KeyPair
	{
		/// <summary>
		/// </summary>
		/// <param name="privateKey">
		/// A private key used to create cryptographic signatures.</param>
		/// <param name="publicKey">
		/// A public key used to verify cryptographic signatures.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		public KeyPair(string privateKey, string publicKey)
		{
			PrivateKey = AssertArg.IsNotNull(privateKey, nameof(privateKey));
			PublicKey = AssertArg.IsNotNull(publicKey, nameof(publicKey));
		}

		/// <summary>
		/// A public key used to verify cryptographic signatures.
		/// </summary>
		public string PublicKey { get; }

		/// <summary>
		/// A private key used to create cryptographic signatures.
		/// </summary>
		public string PrivateKey { get; }
	}
}
