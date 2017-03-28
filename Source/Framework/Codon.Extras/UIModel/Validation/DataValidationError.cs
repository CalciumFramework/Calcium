#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-09-20 20:05:58Z</CreationDate>
</File>
*/
#endregion

using System;
using Codon.Services;

namespace Codon.UIModel.Validation
{
	/// <summary>
	/// Provides information about an object member that
	/// has failed validation.
	/// </summary>
	public class DataValidationError
	{
		public int Id { get; set; }

		string errorMessage;

		/// <summary>
		/// Gets or sets the error message that is displayed to the user.
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage
		{
			get
			{
				var service = Dependency.Resolve<IStringParserService>();
				return service.Parse(errorMessage);
			}
			set => errorMessage = value;
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="DataValidationError"/> class.
		/// </summary>
		public DataValidationError()
		{
			/* Intentionally left blank. */
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="DataValidationError"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="errorMessage">The error message.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified error message is <c>null</c>.</exception>
		public DataValidationError(int id, string errorMessage)
		{
			Id = id;
			this.errorMessage = AssertArg.IsNotNull(
						errorMessage, nameof(errorMessage));
		}

		public override string ToString()
		{
			return ErrorMessage;
		}
	}
}