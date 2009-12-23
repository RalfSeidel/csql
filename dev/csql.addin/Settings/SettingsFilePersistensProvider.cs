using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace csql.addin.Settings
{
	/// <summary>
	/// Implementation of a setting persistens provider to load and save
	/// the setting to a file.
	/// </summary>
	public class SettingsFilePersistensProvider : ISettingsPersistensProvider
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
				var assembly = System.Reflection.Assembly.GetExecutingAssembly();
				var assemblyDirectory = System.IO.Path.GetDirectoryName( assembly.Location );
				var settingsPath = System.IO.Path.Combine( assemblyDirectory, "csql.addin.settings.xml" );
				return settingsPath;
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
		}

		/// <summary>
		/// Initializes the provider with an explicite path.
		/// </summary>
		/// <remarks>
		/// This constructor is currently only used by the tests.
		/// </remarks>
		/// <param name="settingsPath">The settings file path.</param>
		public SettingsFilePersistensProvider( string settingsPath )
		{
			this.settingsPath = settingsPath;
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
			using ( TextWriter textWriter = new StreamWriter( this.settingsPath ) ) {
				XmlSerializer xmlSerializer = new XmlSerializer( settings.GetType() );
				xmlSerializer.Serialize( textWriter, settings );
				textWriter.Close();
			}
		}

		#endregion
	}
}
