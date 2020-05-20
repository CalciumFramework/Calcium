using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Calcium;

[assembly: AssemblyTitle("Calcium.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Calcium.Tests")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("7b912463-2bb6-416f-a5b7-8772342cc15b")]

/* Assembly uses wildcard scheme to support 
 * the AssemblyBuildTimeTests class. */
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly:InternalsVisibleTo(nameof(Calcium) + ".Extras.Tests, PublicKey=" + AssemblyConstants.PublicKey)]
