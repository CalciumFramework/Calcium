﻿#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-09-21 17:24:28Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Codon.UIModel.Validation
{
	/// <summary>
	/// Contains the result of a property validation.
	/// </summary>
	public class ValidationCompleteEventArgs : EventArgs
	{
		/// <summary>
		/// If an exception occurs during validation, 
		/// this property is populated with that exception.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// The property to which this validation was performed.
		/// </summary>
		public string PropertyName { get; private set; }

		/// <summary>
		/// The list of validation errors discovered
		/// during validation. This list can be displayed
		/// to the user.
		/// </summary>
		public IEnumerable<DataValidationError> Errors { get; private set; }

		public ValidationCompleteEventArgs(string propertyName)
		{
			PropertyName = AssertArg.IsNotNull(
				propertyName, nameof(propertyName)); ;
		}

		public ValidationCompleteEventArgs(
			string propertyName, IEnumerable<DataValidationError> errors)
			: this(propertyName)
		{
			Errors = errors;
		}

		public ValidationCompleteEventArgs(
			string propertyName, Exception exception)
			: this(propertyName)
		{
			Exception = exception;
		}
	}
}