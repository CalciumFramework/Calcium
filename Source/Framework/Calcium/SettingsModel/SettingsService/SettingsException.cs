#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-09-29 12:38:23Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.Services;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// Represents an error raised by the <see cref="ISettingsService" /> imlementation.
	/// </summary>
	public class SettingsException : Exception
	{
		/// <inheritdoc />
		public SettingsException()
		{
		}

		/// <inheritdoc />
		public SettingsException(string message) : base(message)
		{
		}

		/// <inheritdoc />
		public SettingsException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
