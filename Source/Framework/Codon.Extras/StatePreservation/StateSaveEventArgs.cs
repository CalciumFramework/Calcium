using System;
using System.Collections.Generic;

namespace Codon.StatePreservation
{
	public class StateSaveEventArgs : EventArgs
	{
		public StateSaveEventArgs(
			IDictionary<string, object> persistentStateDictionary, 
			IDictionary<string, object> transientStateDictionary)
		{
			PersistentStateDictionary = persistentStateDictionary;
			TransientStateDictionary = transientStateDictionary;
		}

		public IDictionary<string, object> PersistentStateDictionary { get; }
		public IDictionary<string, object> TransientStateDictionary { get; }
	}
}