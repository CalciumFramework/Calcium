#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Codon.StatePreservation
{
	/// <summary>
	/// This class is used to broadcast a message 
	/// via the <see cref="Services.IMessenger"/>
	/// to notify subscribers that they should save their state.
	/// </summary>
	public class SaveStateRequest : EventArgs
	{
		public ApplicationStateTypes ApplicationStateTypes { get; private set; }

		public IDictionary<string, object> TransientStateDictionary { get; private set; }

		public SaveStateRequest(
			ApplicationStateTypes stateTypes,
			IDictionary<string, object> transientStateDictionary)
		{
			ApplicationStateTypes = stateTypes;
			TransientStateDictionary = transientStateDictionary;
		}
	}
}
