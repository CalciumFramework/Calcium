#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-09 21:04:14Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.LauncherModel.Launchers;

namespace Calcium.LauncherModel
{
	/// <summary>
	/// A chooser launches an external activity
	/// and returns a result. For example, see
	/// <see cref="IPhotoLauncher"/>
	/// </summary>
	/// <typeparam name="T">
	/// The type of object received by the <see cref="Completed"/>
	/// event handler.</typeparam>
	public interface ILauncher<T>
	{
		/// <summary>
		/// Launch the external activity.
		/// </summary>
		void Show();

		/// <summary>
		/// Raised when the external activity completes
		/// and returns control to the application.
		/// </summary>
		event EventHandler<T> Completed;
	}
}
