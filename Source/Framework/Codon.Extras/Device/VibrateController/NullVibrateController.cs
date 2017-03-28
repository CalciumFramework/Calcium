#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 15:38:54Z</CreationDate>
</File>
*/
#endregion

using System;
using Codon.Services;

namespace Codon.Device
{
	/// <summary>
	/// An implementation of the <see cref="IVibrateController"/>
	/// interface that does nothing. If a platform has a vibration
	/// API, then it should be implementated as 
	/// <c>Codon.Device.VibrateController</c> 
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
