using System.ComponentModel;
using Calcium.Concurrency;

namespace Calcium
{
	/// <summary>
	/// This class is not intended for public consumption.
	/// It is public because of code signing and constraints
	/// caused by the Xamarin platforms lack of code signing.
	/// This class provides convenient access to the registered
	/// <see cref="ISynchronizationContext"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class UIContext
    {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetTestContext(ISynchronizationContext context)
		{
			Nested.instance.synchronizationContextUseProperty = context;
		}
		
		#region Singleton implementation

		UIContext()
	    {
	    }
		
	    public static ISynchronizationContext Instance => Nested.instance.SynchronizationContext;
		
		ISynchronizationContext synchronizationContextUseProperty;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public ISynchronizationContext SynchronizationContext
		{
			get => synchronizationContextUseProperty
						?? (synchronizationContextUseProperty
							= Dependency.Resolve<ISynchronizationContext>());
			set => synchronizationContextUseProperty = value;
		}

		class Nested
	    {
		    // Explicit static constructor to tell C# compiler
		    // not to mark type as beforefieldinit
		    static Nested()
		    {
		    }

			internal static readonly UIContext instance = new UIContext();
	    }

	    #endregion
    }
}
