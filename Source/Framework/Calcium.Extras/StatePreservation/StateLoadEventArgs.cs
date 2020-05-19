using System;
using System.Collections.Generic;

namespace Codon.StatePreservation
{
	public class StateLoadEventArgs : EventArgs
	{
		public StateLoadEventArgs(
			IDictionary<string, object> persistentStateDictionary, 
			IDictionary<string, object> transientStateDictionary, 
			bool loadTransientStateRequired)
		{
			PersistentStateDictionary = persistentStateDictionary;
			TransientStateDictionary = transientStateDictionary;
			LoadTransientStateRequired = loadTransientStateRequired;
		}

		public IDictionary<string, object> PersistentStateDictionary { get; }
		public IDictionary<string, object> TransientStateDictionary { get; }

		public bool LoadTransientStateRequired { get; }
	}
}