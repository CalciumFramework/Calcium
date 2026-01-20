#nullable enable
using System;
using System.Collections.Generic;

namespace Calcium.Networking.UrlParsing
{
	/// <summary>
	/// Creates <see cref="Uri"/> instances using <see cref="Uri.TryCreate(string, UriKind, out Uri?)"/>.
	/// When creation fails, returns diagnostic hints that can be fed back to an LLM or logs.
	/// </summary>
	public class UriCreator
	{
		/// <summary>
		/// Attempts to create an absolute <see cref="Uri"/> from the supplied string.
		/// </summary>
		/// <param name="url">
		/// The URL string to parse. This must be a full absolute URL (for example,
		/// <c>https://example.com/path</c>). The value must not be <c>null</c>.
		/// </param>
		/// <param name="result">
		/// When this method returns, contains a <see cref="UriCreationResult"/>.
		/// If creation succeeds, <see cref="UriCreationResult.ParsedUri"/> will be set and
		/// <see cref="UriCreationResult.Diagnostics"/> will be empty.
		/// If creation fails, <see cref="UriCreationResult.ParsedUri"/> will be <c>null</c> and
		/// <see cref="UriCreationResult.Diagnostics"/> will contain human-readable hints describing
		/// why parsing may have failed.
		/// </param>
		/// <returns>
		/// <c>true</c> if <see cref="Uri.TryCreate(string, UriKind, out Uri?)"/> successfully created
		/// an absolute <see cref="Uri"/>; otherwise <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="url"/> is <c>null</c>.
		/// </exception>
		public bool TryCreate(string url, out UriCreationResult result)
		{
			AssertArg.IsNotNull(url, nameof(url));

			if (string.IsNullOrWhiteSpace(url))
			{
				var diagnostics = new List<UriCreationDiagnostic>
				{
					new UriCreationDiagnostic(
						UriCreationDiagnosticCode.EmptyOrWhitespace,
						"URL is empty or whitespace."
						+ " Provide a full absolute URL, e.g. https://example.com/path",
						index: null)
				};

				result = new UriCreationResult(parsedUri: null, diagnostics: diagnostics);
				return false;
			}

			/* Primary authority: if .NET can create it, we accept it and return no diagnostics. */
			if (Uri.TryCreate(url, UriKind.Absolute, out Uri? parsedUri))
			{
				result = new UriCreationResult(
					parsedUri: parsedUri,
					diagnostics: Array.Empty<UriCreationDiagnostic>());

				return true;
			}

			List<UriCreationDiagnostic> diagnosticsOnFailure = Diagnose(url);

			if (diagnosticsOnFailure.Count == 0)
			{
				diagnosticsOnFailure.Add(new UriCreationDiagnostic(
					UriCreationDiagnosticCode.DotNetParsingFailed,
					"URI failed .NET parsing. Check for invalid characters,"
					+ " truncated text, missing scheme, or missing query encoding.",
					index: null));
			}
			else
			{
				diagnosticsOnFailure.Add(new UriCreationDiagnostic(
					UriCreationDiagnosticCode.DotNetParsingFailed,
					"URI failed .NET parsing.",
					index: null));
			}

			result = new UriCreationResult(parsedUri: null, diagnostics: diagnosticsOnFailure);
			return false;
		}

		static List<UriCreationDiagnostic> Diagnose(string url)
		{
			List<UriCreationDiagnostic> diagnostics = new();

			/* Missing scheme is extremely common with copied/pasted URLs. */
			if (!url.Contains("://"))
			{
				diagnostics.Add(new UriCreationDiagnostic(
					UriCreationDiagnosticCode.MissingScheme,
					"Missing scheme. Use an absolute URL"
					+ " starting with https:// (or http://).",
					index: null));
			}

			AddWhitespaceDiagnostics(url, diagnostics);
			AddPercentEncodingDiagnostics(url, diagnostics);
			AddQuoteDiagnostics(url, diagnostics);
			AddTruncationDiagnostics(url, diagnostics);

			return diagnostics;
		}

		static void AddWhitespaceDiagnostics(string url, List<UriCreationDiagnostic> diagnostics)
		{
			/* Whitespace is a very common LLM/user mistake and frequently causes parsing failures. */
			for (int i = 0; i < url.Length; i++)
			{
				if (char.IsWhiteSpace(url[i]))
				{
					diagnostics.Add(new UriCreationDiagnostic(
						UriCreationDiagnosticCode.UnescapedWhitespace,
						$"Contains whitespace at index {i}."
						+ $" Encode spaces as %20 (or '+' inside query values).",
						index: i));
				}
			}
		}

