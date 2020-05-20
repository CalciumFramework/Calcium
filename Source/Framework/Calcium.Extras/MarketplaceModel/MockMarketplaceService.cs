#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-19 12:01:26Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;

using Calcium.Services;

namespace Calcium.MarketplaceModel
{
	/// <summary>
	/// This class is a unit testable mock implementation of
	/// <see cref="IMarketplaceService"/>.
	/// </summary>
	public class MockMarketplaceService : IMarketplaceService
	{
		bool trial = true;

		public bool Trial
		{
			get => trial;
			set => trial = value;
		}

		public string Receipt { get; set; }
		
		public Task<object> PurchaseAppAsync()
		{
			trial = false;

			return Task.FromResult<object>(Receipt);
		}

		public void Review()
		{
			ReviewCalled = true;
		}

		public bool ReviewCalled { get; set; }

		public void ShowDetails(object contentId = null)
		{
			ContentId = contentId;
		}

		public object ContentId { get; set; }
	}
}
