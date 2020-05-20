#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-04-15 21:51:24Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.IO
{
	/// <summary>
	/// This exception is raised when, during the process of deserialization, 
	/// a serialized memento is found with a higher version than 
	/// can be handled. This usually indicates an older application
	/// version is being used to open a file that was created 
	/// in a newer version.
	/// </summary>
	//[Serializable]
	public class MementoIncompatibilityException : Exception
	{
		/// <summary>
		/// Gets or sets the expected memento version 
		/// that is supported by the memento type.
		/// </summary>
		/// <value>The expected memento version 
		/// that is supported by the memento type.</value>
		public double ExpectedMementoVersion { get; }

		/// <summary>
		/// Gets or sets the actual memento version 
		/// that was found on the saved memento.
		/// </summary>
		/// <value>The actual memento version 
		/// that was found on the saved memento.</value>
		public double ActualMementoVersion { get; }

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="MementoIncompatibilityException"/> class.
		/// </summary>
		/// <param name="expectedMementoVersion">
		/// The memento version that is supported.</param>
		/// <param name="actualMementoVersion">
		/// The memento version that was found on the saved memento.
		/// </param>
		/// <param name="message">
		/// The message that may contain 
		/// detailed information pertaining the exception.</param>
		public MementoIncompatibilityException(
			double expectedMementoVersion, 
			double actualMementoVersion,
			string message)
			: base(message)
		{
			ExpectedMementoVersion = expectedMementoVersion;
			ActualMementoVersion = actualMementoVersion;
		}

		public override string ToString()
		{
			return string.Format("ExpectedMementoVersion: {0}, ActualMementoVersion: {1}, {2}",
				ExpectedMementoVersion, 
				ActualMementoVersion, 
				base.ToString());
		}
	}
}
