#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.DialogModel
{
	/// <summary>
	/// Represents a user response for a question asked via the
	/// <see cref="Services.IDialogService"/>.
	/// </summary>
	/// <typeparam name="TResponse">
	/// The type of object that is specific to the question,
	/// such as a <see cref="TextResponse"/>.</typeparam>
	public class QuestionResponse<TResponse>
	{
		/// <summary>
		/// The question asked. This property is intended to be used 
		/// to match up the response with the original question.
		/// </summary>
		public IQuestion<TResponse> Question { get; private set; }

		/// <summary>
		/// The response provided by the user.
		/// </summary>
		public TResponse Response { get; private set; }

		/// <summary>
		/// If an error occurs while asking the question
		/// it is assigned to this property.
		/// </summary>
		public Exception Error { get; private set; }

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="result">The user response.</param>
		/// <param name="question">The original question.</param>
		/// <param name="error">
		/// If an error occurs while asking the question
		/// it is passed to the constructor.</param>
		public QuestionResponse(
			TResponse result, 
			IQuestion<TResponse> question, 
			Exception error = null)
		{
			Response = result;
			Question = AssertArg.IsNotNull(question, nameof(question));
			Error = error;
		}
	}
}
