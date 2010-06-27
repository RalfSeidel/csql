using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using csql.addin.Settings.Gui;
using System;
using System.Reflection;

namespace csql.addin.Settings
{
	/// <summary>
	/// An item of the settings collection i.e. one of the settings the user has created
	/// and is able to select as the active csql configuration.
	/// </summary>
	[DefaultProperty("Name")]
	[Serializable]
	public class CSqlParameter : INotifyPropertyChanged
	{
		#region Data fields

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string name;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string outputFile;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string temporaryFile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<string> includeDirectories = new List<string>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly List<PreprocessorDefinition> preprocessorDefinitions = new List<PreprocessorDefinition>();

		#endregion

		[Category( "CSql" )]
		[DefaultValue( "New" )]
		[DisplayName( "Configuration Name" )]
		public string Name 
		{
			get
			{
				return this.name;
			}
			set
			{
				if ( String.Equals( this.name, value ) )
					return;

				this.name = value;
				OnPropertyChanged("Name");
			}
		}

		[Category( "CSql" )]
		[DefaultValue( false )]
		[DisplayName( "Distribution File Enabled" )]
		public bool IsOutputFileEnabled { get; set; }

		[Category( "CSql" )]
		[DefaultValue( "" )]
		[DisplayName( "Distribution File" )]
		[Editor( typeof( OutputFileEditor ), typeof( UITypeEditor ) )]
		public string OutputFile 
		{ 
			get
			{
				return this.outputFile;
			}
			set
			{
				if ( value == null )
					value = "";

				if ( String.Equals( this.outputFile, value ) )
					return;

				this.outputFile = value;
				OnPropertyChanged("OutputFile");
			}
		}

		[Category( "CSql" )]
		[DefaultValue( true )]
		[DisplayName("Stop on First Error")]
		public bool IsBreakOnErrosEnabled { get; set; }

		[Category( "CSql" )]
		[DefaultValue( TraceLevel.Info )]
		[DisplayName("Verbosity")]
		public TraceLevel Verbosity { get; set; }

		[Category( "Preprocessor" )]
		[DefaultValue( true )]
		[DisplayName( "Enable Preprocessing" )]
		public bool IsPreprocessorEnabled { get; set; }

		[Category( "Preprocessor" )]
		[DisplayName( "Include Directories" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( @"System.Windows.Forms.Design.StringCollectionEditor,  System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof( System.Drawing.Design.UITypeEditor ) )]
		[TypeConverter(typeof(PathCollectionConverter))]
		public List<string> IncludeDirectories 
		{ 
			get { return includeDirectories; }
			set 
			{ 
				if ( Object.Equals( this.includeDirectories, value ) )
					return;

				this.includeDirectories.Clear();
				foreach ( var directory in value ) {
					if ( !String.IsNullOrEmpty( directory  ) ) 
						this.includeDirectories.Add( directory ); 
				}
				OnPropertyChanged( "IncludeDirectories" );
			}
		}

		[Category( "Preprocessor" )]
		[DisplayName( "Preprocess Definitions" )]
		[Editor( typeof( PreprocessorDefinitionEditor ), typeof( UITypeEditor ) )]
		[TypeConverter( typeof( PreprocessorDefinitionCollectionConverter ) )]
		public List<PreprocessorDefinition> PreprocessorDefinitions
		{
			get 
			{ 
				return this.preprocessorDefinitions; 
			}
			set
			{
				if ( Object.Equals( this.preprocessorDefinitions, value ) )
					return;

				this.preprocessorDefinitions.Clear();
				foreach ( var pd in value ) {
					if ( !String.IsNullOrEmpty( pd.Name ) )
						this.preprocessorDefinitions.Add( pd );
				}
				OnPropertyChanged( "PreprocessorDefinitions" );
			}
		}

		[Category( "Preprocessor" )]
		[DefaultValue( false )]
		[DisplayName( "Temp File Enabled" )]
		public bool IsTemporaryFileEnabled { get; set; }

		[Category( "Preprocessor" )]
		[DisplayName( "Temp File" )]
		[DefaultValue( "" )]
		[Editor( typeof( OutputFileEditor ), typeof( UITypeEditor ) )]
		public string TemporaryFile
		{
			get
			{
				return this.temporaryFile;
			}
			set
			{
				if ( value == null )
					value = "";

				if ( String.Equals( this.temporaryFile, value ) )
					return;

				this.temporaryFile = value;
				OnPropertyChanged( "TemporaryFile" );
			}
		}
		[Category( "Preprocessor" )]
		[DisplayName( "Advanced Preprocess Parameter" )]
		[DefaultValue( "" )]
		public string AdvancedPreprocessorParameter { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public CSqlParameter()
		{
			this.name = "New";
			this.Verbosity = TraceLevel.Info;
			this.AdvancedPreprocessorParameter = "";

			this.outputFile = "";
			IsOutputFileEnabled = false;
			IsPreprocessorEnabled = true;
			IsBreakOnErrosEnabled = true;
			TemporaryFile = "";
			IsTemporaryFileEnabled = false;
		}

		/// <summary>
		/// Copy Construktor
		/// </summary>
		/// <param name="settingsObject">The settings object to copy.</param>
		public CSqlParameter( CSqlParameter settingsObject )
			: this()
		{
			this.Name   = settingsObject.Name + " (Copy)";
			this.Verbosity = Verbosity;

			this.IncludeDirectories.Clear();
			foreach ( string item in settingsObject.IncludeDirectories )
				this.IncludeDirectories.Add( item );

			this.preprocessorDefinitions.Clear();
			foreach ( PreprocessorDefinition def in settingsObject.PreprocessorDefinitions ) {
				PreprocessorDefinition defCopy = new PreprocessorDefinition( def );
				this.preprocessorDefinitions.Add( defCopy );
			}

			this.AdvancedPreprocessorParameter = settingsObject.AdvancedPreprocessorParameter;
			this.OutputFile = settingsObject.OutputFile;
			this.IsOutputFileEnabled = settingsObject.IsOutputFileEnabled;
			this.IsPreprocessorEnabled = settingsObject.IsPreprocessorEnabled;
			this.IsBreakOnErrosEnabled = settingsObject.IsBreakOnErrosEnabled;
			this.TemporaryFile = settingsObject.TemporaryFile;
			this.IsTemporaryFileEnabled = settingsObject.IsTemporaryFileEnabled;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		protected internal void OnPropertyChanged( string propertyName )
		{
			if ( PropertyChanged != null ) {
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		#endregion
	}
}
