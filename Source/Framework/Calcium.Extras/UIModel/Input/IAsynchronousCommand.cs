#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-01 12:23:42Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;

namespace Calcium.UIModel.Input
{
	/// <summary>
	/// This interface represents an
	/// <see cref="System.Windows.Input.ICommand"/>
	/// that is able to be refreshed asynchronously.
	/// </summary>
	public interface IAsynchronousCommand : ICommandBase
	{
		/// <summary>
		/// Refresh the command state using the specified
		/// command parameter.
		/// </summary>
		/// <param name="commandParameter">
		/// Auxiliary information used by the command.</param>
		Task RefreshAsync(object commandParameter);
	}
}
