#if __Android__
using System.IO;

using Android.Content;

namespace Codon.LauncherModel.Launchers
{
	public class PhotoResult : PhotoResultBase
	{
		public override Stream ChosenPhoto
		{
			get
			{
				var context = Dependency.Resolve<Context>();
				
				Stream stream = context.ContentResolver.OpenInputStream(Uri);
				return stream;
			}
			set
			{
				
			}
		}
	}
}
#endif