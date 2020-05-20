#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-23 17:06:15Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.UserOptionsModel
{
	/// <summary>
	/// Provides the result of a user option validation.
	/// </summary>
	public class ValidationResult
	{
		public Exception Exception { get; set; }
		public ValidationResultValue Value { get; private set; }
		public string FailureReason { get; private set; }

		/// <summary>
		/// Used to initialize a successful validation result.
		/// </summary>
		public ValidationResult()
		{
			/* Intentionally left blank. */
		}

		/// <summary>
		/// Used to initialize a failed validation result.
		/// </summary>
		/// <param name="validationResultValue">The validation result value.</param>
		/// <param name="failureReason">The failure reason.</param>
		public ValidationResult(ValidationResultValue validationResultValue, string failureReason)
		{
			Value = validationResultValue;
			FailureReason = failureReason;
		}

		/// <summary>
		/// Used to initialize a failed validation result, 
		/// where the validation caused an exception being raised.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="description">The description.</param>
		public ValidationResult(Exception exception, string description)
		{
			Exception = exception;
			Value = ValidationResultValue.InvalidRaisedException;
			FailureReason = description;
		}
	}
}
