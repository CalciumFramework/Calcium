#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 15:38:54Z</CreationDate>
</File>
*/
#endregion

using System;
using Calcium.Services;

namespace Calcium.Device
{
	/// <summary>
	/// An implementation of the <see cref="IVibrateController"/>
	/// interface that does nothing. If a platform has a vibration
	/// API, then it should be implementated as 
	/// <c>Calcium.Device.VibrateController</c> 
	/// in a platform specific assembly.
	/// </summary>
	public class NullVibrateController : IVibrateController
	{
		public void Start(TimeSpan duration)
		{
		}

		public void Stop()
		{
		}
	}
}
