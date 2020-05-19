using Calcium.InversionOfControl;

namespace Calcium.StatePreservation
{
	/// <summary>
	/// This interface is the code contract for a class 
	/// that monitors application life-cycle 
	/// and page navigation events. When such an event occurs,
	/// the state manager identifies the active <see cref="IStateful"/>
	/// instances and instructs them to save or restore state.
	/// For example, if the application navigates
	/// away from a page and that page's view-model
	/// implements <see cref="IStateful"/>, then
	/// the state manager informs that object that is should
	/// save its state. Conversely, if the app navigates to
	/// a page whose view-model implements <see cref="IStateful"/>
	/// then the state manager provides the view-model
	/// with an opportunity to restore its state.
	/// </summary>
	[DefaultTypeName(AssemblyConstants.Namespace + "." 
		+ nameof(ComponentModel) + "." + nameof(StatePreservation)
		+ AssemblyConstants.ExtrasPlatformAssembly, Singleton = true)]
	public interface IStateManager
	{
		/// <summary>
		/// Call this once to have the state manager begin 
		/// monitoring for navigation and application life-cycle events.
		/// </summary>
		void Initialize();

		bool ShouldLoadTransientState
		{
			get;
		}
	}
}
