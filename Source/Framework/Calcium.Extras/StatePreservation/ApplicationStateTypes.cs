using System;

namespace Calcium.StatePreservation
{
	[Flags]
	public enum ApplicationStateTypes
	{
		Persistent,
		Transient
	}

	/// <summary>
	/// Used by the state preservation subsystem.
	/// </summary>
	public enum ApplicationStateType
	{
		Persistent,
		Transient
	}
}
