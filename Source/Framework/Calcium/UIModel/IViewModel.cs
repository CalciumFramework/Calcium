namespace Calcium.UIModel
{
	/// <summary>
	/// The basic code contract for view-model.
	/// </summary>
	public interface IViewModel
	{
		/// <summary>
		/// Performs any activities required to disconnect
		/// itself from the framework infrastructure, 
		/// such as unsubscribing from the <c>IMessenger</c>.
		/// </summary>
		void CleanUp();
	}
}
