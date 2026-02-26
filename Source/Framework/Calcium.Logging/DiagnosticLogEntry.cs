#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-02-25 22:34:50Z</CreationDate>
</File>
*/
#endregion

using System.Text.Json.Serialization;

namespace Calcium.Logging
{
	public sealed class DiagnosticLogEntry
	{
		[JsonPropertyName("v")]
		public int Version { get; set; } = 1;

		/// <summary>
		/// ISO-8601 UTC timestamp
		/// </summary>
		[JsonPropertyName("ts")]
		public DateTime TimestampUtc { get; set; }

		[JsonPropertyName("lvl")]
		public int Level { get; set; }

		/// <summary>
		/// Component / class / subsystem
		/// </summary>
		[JsonPropertyName("src")]
		public string? Source { get; set; }

		/// <summary>
		/// Human-readable message
		/// </summary>
		[JsonPropertyName("msg")]
		public required string Message { get; set; }

		/// <summary>
		/// Optional exception summary
		/// </summary>
		[JsonPropertyName("ex")]
		public string? Exception { get; set; }

		/// <summary>
		/// Arbitrary structured data
		/// </summary>
		[JsonPropertyName("props")]
		public IDictionary<string, object>? Properties { get; set; }

		/// <summary>
		/// Process ID
		/// </summary>
		[JsonPropertyName("pid")]
		public int ProcessId { get; set; }

		/// <summary>
		/// Managed thread ID
		/// </summary>
		[JsonPropertyName("tid")]
		public int ThreadId { get; set; }

		/// <summary>
		/// Application name
		/// </summary>
		[JsonPropertyName("app")]
		public required string Application { get; set; }

		/// <summary>
		/// Machine name
		/// </summary>
		[JsonPropertyName("host")]
		public required string Host { get; set; }

		/// <summary>
		/// Optional correlation ID
		/// </summary>
		[JsonPropertyName("cid")]
		public string? CorrelationId { get; set; }

		[JsonPropertyName("sid")]
		public string? SessionId { get; set; }

		[JsonPropertyName("seq")]
		public long Sequence { get; set; }
	}
}