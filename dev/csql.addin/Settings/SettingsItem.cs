using System.Collections.Generic;

namespace csql.addin.Settings
{
	/// <summary>
	/// An item of the settings collection i.e. one of the settings the user has created
	/// and is able to select as the active csql configuration.
	/// </summary>
	public class SettingsItem
	{
		public string Description { get; set; }
		public int VerbosityLevel { get; set; }

		public string DbUsername { get; set; }
		public string DbPassword { get; set; }
		public string DbServer { get; set; }
		public string DbDatabase { get; set; }
		public bool DbUseIs { get; set; }

		public List<string> IncludeFolders { get; set; }
		public List<PreprocessorDefinition> PreprocessorDefinitions { get; set; }
		public string PreprocessorAdvancedDefinitions { get; set; }

		public string Distributionfile { get; set; }
		public bool IsDistributionfileEnabled { get; set; }
		public bool IsPreprocessorEnabled { get; set; }
		public bool IsBreakOnErrosEnabled { get; set; }

		public string TemporaryOutputFile { get; set; }
		public bool IsTemporaryOutputFileEnabled { get; set; }

		public SettingsItem()
		{
			this.Description = "New";
			this.VerbosityLevel = 0;

			this.IncludeFolders = new List<string>();
			this.PreprocessorDefinitions = GetDefaultPreprocessorDefinitions();
			this.PreprocessorAdvancedDefinitions = "";


			this.DbUsername = "";
			this.DbDatabase = "";
			this.DbPassword = "";
			this.DbServer = "";
			this.DbUseIs = false;

			Distributionfile = "";
			IsDistributionfileEnabled = false;
			IsPreprocessorEnabled = true;
			IsBreakOnErrosEnabled = false;
			TemporaryOutputFile = "";
			IsTemporaryOutputFileEnabled = false;
		}

		/// <summary>
		/// Copy Construktor
		/// </summary>
		/// <param name="settingsObject">The settings object to copy.</param>
		public SettingsItem( SettingsItem settingsObject )
			: this()
		{
			this.Description    = settingsObject.Description + " (Copy)";
			this.VerbosityLevel = VerbosityLevel;

			this.DbUsername = settingsObject.DbUsername;
			this.DbPassword = settingsObject.DbPassword;
			this.DbServer = settingsObject.DbServer;
			this.DbDatabase = settingsObject.DbDatabase;
			this.DbUseIs = settingsObject.DbUseIs;

			foreach ( string item in settingsObject.IncludeFolders )
				this.IncludeFolders.Add( item );

			foreach ( PreprocessorDefinition item in settingsObject.PreprocessorDefinitions )
				this.PreprocessorDefinitions.Add( new PreprocessorDefinition( item ) );

			this.PreprocessorAdvancedDefinitions = settingsObject.PreprocessorAdvancedDefinitions;


			this.Distributionfile = settingsObject.Distributionfile;
			this.IsDistributionfileEnabled = settingsObject.IsDistributionfileEnabled;
			this.IsPreprocessorEnabled = settingsObject.IsPreprocessorEnabled;
			this.IsBreakOnErrosEnabled = settingsObject.IsBreakOnErrosEnabled;
			this.TemporaryOutputFile = settingsObject.TemporaryOutputFile;
			this.IsTemporaryOutputFileEnabled = settingsObject.IsTemporaryOutputFileEnabled;

		}

		private List<PreprocessorDefinition> GetDefaultPreprocessorDefinitions()
		{
			List<PreprocessorDefinition> result = new List<PreprocessorDefinition>();

			PreprocessorDefinition def;

			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_DROP_CREATE" };
			result.Add( def );
			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_CREATE_SCHEMAS" };
			result.Add( def );
			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_CREATE_TABLES" };
			result.Add( def );
			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_CREATE_PRIMARY_KEYS" };
			result.Add( def );
			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_CREATE_FOREIGN_KEYS" };
			result.Add( def );
			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_CREATE_FUNCTIONS" };
			result.Add( def );
			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_CREATE_PROCEDURES" };
			result.Add( def );
			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_CREATE_VIEWS" };
			result.Add( def );
			def = new PreprocessorDefinition() { IsEnabled = false, Key = "CSQL_CREATE_SYNONYMS" };
			result.Add( def );
			

			return result;
		}
	}
}
