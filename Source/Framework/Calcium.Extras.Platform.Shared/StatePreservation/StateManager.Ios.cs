#if __IOS__

namespace Codon.StatePreservation
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
