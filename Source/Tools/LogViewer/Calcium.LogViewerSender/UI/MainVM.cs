using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Calcium.ComponentModel;
using Calcium.Logging;
using Calcium.Logging.Loggers;
using Calcium.UIModel.Input;

using Material.Icons;

using Orpius.Shell;

namespace Calcium.LogViewerSender.UI
{
	public class MainVM : VMBase
	{
		int port = 17888;

		public int Port
		{
			get => port;
			set
			{
				if (Set(ref port, value) == AssignmentResult.Success)
				{
					RecreateLogger();
				}
			}
		}

		string host = "127.0.0.1";

		public string Host
		{
			get => host;
			set
			{
				if (Set(ref host, value) == AssignmentResult.Success)
				{
					RecreateLogger();
				}
			}
		}

		string? messageText;

		public string? MessageText
		{
			get => messageText;
			set
			{
				if (Set(ref messageText, value) == AssignmentResult.Success)
				{
					sendMessageCommand?.RaiseCanExecuteChanged();
				}
			}
		}

		string? exceptionString;

		public string? ExceptionString
		{
			get => exceptionString;
			set => Set(ref exceptionString, value);
		}

		LogLevel logLevel = LogLevel.Info;

		public LogLevel LogLevel
		{
			get => logLevel;
			set => Set(ref logLevel, value);
		}

		public IList<LogLevel> LogLevels { get; } = Enum.GetValues<LogLevel>();

		UdpLog? udpLog;

		AsyncUICommand? sendMessageCommand;

		public AsyncUICommand SendMessageCommand
			=> sendMessageCommand ??= new AsyncUICommand(SendMessageAsync, CanSendMessageAsync)
			{
				IconCharacter = MaterialIconKind.Send.ToString(),
				TextFunc = _ => Task.FromResult("Send")
			};

		async Task<bool> CanSendMessageAsync(object arg)
		{
			try
			{
				if (Busy)
				{
					return false;
				}

				return !string.IsNullOrWhiteSpace(MessageText);
			}
			catch (Exception ex)
			{
				return await Task.FromException<bool>(ex);
			}
		}

		async Task SendMessageAsync(object obj)
		{
			if (UIEnvironment.DesignTime)
			{
				return;
			}

			try
			{
				using var busyScope = BusyTracker.EnterBusy();

				string? text = MessageText?.Trim();

				if (string.IsNullOrWhiteSpace(text))
				{
					return;
				}

				UdpLog log = GetOrCreateLogger();

				var properties = new Dictionary<string, object>
				{
					{ "sender", "LogViewerSender" },
					{ "port", Port },
					{ "logLevel", (int)LogLevel },
					{ "timestampLocal", DateTime.Now.ToString("O") },
				};

				Exception? exception = null;

				if (!string.IsNullOrWhiteSpace(ExceptionString))
				{
					exception = new Exception(ExceptionString!.Trim());
				}

				/* Use the Calcium LogBase entry points so you test the same paths your app uses. */
				WriteToLog(log, LogLevel, text, exception, properties);

				/* Small delay so you can hammer the button and still see output order clearly. */
				await Task.Delay(10);

				/* Reset only the text fields (keep port/host/level selections). */
				MessageText     = null;
				ExceptionString = null;
				AppendFeedback("Message sent");
			}
			catch (Exception ex)
			{
				AppendFeedback("Send failed: " + ex.Message);
			}
			finally
			{
				sendMessageCommand?.RaiseCanExecuteChanged();
			}
		}

		void WriteToLog(ILog log,
						LogLevel level,
						string message,
						Exception? exception,
						IDictionary<string, object> properties)
		{
			log.WriteAsync(level, message, exception, properties);
		}

		UdpLog GetOrCreateLogger()
		{
			if (udpLog != null)
			{
				return udpLog;
			}

			udpLog = new UdpLog(Port)
			{
				/* Sender tool should be chatty. Viewer can filter. */
				MinimumLogLevel = LogLevel.All
			};

			return udpLog;
		}

		void RecreateLogger()
		{
			try
			{
				udpLog?.Dispose();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during LogViewerSender logger disposal: " + ex);
			}

			udpLog = null;
		}

		string? feedbackText;

		public string? FeedbackText
		{
			get => feedbackText;
			set => Set(ref feedbackText, value);
		}

		void AppendFeedback(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			string line = $"[{DateTime.Now:HH:mm:ss}] {message}";

			if (string.IsNullOrWhiteSpace(FeedbackText))
			{
				FeedbackText = line;
				return;
			}

			/* Keep it to a reasonable size. */
			const int maxCharacters = 8000;

			string updated = FeedbackText + Environment.NewLine + line;

			if (updated.Length > maxCharacters)
			{
				updated = updated.Substring(updated.Length - maxCharacters);
			}

			FeedbackText = updated;
		}
	}
}