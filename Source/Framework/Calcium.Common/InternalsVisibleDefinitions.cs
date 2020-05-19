using System.Runtime.CompilerServices;

#pragma warning disable CS0436 // Type conflicts with imported type

/* The purpose of the following InternalsVisibleTo attributes 
 * is to make all framework library internals visible to one another. */
#if __ANDROID__ || __IOS__
[assembly: InternalsVisibleTo(nameof(Codon) + ".Tests")]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Extras.Tests")]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Platform")]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Extras.Platform")]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Extras.Platform.Tests")]
#else
[assembly: InternalsVisibleTo(nameof(Codon) + ".Tests" + Tokens.PublicKeySuffix)]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Extras.Tests" + Tokens.PublicKeySuffix)]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Platform" + Tokens.PublicKeySuffix)]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Extras" + Tokens.PublicKeySuffix)]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Extras.Platform" + Tokens.PublicKeySuffix)]
[assembly: InternalsVisibleTo(nameof(Codon) + ".Extras.Platform.Tests" + Tokens.PublicKeySuffix)]
[assembly: InternalsVisibleTo(nameof(Codon) + ".UI.Data" + Tokens.PublicKeySuffix)]
[assembly: InternalsVisibleTo(nameof(Codon) + ".UndoModel" + Tokens.PublicKeySuffix)]
#endif

#pragma warning restore CS0436 // Type conflicts with imported type

class Tokens
{
	internal const string PublicKey = "0024000004800000940000000602000000240000525341310004000001000100e57a0f3c7c2a447fe73ffa3a7a6bcfd816816fd09fd4cdbb8f275bc7a0228f39de5c27340f214c362cc0e1e8690936f72e4dd7404826d4ab2c4cf4be00977a3a5ed65629b7044fa5598be87d3d57c3a05942d5b969794786949ca3f3e845117b20a246ca4fb52450e57f02d2c8634689e1f3df333fc33d1e28ea07541633f598";
	internal const string PublicKeySuffix = ", PublicKey=" + PublicKey;
} 