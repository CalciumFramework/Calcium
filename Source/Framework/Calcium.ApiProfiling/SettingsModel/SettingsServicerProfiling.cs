using Calcium.ApiProfiling;

namespace Calcium.SettingsModel
{
    class SettingsServiceProfiling : IProfilable
	{
		public ProfileResult Profile()
		{
			ProfileResult result = new ProfileResult
			{
				Name = nameof(SettingsService)
			};

			return result;
		}
	}
}
