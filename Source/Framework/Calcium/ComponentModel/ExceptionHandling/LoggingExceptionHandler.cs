#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-05 15:27:23Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.Logging;

namespace Calcium.ComponentModel.ExceptionHandlers
{
	/// <summary>
	/// The default implementation of <see cref="IExceptionHandler"/>.
	/// This class does not handle exceptions, but merely logs
	/// them and instructs the caller to rethrow them.
	/// </summary>
    class LoggingExceptionHandler : IExceptionHandler
    {
	    public bool ShouldRethrowException(
			Exception exception, 
			object owner, 
			string memberName = null, 
			string filePath = null,
		    int lineNumber = 0)
	    {
		    var log = Dependency.Resolve<ILog>();
		    if (log.ErrorEnabled)
		    {
			    log.Error("LoggingExceptionHandler: Unhandled exception occurred. " + owner,
							exception, null, memberName, filePath, lineNumber);
		    }

		    return true;
	    }
    }
}
