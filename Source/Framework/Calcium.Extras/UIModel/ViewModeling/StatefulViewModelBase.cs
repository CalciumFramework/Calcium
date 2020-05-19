#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-02-21 17:24:08Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;

using Codon.StatePreservation;

namespace Codon.UIModel
{
	/// <summary>
	/// A view-model that includes built-in support
	/// for property state preservation.
	/// </summary>
	public abstract class StatefulViewModelBase 
		: ViewModelBase, IStateful
	{
		readonly StatePreservationStrategy stateStrategy;

		/// <summary>
		/// Gets the <see cref="StatePreservationStrategy"/>
		/// that is used to register and persist property state.
		/// </summary>
		protected StatePreservationStrategy State => stateStrategy;

		protected StatefulViewModelBase()
		{
			stateStrategy = new StatePreservationStrategy(GetType());
		}

		void IStateful.LoadState(
			IDictionary<string, object> persistentStateDictionary,
			IDictionary<string, object> transientStateDictionary,
			bool loadTransientStateRequired)
		{
			stateStrategy?.LoadState(
				persistentStateDictionary,
				transientStateDictionary,
				loadTransientStateRequired);
		}

		void IStateful.SaveState(
			IDictionary<string, object> persistentStateDictionary,
			IDictionary<string, object> transientStateDictionary)
		{
			stateStrategy?.SaveState(
				persistentStateDictionary,
				transientStateDictionary);
		}
	}
}