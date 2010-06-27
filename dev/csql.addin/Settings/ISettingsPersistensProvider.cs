
namespace csql.addin.Settings
{
	/// <summary>
	/// Interface for loading and storing settings.
	/// </summary>
	internal interface ISettingsPersistensProvider
	{
		/// <summary>
		/// Gets the location url of the settings store.
		/// </summary>
		string Url { get; }

		/// <summary>
		/// Check if the settings can be provided e.g. check if 
		/// the persitens file exists.
		/// </summary>
		bool CanLoadSettings { get; }

		/// <summary>
		/// Loads the settings from the persistens store.
		/// </summary>
		/// <returns>Loaded settings.</returns>
		SettingsCollection LoadSettings();

		/// <summary>
		/// Saves the settings.
		/// </summary>
		/// <param name="settings">A collection of settings.</param>
		void SaveSettings( SettingsCollection settings );
	}
}
