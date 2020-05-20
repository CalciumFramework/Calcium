#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-09-06 16:53:52Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.ComponentModel
{
	/// <summary>
	/// Indicates the result of a property value assignment.
	/// <see cref="PropertyChangeNotifier.Set{TProperty,TField}"/>.
	/// </summary>
	public enum AssignmentResult
	{
		/// <summary>
		/// The assignment succeeded and the value changed.
		/// </summary>
		Success,

		/// <summary>
		/// A <see cref="System.ComponentModel.INotifyPropertyChanging"/> 
		/// event subscriber cancelled the update.
		/// </summary>
		Cancelled,

		/// <summary>
		/// The property value is already equal to the specified value.
		/// No change was made.
		/// </summary>
		AlreadyAssigned,

		/// <summary>
		/// The owner object associated with the 
		/// <see cref="PropertyChangeNotifier"/> was disposed.
		/// No further change events can be raised.
		/// </summary>
		OwnerDisposed
	}
}
