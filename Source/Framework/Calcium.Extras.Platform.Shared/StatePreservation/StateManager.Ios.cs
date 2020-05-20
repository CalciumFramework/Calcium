#if __IOS__

namespace Calcium.StatePreservation
{
    public class StateManager : IStateManager
    {
	    public void Initialize()
	    {
		    
	    }

	    public bool ShouldLoadTransientState { get; }
    }
}

#endif
