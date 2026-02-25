using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Threading;

using Calcium.Logging;
using Calcium.UIModel.Input;

using Orpius.Shell;

namespace Calcium.LogViewer.UI
{
	public sealed partial class MainVM : VMBase, IDisposable
	{
		public MainVM()
		{
			if (UIEnvironment.DesignTime)
			{
				PopulateDesignTimeData();
			}
			else
			{
				StartCommand.Execute(null);
			}
		}

		static readonly JsonSerializerOptions serializerOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		const int maxEntriesDefault = 2000;

		int port = 17888;

		public int Port
		{
			get => port;
			set => Set(ref port, value);
		}

		int maxEntries = maxEntriesDefault;

		public int MaxEntries
		{
			get => maxEntries;
			set => Set(ref maxEntries, value);
		}

		bool isListening;

		public bool IsListening
		{
			get => isListening;
			private set => Set(ref isListening, value);
		}

		string? statusText;

		public string? StatusText
		{
			get => statusText;
			private set => Set(ref statusText, value);
		}

		public ObservableCollection<DiagnosticLogEntry> LogEntries { get; } = new();

		CancellationTokenSource? listenerTokenSource;
		Task? listenerTask;

		AsyncUICommand? clearCommand;
		public AsyncUICommand ClearCommand
			=> clearCommand ??= new AsyncUICommand(ClearAsync, CanClearAsync)
			{
				TextFunc = _ => Task.FromResult("Clear")
			};

		AsyncUICommand? startCommand;
		public AsyncUICommand StartCommand
			=> startCommand ??= new AsyncUICommand(StartAsync, CanStartAsync)
			{
				TextFunc = _ => Task.FromResult("Start")
			};

		AsyncUICommand? stopCommand;
		public AsyncUICommand StopCommand
			=> stopCommand ??= new AsyncUICommand(StopAsync, CanStopAsync)
			{
				TextFunc = _ => Task.FromResult("Stop")
			};

		Task<bool> CanClearAsync(object arg)
		{
			return Task.FromResult(!Busy && LogEntries.Count > 0);
		}

		Task<bool> CanStartAsync(object arg)
		{
			return Task.FromResult(!Busy && !IsListening && Port > 0 && Port <= 65535);
		}

		Task<bool> CanStopAsync(object arg)
		{
			return Task.FromResult(!Busy && IsListening);
		}

		async Task ClearAsync(object obj)
		{
			if (UIEnvironment.DesignTime)
			{
				return;
			}

			using var busyScope = BusyTracker.EnterBusy();

			await Dispatcher.UIThread.InvokeAsync(() =>
			{
				LogEntries.Clear();
				StatusText = "Cleared.";
				RaiseCommandStates();
			});
		}

		async Task StartAsync(object obj)
		{
			if (UIEnvironment.DesignTime)
			{
				return;
			}

			if (IsListening)
			{
				return;
			}

			using var busyScope = BusyTracker.EnterBusy();

			try
			{
				listenerTokenSource = new CancellationTokenSource();

				IsListening = true;
				StatusText = $"Listening on UDP 127.0.0.1:{Port}";

				RaiseCommandStates();

				listenerTask = Task.Run(() => ListenLoopAsync(Port, listenerTokenSource.Token));
			}
			catch (Exception ex)
			{
				StatusText = "Failed to start listener: " + ex.Message;
				IsListening = false;
				RaiseCommandStates();
				throw;
			}

			await Task.CompletedTask;
		}

		async Task StopAsync(object obj)
		{
			if (UIEnvironment.DesignTime)
			{
				return;
			}

			if (!IsListening)
			{
				return;
			}

			using var busyScope = BusyTracker.EnterBusy();

			try
			{
				listenerTokenSource?.Cancel();

				if (listenerTask != null)
				{
					await listenerTask;
				}

				StatusText = "Stopped.";
			}
			catch (Exception ex)
			{
				StatusText = "Error stopping listener: " + ex.Message;
			}
			finally
			{
				listenerTokenSource?.Dispose();
				listenerTokenSource = null;
				listenerTask = null;
				IsListening = false;
				RaiseCommandStates();
			}
		}

		async Task ListenLoopAsync(int listenPort, CancellationToken token)
		{
			try
			{
				using var udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, listenPort));

				while (!token.IsCancellationRequested)
				{
					UdpReceiveResult result;

					try
					{
						result = await udpClient.ReceiveAsync(token);
					}
					catch (OperationCanceledException)
					{
						break;
					}

					string json;

					try
					{
						json = Encoding.UTF8.GetString(result.Buffer);
					}
					catch (Exception ex)
					{
						await PostStatusAsync("Invalid UTF-8 payload: " + ex.Message);
						continue;
					}

					DiagnosticLogEntry? entry;

					try
					{
						entry = JsonSerializer.Deserialize<DiagnosticLogEntry>(json, serializerOptions);
					}
					catch (Exception ex)
					{
						await PostStatusAsync("Invalid JSON payload: " + ex.Message);
						continue;
					}

					if (entry == null)
					{
						continue;
					}

					void AddLocal()
					{
						LogEntries.Add(entry);
						//SelectedEntry = entry;

						/* Trim old entries. */
						while (LogEntries.Count > MaxEntries && LogEntries.Count > 0)
						{
							LogEntries.RemoveAt(0);
						}

						RaiseCommandStates();
					}

					await Dispatcher.UIThread.InvokeAsync(AddLocal, DispatcherPriority.Background);
				}
			}
			catch (Exception ex)
			{
				await PostStatusAsync("Listener error: " + ex);
			}
		}

		Task PostStatusAsync(string message)
		{
			return Dispatcher.UIThread.InvokeAsync(() =>
			{
				StatusText = message;
			}).GetTask();
		}

		void RaiseCommandStates()
		{
			clearCommand?.RaiseCanExecuteChanged();
			startCommand?.RaiseCanExecuteChanged();
			stopCommand?.RaiseCanExecuteChanged();
		}

		public void Dispose()
		{
			try
			{
				listenerTokenSource?.Cancel();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during LogViewer disposal (Cancel): " + ex);
			}

			try
			{
				listenerTokenSource?.Dispose();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error during LogViewer disposal (Dispose): " + ex);
			}
		}

		DiagnosticLogEntry? selectedEntry;

		public DiagnosticLogEntry? SelectedEntry
		{
			get => selectedEntry;
			set => Set(ref selectedEntry, value);
		}

		bool autoScroll = true;

		public bool AutoScroll
		{
			get => autoScroll;
			set => Set(ref autoScroll, value);
		}

		GridLength messageColumnWidth = new GridLength(420);

		public GridLength MessageColumnWidth
		{
			get => messageColumnWidth;
			set => Set(ref messageColumnWidth, value);
		}

		GridLength exceptionColumnWidth = new GridLength(320);

		public GridLength ExceptionColumnWidth
		{
			get => exceptionColumnWidth;
			set => Set(ref exceptionColumnWidth, value);
		}
	}
}