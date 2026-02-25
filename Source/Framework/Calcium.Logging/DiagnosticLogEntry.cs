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

		/* ISO-8601 UTC timestamp */
		[JsonPropertyName("ts")]
		public DateTime TimestampUtc { get; set; }

		[JsonPropertyName("lvl")]
		public int Level { get; set; }

		/* Component / class / subsystem */
		[JsonPropertyName("src")]
		public string? Source { get; set; }

		/* Human-readable message */
		[JsonPropertyName("msg")]
		public required string Message { get; set; }

		/* Optional exception summary */
		[JsonPropertyName("ex")]
		public string? Exception { get; set; }

		/* Arbitrary structured data */
		[JsonPropertyName("props")]
		public IDictionary<string, object>? Properties { get; set; }

		/* Process ID */
		[JsonPropertyName("pid")]
		public int ProcessId { get; set; }

		/* Managed thread ID */
		[JsonPropertyName("tid")]
		public int ThreadId { get; set; }

		/* Application name */
		[JsonPropertyName("app")]
		public required string Application { get; set; }

		/* Machine name */
		[JsonPropertyName("host")]
		public required string Host { get; set; }

		/* Optional correlation ID */
		[JsonPropertyName("cid")]
		public string? CorrelationId { get; set; }

		[JsonPropertyName("sid")]
		public string? SessionId { get; set; }

		[JsonPropertyName("seq")]
		public long Sequence { get; set; }
	}
}