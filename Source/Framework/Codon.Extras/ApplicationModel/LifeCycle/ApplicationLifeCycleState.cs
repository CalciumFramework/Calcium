#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-05-07 16:55:27Z</CreationDate>
</File>
*/
#endregion

namespace Codon.ApplicationModel
{
	/// <summary>
	/// This enum represents the various life cycle states 
	/// of an application.
	/// For example, when the application is launching, 
	/// the <see cref="Services.IMessenger"/>
	/// may dispatch a <see cref="ApplicationLifeCycleMessage"/> 
	/// with the value <see cref="Launching"/>.
	/// </summary>
	public enum ApplicationLifeCycleState
	{
		Activated,
		Deactivated,
		Launching,
		Closing
	}
}