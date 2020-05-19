#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-27 15:13:08Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.InversionOfControl
{
	/// <summary>
	/// Occurs when an exception is raised while attempting
	/// to resolve a object via the <see cref="FrameworkContainer"/>.
	/// </summary>
	//[Serializable]
	public class ResolutionException : Exception
	{
		public ResolutionException()
		{
		}

		public ResolutionException(string message) : base(message)
		{
		}

		public ResolutionException(string message, Exception innerException) 
			: base(message, innerException)
		{
		}
	}
}