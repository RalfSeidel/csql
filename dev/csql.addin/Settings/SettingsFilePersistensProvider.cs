using System;
using System.IO;
using System.Xml.Serialization;

namespace csql.addin.Settings
{
	/// <summary>
	/// Implementation of a setting persistens provider to load and save
	/// the setting to a file.
	/// </summary>
	internal class SettingsFilePersistensProvider : ISettingsPersistensProvider
	{
		private readonly string settingsPath;

		/// <summary>
		/// Gets the default path for the settings files
		/// </summary>
		/// <value>The default path.</value>
		public static string DefaultPath
		{
			get
			{
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Sql Service GmbH\csql\settings.xml";
			}
		}


		/// <summary>
		/// Initializes a new file persistens provider using 
		/// the <see cref="P:DefaultPath"/> for the the 
		/// setting file.
		/// </summary>
		/// <param name="settingsPath">The settings path.</param>
		public SettingsFilePersistensProvider( )
		{
			this.settingsPath = DefaultPath;

		    if(! File.Exists(this.settingsPath))
                SaveSettings(new SettingsCollection());
		}





		#region IStettingsPersistensProvider Members

		/// <summary>
		/// Returns the file path of the setting file.
		/// </summary>
		public string Url 
		{
			get { return this.settingsPath; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance can load settings.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can load settings; <c>false</c> otherwise.
		/// </value>
		public bool CanLoadSettings
		{
			get
			{
				if ( !File.Exists( this.settingsPath ) )
					return false;

				FileInfo fi = new System.IO.FileInfo(this.settingsPath);
				return fi.Length != 0;
			}
		}


		/// <summary>
		/// Loads the settings from the file specified in the construtor of this object.
		/// </summary>
		/// <returns>Loaded settings.</returns>
		public SettingsCollection LoadSettings()
		{
			using ( TextReader textReader = new StreamReader( this.settingsPath ) ) {
				XmlSerializer xmlSerializer = new XmlSerializer( typeof( SettingsCollection ) );
				SettingsCollection configuration = (SettingsCollection)xmlSerializer.Deserialize( textReader );
				textReader.Close();
				return configuration;
			}
		}

		/// <summary>
		/// Saves the settings in the file specified in the construtor of this object.
		/// </summary>
		/// <remarks>
		/// An existing file will be overwritten.
		/// </remarks>
		/// <param name="settings">The settings to save.</param>
		public void SaveSettings( SettingsCollection settings )
		{
            if(! Directory.Exists(Path.GetDirectoryName(this.settingsPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(this.settingsPath));

			using ( TextWriter textWriter = new StreamWriter( this.settingsPath ) ) {
				XmlSerializer xmlSerializer = new XmlSerializer( settings.GetType() );
				xmlSerializer.Serialize( textWriter, settings );
				textWriter.Close();
			}
		}

		#endregion
	}
}
