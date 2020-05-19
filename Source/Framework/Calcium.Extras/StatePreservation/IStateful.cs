#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-17 15:50:52Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;

namespace Codon.StatePreservation
{
	/// <summary>
	/// Classes implementing this interface are able
	/// to load and restore state.
	/// </summary>
	public interface IStateful
	{
		/// <summary>
		/// Load or restore state using the specified
		/// state dictionaries. If <c>loadTransientStateRequired</c>
		/// is <c>true</c> state should be loaded from the 
		/// <c>transientStateDictionary</c>.
		/// </summary>
		/// <param name="persistentStateDictionary">
		/// A state dictionary whose contents survives
		/// across app launch and exit cycles.</param>
		/// <param name="transientStateDictionary">
		/// A state dictionary whose contents survives
		/// while the application is not exited.
		/// Its contents may also be persisted if the application
		/// is tombstoned.</param>
		/// <param name="loadTransientStateRequired">
		/// If <c>true</c>, state should be loaded from the
		/// <c>transientStateDictionary</c>.</param>
		void LoadState(IDictionary<string, object> persistentStateDictionary,
						IDictionary<string, object> transientStateDictionary,
						bool loadTransientStateRequired);

		/// <summary>
		/// Saves state to the specified state dictionaries.
		/// </summary>
		/// <param name="persistentStateDictionary">
		/// A state dictionary whose contents survives
		/// across app launch and exit cycles.</param>
		/// <param name="transientStateDictionary">
		/// A state dictionary whose contents survives
		/// while the application is not exited.
		/// Its contents may also be persisted if the application
		/// is tombstoned.</param>
		void SaveState(IDictionary<string, object> persistentStateDictionary,
						IDictionary<string, object> transientStateDictionary);
	}
}