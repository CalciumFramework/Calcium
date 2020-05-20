#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-01 12:23:28Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.UIModel.Input
{
	/// <summary>
	/// The base code contract for a command.
	/// </summary>
	public interface ICommandBase
	{
		/// <summary>
		/// Raises the <c>CanExecuteChanged</c> event of the
		/// command. This causes subscribers to requery
		/// the status of the command using the <c>CanExecute</c>
		/// method.
		/// </summary>
		void RaiseCanExecuteChanged();

		/// <summary>
		/// Refresh the command state using the specified
		/// command parameter.
		/// </summary>
		/// <param name="commandParameter">
		/// Auxiliary information used by the command.</param>
		void Refresh(object commandParameter);
	}
}
