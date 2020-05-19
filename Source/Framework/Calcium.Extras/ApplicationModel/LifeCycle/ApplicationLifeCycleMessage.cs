﻿#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-05-07 16:49:52Z</CreationDate>
</File>
*/
#endregion

using Codon.Messaging;

namespace Codon.ApplicationModel
{
	/// <summary>
	/// This message is dispatched by the <see cref="Services.IMessenger"/>
	/// when a corresponding applicatation lifecycle event occurs.
	/// <seealso cref="ApplicationLifeCycleState"/>
	/// </summary>
	public class ApplicationLifeCycleMessage 
		: MessageBase<ApplicationLifeCycleState>
	{
		public ApplicationLifeCycleState State => Payload;

		/// <inheritdoc />
		public ApplicationLifeCycleMessage(
			object sender, ApplicationLifeCycleState payload) 
			: base(sender, payload)
		{
			/* Intentionally left blank. */
		}

		public bool FastResumeExpired { get; set; }
	}
}