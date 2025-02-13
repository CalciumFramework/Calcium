using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Calcium.Cryptography
{
	/// <summary>
	/// This class is used in conjunction with an <see cref="ISigner"/>
	/// to create signatures that are compatible with XML.
	/// Signatures can be appended to text and automatically
	/// located and verified at a later stage.
	/// </summary>
	public class SignatureEmbedment
	{
		/// <summary>
		/// This value finds its way into a CDATA section and is used
		/// to denote the beginning of a signature section. It needs
		/// to be unique in the off chance that another CDATA section
		/// exists in a document that might be misidentified as the signature.
		/// </summary>
		public string SignatureIndicator { get; set; } = "iWyrd55j10OmKa5KMhdpw";

		Regex GetSignatureRegex()
		{
			Regex regex = new Regex($@"(?<All><!\[CDATA\[{Regex.Escape(SignatureIndicator)}(?<Signature>.+)\]\]>)", RegexOptions.Multiline);
			return regex;
		}

		/// <summary>
		/// The default encoding to use when generating a signature.
		/// </summary>
		public Encoding Encoding { get; set; } = Encoding.UTF8;

		/// <summary>
		/// Creates a signature for the specified text
		/// and embeds it into the text.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/>
		/// instance that is used to generate the signature.</param>
		/// <param name="text">The text for which to generate a signature.</param>
		/// <param name="privateKey">The private key for signing the text.</param>
		/// <returns>The specified text with a signature appended to it.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		public async Task<string> EmbedSignatureAsync(
			ISigner signer, string text, string privateKey)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(privateKey, nameof(privateKey));

			string signatureText = await signer.CreateBase64SignatureAsync(text, privateKey, Encoding).ConfigureAwait(false);

			string result = $"{text}<![CDATA[{SignatureIndicator}{signatureText}]]>";
			return result;
		}

		/// <summary>
		/// Creates a signature for the specified text
		/// but does not embed it into the text.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/>
		/// instance that is used to generate the signature.</param>
		/// <param name="text">The text for which to generate a signature.</param>
		/// <param name="privateKey">The private key for signing the text.</param>
		/// <returns>The signature block, which can be placed alongside
		/// or within the text.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		public async Task<string> CreateButDoNotEmbedSignatureAsync(
			ISigner signer, string text, string privateKey)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(privateKey, nameof(privateKey));

			string signatureText = await signer.CreateBase64SignatureAsync(text, privateKey, Encoding).ConfigureAwait(false);

			string result = $"<![CDATA[{SignatureIndicator}{signatureText}]]>";
			return result;
		}

		/// <summary>
		/// Searches the specified text for a signature block.
		/// If found it is verified that is belongs to the text,
		/// which indicates that the text has not been modified.
		/// </summary>
		/// <param name="signer">The <see cref="ISigner"/>
		/// instance that is used to generate the signature.</param>
		/// <param name="text">The text which contains an embedded signature.</param>
		/// <param name="publicKey">The public key for verifying the signature.</param>
		/// <returns>A <see cref="TextVerificationResult"/> indicating
		/// whether the signature is valid or not. If the text does
		/// not contain an embedded signature, <c>SignatureVerified</c>
		/// is set to <c>false</c>. The <c>TextVerificationResult</c>
		/// object also contains the text without the signature.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if any of the arguments are null.</exception>
		public async Task<TextVerificationResult> VerifySignatureAsync(
			ISigner signer, string text, string publicKey)
		{
			AssertArg.IsNotNull(signer, nameof(signer));
			AssertArg.IsNotNull(text, nameof(text));
			AssertArg.IsNotNull(publicKey, nameof(publicKey));

			var regex = GetSignatureRegex();
			var match = regex.Match(text);
			if (!match.Success)
			{
				return new TextVerificationResult(false, null);
			}

			var signatureText = match.Groups["Signature"].Value;
			var allText = match.Groups["All"].Value;
			string withoutSignature = text.Replace(allText, string.Empty);

			bool result = await signer.VerifyBase64SignatureAsync(
							withoutSignature, signatureText, publicKey, Encoding);

			return new TextVerificationResult(result, withoutSignature);
		}
	}

	/// <summary>
	/// Represents the result of a cryptographic signature verification.
	/// </summary>
	public class TextVerificationResult
	{
		/// <summary>
		/// </summary>
		/// <param name="signatureVerified"></param>
		/// <param name="textWithoutSignature"></param>
		public TextVerificationResult(bool signatureVerified, string textWithoutSignature)
		{
			SignatureVerified = signatureVerified;
			TextWithoutSignature = textWithoutSignature;
		}

		/// <summary>
		/// If <c>true</c>, the signature is valid.
		/// If <c>false</c>, the signature is invalid,
		/// or no signature could be located.
		/// </summary>
		public bool SignatureVerified { get; }

		/// <summary>
		/// The text with the signature blocked removed.
		/// </summary>
		public string TextWithoutSignature { get; }
	}
}
