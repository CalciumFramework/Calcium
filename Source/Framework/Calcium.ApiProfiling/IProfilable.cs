#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-04-08 12:53:29Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Codon.ApiProfiling
{
    interface IProfilable
    {
	    ProfileResult Profile();
    }

	class ProfileMetric
	{
		public string Name { get; set; }
		public TimeSpan TimeSpan { get; set; }
	}

	class ProfileResult
	{
		public string Name { get; set; }

		public List<ProfileMetric> Metrics { get; } = new List<ProfileMetric>();
	}
}
