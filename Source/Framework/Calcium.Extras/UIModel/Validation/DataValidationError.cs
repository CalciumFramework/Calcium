#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-09-20 20:05:58Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.Services;

namespace Calcium.UIModel.Validation
{
	/// <summary>
	/// Provides information about an object member that
	/// has failed validation.
	/// </summary>
	public interface IDataValidationError
	{
		/// <summary>
		/// A unique ID of the error that can be used to remove
		/// the error from the <see cref="DataErrorNotifier"/>.
		/// </summary>
		Guid   Id           { get; }

		/// <summary>
		/// The reason for the validation error.
		/// </summary>
		string ErrorMessage { get; }
	}

	/// <inheritdoc cref="IDataValidationError" />
	public class DataValidationError : IDataValidationError, 
									   IEquatable<IDataValidationError>
	{
		/// <inheritdoc />
		public Guid Id { get; }

		string errorMessage;

		/// <inheritdoc />
		public string ErrorMessage
		{
			get
			{
				var service = Dependency.Resolve<IStringParserService>();
				return service.Parse(errorMessage);
			}
			set => errorMessage = value;
		}

		/// <param name="id">The unique ID of this error.</param>
		/// <param name="errorMessage">The error message. Must not be <c>null</c>.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified error message is <c>null</c>.</exception>
		public DataValidationError(Guid id, string errorMessage)
		{
			Id                = id;
			this.errorMessage = AssertArg.IsNotNull(errorMessage, nameof(errorMessage));
		}

		public override string ToString()
		{
			return ErrorMessage;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			return obj is IDataValidationError other && Equals(other);
		}

		public override int GetHashCode() => Id.GetHashCode();

		public bool Equals(IDataValidationError other)
		{
			if (other == null)
			{
				return false;
			}

			return Id.Equals(other.Id);
		}
	}
}
