#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-01-23 17:21:10Z</CreationDate>
</File>
*/
#endregion

namespace Codon.UndoModel
{
	/// <summary>
	/// Is used to indicate the execution result of an <see cref="IUnit"/>.
	/// </summary>
	public enum UnitResult
	{
		/// <summary>
		/// The unit completed successfully.
		/// </summary>
		Completed,
		/// <summary>
		/// The unit was cancelled by the instance, or nested instance.
		/// </summary>
		Cancelled,
		/// <summary>
		/// Unit handled failure.
		/// </summary>
		Failed,
		/// <summary>
		/// State changed in that the unit was deemed no longer executable.
		/// </summary>
		NoUnit
	}

}