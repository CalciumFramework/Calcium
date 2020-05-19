using System;

using Codon.MissingTypes.System.ComponentModel;

namespace Codon.Navigation
{
	/// <summary>
	/// Arguments used in conjunction with the 
	/// <see cref="Navigation.INavigationAware.HandleNavigatingFrom"/> event.
	/// </summary>
	public class NavigatingFromEventArgs : CancelEventArgs
	{
		public NavigationType NavigationType { get; private set; }

		/// <summary>
		/// The destination <c>Uri</c>.
		/// </summary>
		public Uri Uri { get; private set; }

		public NavigatingFromEventArgs(
			NavigationType navigationType, Uri uri)
		{
			NavigationType = navigationType;
			Uri = uri;
		}		
	}
}