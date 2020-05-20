#if __ANDROID__
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Calcium.Services;

namespace Calcium.MarketplaceModel
{
	public class MarketplaceService : IMarketplaceService
	{
		public bool Trial { get; private set; }

		public Task<object> PurchaseAppAsync()
		{
			Review();

			return Task.FromResult<object>(null);
		}

		public void Review()
		{
			var context = Dependency.Resolve<Context>();
			
			var appPackageName = context.PackageName; // getPackageName() from Context or Activity object
			try
			{
				context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("market://details?id=" + appPackageName)));
			}
			catch (ActivityNotFoundException)
			{
				context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName)));
			}
		}

		public void ShowDetails(object contentId = null)
		{
			var context = Dependency.Resolve<Context>();

			try
			{
				context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("market://details?id=" + contentId)));
			}
			catch (ActivityNotFoundException)
			{
				context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=" + contentId)));
			}
		}
	}
}
#endif
