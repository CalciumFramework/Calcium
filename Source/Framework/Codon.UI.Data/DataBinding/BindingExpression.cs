#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

namespace Codon.UI.Data
{
	public class BindingExpression
	{
		public string Path { get; set; }
		public string Source { get; set; }
		public string Target { get; set; }
		public string Converter { get; set; }
		public string ConverterParameter { get; set; }
		public BindingMode Mode { get; set; }
		public string ViewValueChangedEvent { get; set; }

#if MONODROID || __ANDROID__
		internal Android.Views.View View { get; set; }
#else
		/// <summary>
		/// Not for public use. Should be internal.
		/// The assembly's strong name, however, prevents Xamarin 
		/// based libraries, namely Codon.Extras.Platform.Android, 
		/// from opening up visibility with the InternalsVisibleTo. 
		/// Xamarin based libraries can't be strong named, 
		/// and InternalsVisibleTo requires a strong named assembly 
		/// name if the assembly where the attribute is declared 
		/// is strong named.
		/// </summary>
		public object View { get; set; }
#endif
	}
}