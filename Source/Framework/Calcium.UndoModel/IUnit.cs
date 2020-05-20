#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-01-23 17:21:10Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.UndoModel
{
	/// <summary>
	/// An unit performs an application unit.
	/// An unit may be a command behaviour, that is, it may encapsulate 
	/// the logic performed when a command is initiated.
	/// </summary>
	public interface IUnit
	{
		/// <summary>
		/// Gets the user friendly description of the unit.
		/// </summary>
		/// <value>The description.</value>
		string DescriptionForUser { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IUnit"/> can be undone.
		/// This means that the unit is able to roll back changes made during its execution.
		/// </summary>
		/// <value><c>true</c> if undoable; otherwise, <c>false</c>.</value>
		bool Undoable
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IInternalUnit"/> 
		/// can be executed more than once.
		/// </summary>
		/// <value><c>true</c> if repeatable; otherwise, <c>false</c>.</value>
		bool Repeatable
		{
			get;
		}
	}
}
