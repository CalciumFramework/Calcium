#define DEBUG

#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-18 15:31:55Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Codon.Platform;

namespace Codon.Logging.Loggers
{
	/// <summary>
	/// This is an implementation of the <see cref="ILog"/>
	/// that writes log messages 
	/// to the <c>System.Diagnostics.Debug</c> class.
	/// </summary>
	public class DebugLog : LogBase
	{
		bool useConsole;
		PlatformId platformId;
		bool debuggerAttached = Debugger.IsAttached;
		bool initialized;

		void Init()
		{
			/* The Mono linker doesn't appear to respect the DEBUG symbol defined at the top of this file. 
			 * It means that Debug.WriteLine(...) doesn't work on those platform. 
			 * Hence the use of the console. */
			platformId = PlatformDetector.PlatformId;
			if (platformId == PlatformId.Android 
				|| platformId == PlatformId.Ios)
			{
				useConsole = true;
			}
		}

		public override Task WriteAsync(
			LogLevel logLevel,
			string message,
			Exception exception,
			IDictionary<string, object> properties,
			[CallerMemberName]string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			if (!debuggerAttached)
			{
				return Task.CompletedTask;
			}

			if (!initialized)
			{
				Init();
				initialized = true;
			}

//			if (platformId == PlatformId.Wpf)
//			{
//				/* .NET Standard and WPF sometimes has an issue with you Debug.WriteLine and Console.WriteLine.
//				 * there is a platform specific WPF DebugLogWpf class in the WPF platform library. */
//				return Task.CompletedTask;
//			}

			string exceptionMessage = exception != null ? exception.ToString() : string.Empty;
			string logMessage = string.Format("Log {0:G} - {1} member:{2} file:{3} line:{4} exception:{5}", 
									logLevel, message, memberName, filePath,
									lineNumber.ToString(CultureInfo.InvariantCulture),
									exceptionMessage);
			
#if NETSTANDARD || NETFX_CORE
			if (useConsole)
			{
				Console.WriteLine("{0:G} {1}", logLevel, logMessage);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("{0:G} {1}", logLevel, logMessage);
			}
#else
			Debugger.Log((int)logLevel, logLevel.ToString("G"), logMessage);
#endif
			return Task.CompletedTask;
		}
	}
}
