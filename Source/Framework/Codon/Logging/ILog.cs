#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-18 15:27:07Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Codon.InversionOfControl;
using Codon.Logging.Loggers;

namespace Codon.Logging
{
	/// <summary>
	/// Represents a output channel 
	/// for storing or transmitting log messages.
	/// </summary>
	[DefaultType(typeof(DebugLog), Singleton = true)]
	public interface ILog
	{
		/// <summary>
		/// Writes a message to the log. 
		/// This is the usual override point for an implementation 
		/// of an <c>ILog</c> implementation.
		/// </summary>
		/// <param name="logLevel">Is used by filters to decide 
		/// whether to process the message.</param>
		/// <param name="message">The textual content of the entry.</param>
		/// <param name="exception">Can be null.</param>
		/// <param name="properties">Can be null. A optional list of properties 
		/// that may (or may not) be used by a particular <c>ILog</c> implementation.</param>
		/// <param name="memberName">The current member.</param>
		/// <param name="filePath">The name of the file.</param>
		/// <param name="lineNumber">The originating line number.</param>
		Task WriteAsync(
			LogLevel logLevel,
			string message,
			Exception exception,
			IDictionary<string, object> properties,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0);

		/// <summary>
		/// Determines if a log entry will be created
		/// at the specified level. Use this method
		/// to prevent the unnecessary log entry preparation.
		/// </summary>
		/// <param name="logLevel"></param>
		/// <returns><c>true</c> if a call at the specified <seealso cref="LogLevel"/>
		/// will succeed; <c>false</c> otherwise.</returns>
		bool IsLogEnabledForLevel(LogLevel logLevel);

		/// <summary>
		/// Writes a log entry 
		/// at the <see cref="LogLevel.Trace"/> level.
		/// The log entry is not processed if the minimum log level
		/// is greater than the trace level.
		/// </summary>
		/// <param name="message">The message to be written to the log.</param>
		/// <param name="exception">An exception to be written to the log. 
		/// (Can be null)</param>
		/// <param name="properties">A dictionary of properties 
		/// that may be leveraged by the <c>ILog</c> implementation.</param>
		/// <param name="memberName">
		/// The class member name of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="filePath">
		/// The file path of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="lineNumber">
		/// The line number of the call origin.
		/// Automatically populated by the compiler.</param>
		void Trace(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0);

		/// <summary>
		/// Writes a log entry 
		/// at the <see cref="LogLevel.Debug"/> level.
		/// The log entry is not processed if the minimum log level
		/// is greater than the debug level.
		/// </summary>
		/// <param name="message">The message to be written to the log.</param>
		/// <param name="exception">An exception to be written to the log. 
		/// (Can be null)</param>
		/// <param name="properties">A dictionary of properties 
		/// that may be leveraged by the <c>ILog</c> implementation.</param>
		/// <param name="memberName">
		/// The class member name of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="filePath">
		/// The file path of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="lineNumber">
		/// The line number of the call origin.
		/// Automatically populated by the compiler.</param>
		void Debug(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0);

		/// <summary>
		/// Writes a log entry 
		/// at the <see cref="LogLevel.Info"/> level.
		/// The log entry is not processed if the minimum log level
		/// is greater than the info level.
		/// </summary>
		/// <param name="message">The message to be written to the log.</param>
		/// <param name="exception">An exception to be written to the log. 
		/// (Can be null)</param>
		/// <param name="properties">A dictionary of properties 
		/// that may be leveraged by the <c>ILog</c> implementation.</param>
		/// <param name="memberName">
		/// The class member name of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="filePath">
		/// The file path of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="lineNumber">
		/// The line number of the call origin.
		/// Automatically populated by the compiler.</param>
		void Info(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0);

		/// <summary>
		/// Writes a log entry 
		/// at the <see cref="LogLevel.Warn"/> level.
		/// The log entry is not processed if the minimum log level
		/// is greater than the warn level.
		/// </summary>
		/// <param name="message">The message to be written to the log.</param>
		/// <param name="exception">An exception to be written to the log. 
		/// (Can be null)</param>
		/// <param name="properties">A dictionary of properties 
		/// that may be leveraged by the <c>ILog</c> implementation.</param>
		/// <param name="memberName">
		/// The class member name of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="filePath">
		/// The file path of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="lineNumber">
		/// The line number of the call origin.
		/// Automatically populated by the compiler.</param>
		void Warn(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0);

		/// <summary>
		/// Writes a log entry 
		/// at the <see cref="LogLevel.Error"/> level.
		/// The log entry is not processed if the minimum log level
		/// is greater than the error level.
		/// </summary>
		/// <param name="message">The message to be written to the log.</param>
		/// <param name="exception">An exception to be written to the log. 
		/// (Can be null)</param>
		/// <param name="properties">A dictionary of properties 
		/// that may be leveraged by the <c>ILog</c> implementation.</param>
		/// <param name="memberName">
		/// The class member name of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="filePath">
		/// The file path of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="lineNumber">
		/// The line number of the call origin.
		/// Automatically populated by the compiler.</param>
		void Error(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0);

		/// <summary>
		/// Writes a log entry 
		/// at the <see cref="LogLevel.Fatal"/> level.
		/// The log entry is not processed if the minimum log level
		/// is equal to <see cref="LogLevel.None"/>.
		/// </summary>
		/// <param name="message">The message to be written to the log.</param>
		/// <param name="exception">An exception to be written to the log. 
		/// (Can be null)</param>
		/// <param name="properties">A dictionary of properties 
		/// that may be leveraged by the <c>ILog</c> implementation.</param>
		/// <param name="memberName">
		/// The class member name of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="filePath">
		/// The file path of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="lineNumber">
		/// The line number of the call origin.
		/// Automatically populated by the compiler.</param>
		void Fatal(
			string message,
			Exception exception = null,
			IDictionary<string, object> properties = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0);

		/// <summary>
		/// Return <c>true</c> is a request to write 
		/// a log entry at the <see cref="LogLevel.Trace"/>
		/// would succeed; <c>false</c> otherwise.
		/// </summary>
		bool TraceEnabled { get; }

		/// <summary>
		/// Return <c>true</c> is a request to write 
		/// a log entry at the <see cref="LogLevel.Debug"/>
		/// would succeed; <c>false</c> otherwise.
		/// </summary>
		bool DebugEnabled { get; }

		/// <summary>
		/// Return <c>true</c> is a request to write 
		/// a log entry at the <see cref="LogLevel.Info"/>
		/// would succeed; <c>false</c> otherwise.
		/// </summary>
		bool InfoEnabled { get; }

		/// <summary>
		/// Return <c>true</c> is a request to write 
		/// a log entry at the <see cref="LogLevel.Warn"/>
		/// would succeed; <c>false</c> otherwise.
		/// </summary>
		bool WarnEnabled { get; }

		/// <summary>
		/// Return <c>true</c> is a request to write 
		/// a log entry at the <see cref="LogLevel.Error"/>
		/// would succeed; <c>false</c> otherwise.
		/// </summary>
		bool ErrorEnabled { get; }

		/// <summary>
		/// Return <c>true</c> is a request to write 
		/// a log entry at the <see cref="LogLevel.Fatal"/>
		/// would succeed; <c>false</c> otherwise.
		/// </summary>
		bool FatalEnabled { get; }
	}
}