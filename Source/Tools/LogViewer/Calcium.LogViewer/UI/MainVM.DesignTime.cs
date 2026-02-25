using System;
using System.Collections.Generic;

using Avalonia.Threading;

using Calcium.Logging;

using Orpius.Shell;

namespace Calcium.LogViewer.UI
{
	public partial class MainVM
	{
		static readonly Random random = new();

		DispatcherTimer? designTimeTimer;
		long designTimeSequence;

		void PopulateDesignTimeData()
		{
			if (!UIEnvironment.DesignTime)
			{
				return;
			}

			StatusText = "Design-time: generating sample log entries.";

			designTimeTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(700)
			};

			void HandleTimerTickLocal(object? _, EventArgs __)
			{
				LogLevel[] levels = new[] { LogLevel.Trace, LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal };

				LogLevel level = levels[random.Next(levels.Length)];

				string[] sources = { "BusyIndicator3", "ChatVM", "UdpLog", "LogViewerSender", "InferencingService", "PreviewerProcess" };

				string source = sources[random.Next(sources.Length)];

				string[] messages = { "Connected.", "Sending message to viewer.", "Received chat response chunk.", "Active tickers changed.", "HTTP call completed.", "Background task finished.", "Unexpected response shape." };

				string message = messages[random.Next(messages.Length)];

				string? exception = null;

				if (level == LogLevel.Error || level == LogLevel.Fatal)
				{
					string[] exceptions = { "System.TimeoutException: The operation timed out.", "System.InvalidOperationException: Sequence contains no elements.", "Grpc.Core.RpcException: Status(StatusCode=\"Unavailable\", Detail=\"failed to connect\")" };

					exception = exceptions[random.Next(exceptions.Length)];
				}

				Dictionary<string, object> properties = new() { { "designTime", true }, { "rnd", random.Next(1, 10000) } };

				DiagnosticLogEntry entry = new()
				{
					TimestampUtc  = DateTime.UtcNow,
					Level         = (int)level,
					Source        = source,
					Message       = message + "  (sample)",
					Exception     = exception,
					Properties    = properties,
					ProcessId     = 9999,
					ThreadId      = Environment.CurrentManagedThreadId,
					Application   = "LogViewer (Designer)",
					Host          = Environment.MachineName,
					CorrelationId = null,
					SessionId     = "design-session",
					Sequence      = ++designTimeSequence
				};

				LogEntries.Add(entry);

				int limit = 2000;

				try
				{
					limit = MaxEntries;
				}
				catch
				{
					/* If you do not have MaxEntries, keep default. */
				}

				while (LogEntries.Count > limit && LogEntries.Count > 0)
				{
					LogEntries.RemoveAt(0);
				}
			}

			designTimeTimer.Tick += HandleTimerTickLocal;

			designTimeTimer.Start();
		}
	}
}