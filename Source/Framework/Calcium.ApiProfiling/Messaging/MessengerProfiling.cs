using Codon.ApiProfiling;

namespace Codon.Messaging
{
    class MessengerProfiling : IProfilable
	{
		public ProfileResult Profile()
		{
			ProfileResult result = new ProfileResult
			{
				Name = nameof(Messenger)
			};

			return result;
		}
	}
}