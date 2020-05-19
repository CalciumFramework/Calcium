using Codon.ApiProfiling;

namespace Codon.SettingsModel
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