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
	public class LoadStateRequest : EventArgs
	{
		public ApplicationStateTypes ApplicationStateTypes { get; set; }
		public IDictionary<string, object> TransientStateDictionary { get; private set; }

		public LoadStateRequest(
			ApplicationStateTypes stateTypes,
			IDictionary<string, object> transientStateDictionary)
		{
			ApplicationStateTypes = stateTypes;
			TransientStateDictionary = transientStateDictionary;
		}
	}
}
