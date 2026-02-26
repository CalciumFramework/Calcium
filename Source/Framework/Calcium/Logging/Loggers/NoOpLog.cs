#nullable enable

#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-02-26 11:37:32Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Calcium.Logging.Loggers
{
	public class NoOpLog : LogBase
	{
		public override Task WriteAsync(LogLevel logLevel,
										string message,
										Exception exception,
										IDictionary<string, object>? properties,
										[CallerMemberName] string? memberName = null,
										[CallerFilePath] string? filePath = null,
										[CallerLineNumber] int lineNumber = 0)
		{
			return Task.CompletedTask;
		}
	}
}
