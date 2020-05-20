#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-17 15:50:52Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Calcium.StatePreservation
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class StatefulAttribute : Attribute
	{
		public ApplicationStateType StateType { get; private set; }

		public StatefulAttribute(ApplicationStateType stateType)
		{
			StateType = stateType;
		}
	}
}
