#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.LauncherModel
{
	/// <summary>
	/// The base class for a unit testable <see cref="ILauncher{T}"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of object provided 
	/// by the <see cref="Completed"/> event.
	/// </typeparam>
	public abstract class MockLauncherBase<T> : ILauncher<T>
	{
		public bool Shown { get; set; }

		public virtual void Show()
		{
			Shown = true;
		}

		public virtual event EventHandler<T> Completed;

		protected void RaiseCompeleted(T args)
		{
			Completed?.Invoke(this, args);
		}
	}

	/// <summary>
	/// The base class for a unit testable <see cref="ILauncher{T}"/>.
	/// </summary>
	public abstract class MockLauncherBase : MockLauncherBase<bool>
	{
		public override void Show()
		{
			base.Show();
			RaiseCompeleted(true);
		}
	}
}
