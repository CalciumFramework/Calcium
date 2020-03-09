#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-18 15:28:47Z</CreationDate>
</File>
*/
#endregion

namespace Codon.Logging
{
	/// <summary>
	/// Indicates the level at which to write a log entry.
	/// Are used to exclude log entries that have log levels
	/// less than the current threshold level.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// The least restrictive level.
		/// </summary>
		All = 0,
		/// <summary>
		/// For debugging purposes. More verbose than the Debug level.
		/// </summary>
		Trace = 1,
		/// <summary>
		/// For debugging purposes. More verbose than the Info level 
		/// and less verbose than the Trace level.
		/// </summary>
		Debug = 2,
		/// <summary>
		/// Signifies verbose information. More verbose than the Warn level 
		/// and less verbose than the Debug level.
		/// </summary>
		Info = 4,
		/// <summary>
		/// Signifies a warning from e.g. an unexpected event.
		/// </summary>
		Warn = 8,
		/// <summary>
		/// When an application error occurs.
		/// </summary>
		Error = 16,
		/// <summary>
		/// When the application is no longer
		/// able to function or is in an unreliable state.
		/// Highly restrictive logging.
		/// </summary>
		Fatal = 32,
		/// <summary>
		/// Logging is disabled.
		/// </summary>
		None = 64
	}

	/// <summary>
	/// Extension methods for the <see cref="LogLevel"/> enum.
	/// </summary>
	public static class LogLevelExtensions
	{
		/// <summary>
		/// Determines if the specified <c>LogLevel</c>
		/// is of a higher level. 
		/// </summary>
		/// <param name="logLevel">The current log level.</param>
		/// <param name="compareToLevel">A level to compare.</param>
		/// <returns><c>true</c> if the specified <c>LogLevel</c>
		/// is of a equal or higher level; <c>false</c> otherwise.</returns>
		public static bool IsGreaterThanOrEqualTo(
			this LogLevel logLevel, LogLevel compareToLevel)
		{
			return logLevel >= compareToLevel;
		}

		/// <summary>
		/// Determines if the specified <c>LogLevel</c>
		/// is of a lesser level. 
		/// </summary>
		/// <param name="logLevel">The current log level.</param>
		/// <param name="compareToLevel">A level to compare.</param>
		/// <returns><c>true</c> if the specified <c>LogLevel</c>
		/// is of a lesser or equal level; <c>false</c> otherwise.</returns>
		public static bool IsLessThanOrEqualTo(
			this LogLevel logLevel, LogLevel compareToLevel)
		{
			return logLevel <= compareToLevel;
		}
	}
}