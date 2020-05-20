namespace Calcium.LauncherModel.Launchers
{
	/// <summary>
	/// This enum indicates the result 
	/// of an <see cref="ILauncher{T}"/>;
	/// in particular it indicates if the user cancelled
	/// or dismissed the operation, or if the operation
	/// completed successfully.
	/// </summary>
	public enum LauncherResult
	{
		/// <summary>
		/// No result. The user may have dismissed
		/// the operation.
		/// </summary>
		None,
		/// <summary>
		/// The launcher completed.
		/// </summary>
		OK,
		/// <summary>
		/// The operation was cancelled, 
		/// either by the user or the operating system.
		/// </summary>
		Cancel
	}
}
