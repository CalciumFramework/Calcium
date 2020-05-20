#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-26 15:52:48Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.StatePreservation
{
	public partial class StateManager : IStateManager
	{
		public void Initialize()
		{
			/* TODO: Implement. */
		}

		bool shouldLoadTransientState = true;

		public bool ShouldLoadTransientState => shouldLoadTransientState;
	}
}
#endif
