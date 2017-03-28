using System;
using System.Runtime.CompilerServices;
using Codon.ComponentModel.ExceptionHandlers;
using Codon.InversionOfControl;

namespace Codon.ComponentModel
{
	/// <summary>
	/// Interface specifying the functionality to be implemented 
	/// by an object that wishes to manage the handling of exceptions.
	/// </summary>
	[DefaultType(typeof(LoggingExceptionHandler))]
	public interface IExceptionHandler
	{
		/// <summary>
		/// Determines if an exception should be re-thrown
		/// or can be safely ignored.
		/// </summary>
		/// <param name="exception">
		/// The exception that was unhandled.</param>
		/// <param name="owner">
		/// The object directly associated with the exception.</param>
		/// <param name="memberName">
		/// The class member name of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="filePath">
		/// The file path of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <param name="lineNumber">
		/// The line number of the call origin.
		/// Automatically populated by the compiler.</param>
		/// <returns><c>true</c> if the exception should be rethrown;
		/// <c>false</c> if it has been handled.</returns>
		bool ShouldRethrowException(
				Exception exception, 
				object owner, 
				[CallerMemberName]string memberName = null, 
				[CallerFilePath]string filePath = null, 
				[CallerLineNumber]int lineNumber = 0);
	}
}