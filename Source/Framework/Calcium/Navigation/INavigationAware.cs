namespace Calcium.Navigation
{
	/// <summary>
	/// Classes implementing this interface are
	/// able to be notified when a navigation event
	/// occurs.
	/// </summary>
	public interface INavigationAware
	{
		/// <summary>
		/// Called when the class is navigated to.
		/// </summary>
		/// <param name="e">
		/// Arguments containing information about the navigation.
		/// </param>
		void HandleNavigatedTo(NavigatedArgs e);

		/// <summary>
		/// Called when the class is navigated away from.
		/// </summary>
		/// <param name="e">
		/// Arguments containing information about the navigation.
		/// If supported may allows cancellation of the navigation.
		/// </param>
		void HandleNavigatingFrom(NavigatingArgs e);
	}
}
