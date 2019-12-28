#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-02-14 17:42:01Z</CreationDate>
</File>
*/
#endregion

namespace Codon.UndoModel
{
	/// <summary>
	/// Provides functionality to perform and repeat 
	/// a unit within the <see cref="UndoService"/>.
	/// </summary>
	interface IInternalUnit : IUnit
	{
		/// <summary>
		/// Gets the argument that is used during unit execution.
		/// </summary>
		/// <value>The argument used during execution.</value>
		object Argument
		{ 
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IInternalUnit"/> 
		/// can be executed more than once.
		/// </summary>
		/// <value><c>true</c> if repeatable; otherwise, <c>false</c>.</value>
		/// <summary>
		/// Performs the unit, raising events to allowing user code to execute.
		/// </summary>
		/// <param name="argument">The argument used during execution.</param>
		/// <param name="unitMode">Indicates whether the unit is being performed
		/// for the first time, whether it is being redone, or whether it is being repeated.</param>
		/// <returns></returns>
		UnitResult PerformUnit(object argument, UnitMode unitMode);
	}
}