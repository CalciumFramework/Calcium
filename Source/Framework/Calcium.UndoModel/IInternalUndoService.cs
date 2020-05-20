#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-02-14 17:43:52Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.UndoModel
{
	/// <summary>
	/// Allows a <see cref="UnitBase{T}"/> to indicate 
	/// to the <see cref="UndoService"/>
	/// that its <c>Repeatable</c> property has changed.
	/// </summary>
	interface IInternalUndoService
	{
		/// <summary>
		/// Notifies the service that the repeatable property has changed.
		/// </summary>
		/// <param name="unit">The unit.</param>
		void NotifyUnitRepeatableChanged(IInternalUnit unit);
	}
}
