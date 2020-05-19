#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2008-11-03 23:19:09Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.Concurrency
{
	/// <summary>
	/// The exception that is thrown when a thread violates
	/// a concurrency constraint, such as causing 
	/// a prohibited cross-thread operation.
	/// </summary>
	public class ConcurrencyException : Exception
	{
		public ConcurrencyException()
		{
		}

		public ConcurrencyException(string message) : base(message)
		{
		}

		public ConcurrencyException(string message, Exception ex)
			: base(message, ex)
		{
		}

	}
}
