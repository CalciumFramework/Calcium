#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-18 15:28:57Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codon.Logging
{
	/// <summary>
	/// Base implementation of the <see cref="ILog"/> interface.
	/// To create a custom <c>ILog</c> implementation,
	/// derive from this class and implement 
	/// the abstract <see cref="WriteAsync"/> method.
	/// </summary>
	public abstract class LogBase : ILog
	{
		public LogLevel MinimumLogLevel { get; set; } 
			= Debugger.IsAttached ? LogLevel.All : LogLevel.Error;

		/// <summary>
		/// Writes a message to the log. 
		/// This is the usual override point for an implementation of an <c>ILog</c> implementation.
		/// </summary>
		/// <param name="logLevel">Is used by filters to decide whether to process the message.</param>
		/// <param name="message">The textual content of the entry.</param>
		/// <param name="exception">Can be null.</param>
		/// <param name="properties">Can be null. A optional list of properties 
		/// that may (or may not) be used by a particular <c>ILog</c> implementation.</param>
		/// <param name="memberName">The current member.</param>
		/// <param name="filePath">The name of the file.</param>
		/// <param name="lineNumber">The originating line number.</param>
		public abstract Task WriteAsync(
			LogLevel logLevel,
			string message,
			Exception exception,
			IDictionary<string, object> properties,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0);

		public bool IsLogEnabledForLevel(LogLevel logLevel)
		{
			return MinimumLogLevel.IsLessThanOrEqualTo(logLevel);
		}

		public void Trace(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			WriteAsync(LogLevel.Trace, message, exception, properties, memberName, filePath, lineNumber);
		}

		public void Debug(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			WriteAsync(LogLevel.Debug, message, exception, properties, memberName, filePath, lineNumber);
		}

		public void Info(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			WriteAsync(LogLevel.Info, message, exception, properties, memberName, filePath, lineNumber);
		}

		public void Warn(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			WriteAsync(LogLevel.Warn, message, exception, properties, memberName, filePath, lineNumber);
		}

		public void Error(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			WriteAsync(LogLevel.Error, message, exception, properties, memberName, filePath, lineNumber);
		}

		public void Fatal(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			WriteAsync(LogLevel.Fatal, message, exception, properties, memberName, filePath, lineNumber);
		}

		public bool TraceEnabled => IsLogEnabledForLevel(LogLevel.Trace);
		public bool DebugEnabled => IsLogEnabledForLevel(LogLevel.Debug);
		public bool InfoEnabled => IsLogEnabledForLevel(LogLevel.Info);
		public bool WarnEnabled => IsLogEnabledForLevel(LogLevel.Warn);
		public bool ErrorEnabled => IsLogEnabledForLevel(LogLevel.Error);
		public bool FatalEnabled => IsLogEnabledForLevel(LogLevel.Fatal);
	}
}