		static void AddPercentEncodingDiagnostics(string url, List<UriCreationDiagnostic> diagnostics)
		{
			/* Broken percent encoding is a common reason for UriFormatException in prompts. */
			for (int i = 0; i < url.Length; i++)
			{
				if (url[i] != '%')
				{
					continue;
				}

				/* '%' at end */
				if (i == url.Length - 1)
				{
					diagnostics.Add(new UriCreationDiagnostic(
						UriCreationDiagnosticCode.BadPercentEncoding,
						$"Bad percent-encoding at index {i} (%):"
						+ $" percent sign must be followed by two hex digits.",
						index: i));
					continue;
				}

				/* '%' with one trailing char */
				if (i == url.Length - 2)
				{
					string token = url.Substring(i, 2);
					diagnostics.Add(new UriCreationDiagnostic(
						UriCreationDiagnosticCode.BadPercentEncoding,
						$"Bad percent-encoding at index {i} ({token}):"
						+ $" percent sign must be followed by two hex digits.",
						index: i));
					continue;
				}

				char a = url[i + 1];
				char b = url[i + 2];

				/* Legacy %uXXXX style (not valid in standard URIs) */
				if ((a == 'u' || a == 'U') && i + 5 < url.Length)
				{
					char h1 = url[i + 2];
					char h2 = url[i + 3];
					char h3 = url[i + 4];
					char h4 = url[i + 5];

					if (IsHex(h1) && IsHex(h2) && IsHex(h3) && IsHex(h4))
					{
						string token = url.Substring(i, 6);
						diagnostics.Add(new UriCreationDiagnostic(
							UriCreationDiagnosticCode.BadPercentEncoding,
							$"Bad percent-encoding at index {i} ({token}):"
							+ $" legacy %uXXXX encoding detected; use standard percent-encoding (%XX) instead.",
							index: i));
						continue;
					}
				}

				if (!IsHex(a) || !IsHex(b))
				{
					string token = url.Substring(i, 3);
					diagnostics.Add(new UriCreationDiagnostic(
						UriCreationDiagnosticCode.BadPercentEncoding,
						$"Bad percent-encoding at index {i} ({token}):"
						+ $" expected two hex digits (0-9, A-F).",
						index: i));
				}
			}
		}

		static void AddQuoteDiagnostics(string url, List<UriCreationDiagnostic> diagnostics)
		{
			/* Unescaped quotes often show up when users/LLMs paste GitHub search queries. */
			for (int i = 0; i < url.Length; i++)
			{
				if (url[i] == '"')
				{
					diagnostics.Add(new UriCreationDiagnostic(
						UriCreationDiagnosticCode.UnescapedQuote,
						$"Contains an unescaped double-quote at index {i}."
						+ $" Encode quotes as %22.",
						index: i));
				}
			}
		}

		static void AddTruncationDiagnostics(string url, List<UriCreationDiagnostic> diagnostics)
		{
			/* Truncation heuristics: catching common patterns where the URL is cut off. */
			int length = url.Length;

			if (length == 0)
			{
				return;
			}

			int truncationIndex = -1;

			/* Trailing '%' strongly suggests truncation. */
			if (url[length - 1] == '%')
			{
				truncationIndex = length - 1;
			}
			/* Trailing '%X' (single hex digit) commonly comes from cut-off percent encoding. */
			else if (length >= 2 && url[length - 2] == '%' && IsHex(url[length - 1]))
			{
				truncationIndex = length - 2;
			}

			if (truncationIndex < 0)
			{
				return;
			}

			/* Avoid adding a truncation hint if we already recorded
			   a bad percent-encoding at the same index. */
			bool alreadyHasBadPercentAtSameIndex = false;

			for (int i = 0; i < diagnostics.Count; i++)
			{
				UriCreationDiagnostic d = diagnostics[i];
				if (d.Code == UriCreationDiagnosticCode.BadPercentEncoding && d.Index == truncationIndex)
				{
					alreadyHasBadPercentAtSameIndex = true;
					break;
				}
			}

			if (!alreadyHasBadPercentAtSameIndex)
			{
				diagnostics.Add(new UriCreationDiagnostic(
					UriCreationDiagnosticCode.PossibleTruncation,
					$"URL appears truncated near index {truncationIndex}. Ensure it was copied fully.",
					index: truncationIndex));
			}
		}

		static bool IsHex(char c)
		{
			return (c    >= '0' && c <= '9') 
				   || (c >= 'a' && c <= 'f') 
				   || (c >= 'A' && c <= 'F');
		}
	}

	public sealed class UriCreationResult
	{
		public Uri? ParsedUri { get; }

		public IReadOnlyList<UriCreationDiagnostic> Diagnostics { get; }

		public UriCreationResult(Uri? parsedUri, IReadOnlyList<UriCreationDiagnostic>? diagnostics)
		{
			ParsedUri = parsedUri;
			Diagnostics = diagnostics ?? Array.Empty<UriCreationDiagnostic>();
		}
	}

	public sealed class UriCreationDiagnostic
	{
		public UriCreationDiagnosticCode Code { get; }

		public string Message { get; }

		public int? Index { get; }

		public UriCreationDiagnostic(UriCreationDiagnosticCode code, string message, int? index)
		{
			Code = code;
			Message = message;
			Index = index;
		}
	}

	public enum UriCreationDiagnosticCode
	{
		EmptyOrWhitespace,
		MissingScheme,
		UnescapedWhitespace,
		BadPercentEncoding,
		UnescapedQuote,
		PossibleTruncation,
		DotNetParsingFailed
	}
}
