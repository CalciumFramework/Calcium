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
