namespace Codon.SettingsModel
{
	/// <summary>
	/// Indicates the result of SetSetting call in the <see cref="Services.ISettingsService"/>.
	/// </summary>
	public enum SetSettingResult
	{
		/// <summary>
		/// The setting was changed.
		/// </summary>
		Successful,
		/// <summary>
		/// Occurs when a subscriber to the SettingChangedEvent cancels the change.
		/// </summary>
		Cancelled
	}
}