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

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace Calcium.Logging.Loggers
{
	public sealed class UdpLog : LogBase, IDisposable
	{
		static readonly JsonSerializerOptions serializerOptions = new()
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			WriteIndented = false
		};

		/* Keep UDP packets reasonably small to reduce fragmentation risk. */
		const int payloadBytesMax = 1200;

		readonly UdpClient udpClient;
		readonly IPEndPoint remoteEndPoint;

		readonly Channel<byte[]> channel;
		readonly CancellationTokenSource cancellationTokenSource;
		readonly Task senderTask;

		readonly string sessionId;

		long sequence;
		int droppedMessageCount;

		public int DroppedMessageCount => Volatile.Read(ref droppedMessageCount);

		public UdpLog(string host, int port, string? sessionId = null)
		{
			if (string.IsNullOrWhiteSpace(host))
			{
				throw new ArgumentException("Host must be provided.", nameof(host));
			}

			if (port <= 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535.");
			}

			this.sessionId = string.IsNullOrWhiteSpace(sessionId)
				? Guid.NewGuid().ToString("N")
				: sessionId;

			udpClient = new UdpClient();

			IPAddress address = ResolveIPAddress(host);
			remoteEndPoint = new IPEndPoint(address, port);

			channel = Channel.CreateBounded<byte[]>(
				new BoundedChannelOptions(512)
				{
					SingleReader = true,
					SingleWriter = false,
					FullMode = BoundedChannelFullMode.DropOldest
				});

			cancellationTokenSource = new CancellationTokenSource();
			senderTask = Task.Run(() => SenderLoopAsync(cancellationTokenSource.Token));
		}

		public override Task WriteAsync(LogLevel logLevel,
										string message,
										Exception? exception,
										IDictionary<string, object>? properties,
										string? memberName = null,
										string? filePath = null,
										int lineNumber = 0)
		{
			if (!IsLogEnabledForLevel(logLevel))
			{
				return Task.CompletedTask;
			}

			try
			{
				var entry = new DiagnosticLogEntry
				{
					TimestampUtc = DateTime.UtcNow,
					Level = (int)logLevel,
					Source = memberName,
					Message = message,
					Exception = exception?.ToString(),
					Properties = properties,
					ProcessId = Environment.ProcessId,
					ThreadId = Environment.CurrentManagedThreadId,
					Application = AppDomain.CurrentDomain.FriendlyName,
					Host = Environment.MachineName,
					SessionId = sessionId,
					Sequence = Interlocked.Increment(ref sequence)
				};

				byte[] payload = SerializeWithinPayloadLimit(entry);

				if (!channel.Writer.TryWrite(payload))
				{
					Interlocked.Increment(ref droppedMessageCount);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during UdpLog WriteAsync: " + ex);
			}

			return Task.CompletedTask;
		}

		async Task SenderLoopAsync(CancellationToken token)
		{
			try
			{
				while (await channel.Reader.WaitToReadAsync(token).ConfigureAwait(false))
				{
					while (channel.Reader.TryRead(out byte[]? payload))
					{
						try
						{
							await udpClient.SendAsync(payload, payload.Length, remoteEndPoint)
								.ConfigureAwait(false);
						}
						catch (Exception ex)
						{
							await Console.Error.WriteLineAsync("Error during UdpLog send: " + ex);
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
				/* Normal shutdown. */
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync("Error during UdpLog sender loop: " + ex);
			}
		}

		static byte[] SerializeToUtf8(DiagnosticLogEntry entry)
		{
			string json = JsonSerializer.Serialize(entry, serializerOptions);
			return Encoding.UTF8.GetBytes(json);
		}

		byte[] SerializeWithinPayloadLimit(DiagnosticLogEntry entry)
		{
			byte[] payload = SerializeToUtf8(entry);

			if (payload.Length <= payloadBytesMax)
			{
				return payload;
			}

			/* First: drop properties */
			if (entry.Properties is not null)
			{
				entry.Properties = null;
				payload = SerializeToUtf8(entry);

				if (payload.Length <= payloadBytesMax)
				{
					return payload;
				}
			}

			/* Second: shrink exception (usually the biggest) */
			payload = ShrinkFieldUntilFits(
				entry,
				getValue: static e => e.Exception,
				setValue: static (e, v) => e.Exception = v);

			if (payload.Length <= payloadBytesMax)
			{
				return payload;
			}

			/* Third: shrink message */
			payload = ShrinkFieldUntilFits(
				entry,
				getValue: static e => e.Message,
				setValue: static (e, v) => e.Message = v ?? string.Empty);

			if (payload.Length <= payloadBytesMax)
			{
				return payload;
			}

			/* Last resort: minimal valid payload */
			entry.Message = string.Empty;
			entry.Exception = null;
			entry.Properties = null;

			return SerializeToUtf8(entry);
		}

		byte[] ShrinkFieldUntilFits(DiagnosticLogEntry entry,
									Func<DiagnosticLogEntry, string?> getValue,
									Action<DiagnosticLogEntry, string?> setValue)
		{
			string? value = getValue(entry);
			if (string.IsNullOrEmpty(value))
			{
				return SerializeToUtf8(entry);
			}

			int length = value.Length;

			while (length > 0)
			{
				length /= 2;

				string? shortened = length == 0 ? null : value.Substring(0, length);
				setValue(entry, shortened);

				byte[] payload = SerializeToUtf8(entry);
				if (payload.Length <= payloadBytesMax)
				{
					return payload;
				}
			}

			return SerializeToUtf8(entry);
		}

		static IPAddress ResolveIPAddress(string? host)
		{
			if (string.IsNullOrWhiteSpace(host))
			{
				return IPAddress.Loopback;
			}

			/* Fast path: literal IP address (IPv4 or IPv6) */
			if (IPAddress.TryParse(host, out IPAddress? address))
			{
				return address;
			}

			/* Avoid DNS for localhost */
			if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
			{
				return IPAddress.Loopback;
			}

			try
			{
				IPAddress[] addresses = Dns.GetHostAddresses(host);

				if (addresses.Length == 0)
				{
					throw new InvalidOperationException($"Unable to resolve host '{host}'.");
				}

				return addresses[0];
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Failed to resolve host '{host}'.", ex);
			}
		}

		public void Dispose()
		{
			try
			{
				cancellationTokenSource.Cancel();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during UdpLog disposal (Cancel): " + ex);
			}

			try
			{
				channel.Writer.TryComplete();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during UdpLog disposal (TryComplete): " + ex);
			}

			try
			{
				senderTask.Wait(TimeSpan.FromSeconds(1));
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during UdpLog disposal (Wait): " + ex);
			}

			try
			{
				udpClient.Dispose();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during UdpLog disposal (UdpClient.Dispose): " + ex);
			}

			try
			{
				cancellationTokenSource.Dispose();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during UdpLog disposal (CancellationTokenSource.Dispose): " + ex);
			}
		}
	}
}