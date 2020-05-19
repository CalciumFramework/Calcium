#if NETSTANDARD
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-19 12:02:41Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Codon.Logging.Loggers
{
	/// <summary>
	/// An implementation of <see cref="ILog"/>
	/// that writes messages to a file located in isolated storage.
	/// Currently incompatible with WPF.
	/// </summary>
	public class LocalStorageLog : LogBase
	{
		bool writeToConsoleAsWell = true;

		/// <summary>
		/// Gets or sets a value indicating that log messages
		/// should also be written to the console.
		/// </summary>
		public bool WriteToConsoleAsWell
		{
			get => writeToConsoleAsWell;
			set => writeToConsoleAsWell = value;
		}

		string logFileName = "log.txt";

		/// <summary>
		/// The name of the file in isolated storage.
		/// Default is "log.txt".
		/// </summary>
		public string LogFileName
		{
			get => logFileName;
			set => logFileName = AssertArg.IsNotNullOrWhiteSpace(value, nameof(value));
		}

		long logFileSizeMaxKB = 1024; /* 1 MB */

		/// <summary>
		/// This value indicates at what size to clear the log and begin a new log file.
		/// Default is 1024 x 1024 (1 MB).
		/// </summary>
		public long LogFileSizeMaxKB
		{
			get => logFileSizeMaxKB;
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Value must be greater than -1.");
				}

				logFileSizeMaxKB = value;
			}
		}

		readonly object fileLock = new object();

		public override async Task WriteAsync(LogLevel logLevel,
			string message,
			Exception exception,
			IDictionary<string, object> properties,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			if (!logLevel.IsGreaterThanOrEqualTo(MinimumLogLevel))
			{
				return;
			}

			string logLine = string.Format("<Entry><Time>{0}</Time><LogLevel>{1:G}</LogLevel><Message>{2}</Message><Member>{3}</Member><File>{4}</File><Line>{5}</Line><Exception>{6}</Exception></Entry>",
								DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), logLevel,
								CleanForXml(memberName),
								CleanForXml(message),
								CleanForXml(filePath),
								lineNumber.ToString(CultureInfo.InvariantCulture),
								CleanForXml(exception));


			await Task.Run(delegate
			{
				if (writeToConsoleAsWell)
				{
#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
					DiagnosticsDebug.WriteLine("LocalStorageLog:" + logLine);
#else
					Debugger.Log(3, "LocalStorageLog", logLine);
#endif
				}

				lock (fileLock)
				{
					WriteToFile(logLine);
				}
			});
		}

		string CleanForXml(object text)
		{
			if (text == null)
			{
				return string.Empty;
			}

			string result = WebUtility.HtmlEncode(text.ToString());

			return result;
		}

		IsolatedStorageFile GetStorageFile()
		{
#if WPF
			return IsolatedStorageSettingsWpf.GetStorageFile();
#else
			return IsolatedStorageFile.GetUserStoreForApplication();
#endif
		}

		/// <summary>
		/// Writes a line to the current log file.
		/// </summary>
		/// <param name="message"></param>
		void WriteToFile(string message)
		{
			try
			{
				using (IsolatedStorageFile storageFile = GetStorageFile())
				{
					var fileMode = FileMode.Append;
					bool addXmlDeclaration = false;

					using (IsolatedStorageFileStream stream
						= storageFile.OpenFile(logFileName, FileMode.OpenOrCreate))
					{
						long logFileSizeMaxBytes = logFileSizeMaxKB * 1024;
						if (stream.Length >= logFileSizeMaxBytes)
						{
							fileMode = FileMode.Truncate;
							addXmlDeclaration = true;
						}

						if (stream.Length < 5)
						{
							addXmlDeclaration = true;
						}
					}

					using (IsolatedStorageFileStream stream
						= new IsolatedStorageFileStream(logFileName, fileMode, storageFile))
					{
						using (StreamWriter writer = new StreamWriter(stream))
						{
							if (addXmlDeclaration)
							{
								writer.WriteLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
								writer.WriteLine("<Log>");
							}
							writer.WriteLine(message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DiagnosticsDebug.WriteLine("LocalStorageLog: Unable to write to file. Exception: {0}", ex);
			}
		}

		public string ReadLog()
		{
			lock (fileLock)
			{
				using (IsolatedStorageFile storageFile = GetStorageFile())
				{
					if (!storageFile.FileExists(logFileName))
					{
						return string.Empty;
					}

					using (IsolatedStorageFileStream stream
						= storageFile.OpenFile(logFileName, FileMode.Open))
					{
						using (StreamReader reader = new StreamReader(stream))
						{
							string result = reader.ReadToEnd();
							return result + "</Log>";
						}
					}
				}
			}
		}

		public void Clear()
		{
			lock (fileLock)
			{
				using (IsolatedStorageFile storageFile = GetStorageFile())
				{
					if (!storageFile.FileExists(logFileName))
					{
						return;
					}

					storageFile.DeleteFile(logFileName);
				}
			}
		}
	}
}
#endif