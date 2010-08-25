using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using csql.addin.Settings.Gui;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace csql.addin.Settings
{
	/// <summary>
	/// An item of the settings collection i.e. one of the settings the user has created
	/// and is able to select as the active csql configuration.
	/// </summary>
	[DefaultProperty("Name")]
	[Serializable]
	[SuppressMessage( "Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix", Justification="The letter C is not a prefix but part of the product name." )]
	public class CSqlParameter : INotifyPropertyChanged
	{
		#region Data fields

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly List<string> includeDirectories = new List<string>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly List<PreprocessorDefinition> preprocessorDefinitions = new List<PreprocessorDefinition>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string name;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string outputFile;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string temporaryFile;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private int maxResultColumnWidth;

		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public CSqlParameter()
		{
			this.name = "New";
			this.Verbosity = TraceLevel.Info;
			this.AdvancedPreprocessorParameter = String.Empty;

			this.outputFile = String.Empty;
			this.maxResultColumnWidth = 40;
			IsOutputFileEnabled = false;
			IsPreprocessorEnabled = true;
			IsBreakOnErrorEnabled = true;
			TemporaryFile = String.Empty;
			IsTemporaryFileEnabled = false;
		}

		/// <summary>
		/// Copy Construktor
		/// </summary>
		/// <param name="that">The parameter to copy.</param>
		public CSqlParameter( CSqlParameter that )
			: this()
		{
			this.Name = that.Name + " (Copy)";
			this.Verbosity = that.Verbosity;
			this.maxResultColumnWidth = that.maxResultColumnWidth;

			this.IncludeDirectories.Clear();
			foreach ( string item in that.IncludeDirectories )
				this.IncludeDirectories.Add( item );

			this.preprocessorDefinitions.Clear();
			foreach ( PreprocessorDefinition def in that.PreprocessorDefinitions ) {
				PreprocessorDefinition defCopy = new PreprocessorDefinition( def );
				this.preprocessorDefinitions.Add( defCopy );
			}

			this.AdvancedPreprocessorParameter = that.AdvancedPreprocessorParameter;
			this.OutputFile = that.OutputFile;
			this.IsOutputFileEnabled = that.IsOutputFileEnabled;
			this.IsPreprocessorEnabled = that.IsPreprocessorEnabled;
			this.IsBreakOnErrorEnabled = that.IsBreakOnErrorEnabled;
			this.TemporaryFile = that.TemporaryFile;
			this.IsTemporaryFileEnabled = that.IsTemporaryFileEnabled;
		}

		public event PropertyChangedEventHandler PropertyChanged;


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
			get { return this.outputFile; }
			set
			{
				if ( value == null )
					value = String.Empty;

				if ( String.Equals( this.outputFile, value ) )
					return;

				this.outputFile = value;
				OnPropertyChanged("OutputFile");
			}
		}

		[Category( "CSql" )]
		[DefaultValue( true )]
		[DisplayName("Stop on First Error")]
		public bool IsBreakOnErrorEnabled { get; set; }

		[Category( "CSql" )]
		[DefaultValue( TraceLevel.Info )]
		[DisplayName("Verbosity")]
		public TraceLevel Verbosity { get; set; }

		[Category( "CSql" )]
		[DisplayName( "Max Result Column Width" )]
		[Description( "The maximal width of a single result column when traceing query results." )]
		[DefaultValue(40)]
		public int MaxResultColumnWidth
		{
			get { return this.maxResultColumnWidth; }
			set
			{
				if ( value == this.maxResultColumnWidth )
					return;

				if ( value <= 0 ) {
					throw new ArgumentOutOfRangeException( "value", "The maximal length of a result column has to be positive" );
				}

				this.maxResultColumnWidth = value;
				OnPropertyChanged( "MaxResultColumnWidth" );
			}
		}

		[Category( "Preprocessor" )]
		[DefaultValue( true )]
		[DisplayName( "Enable Preprocessing" )]
		public bool IsPreprocessorEnabled { get; set; }

		[Category( "Preprocessor" )]
		[DisplayName( "Include Directories" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( @"System.Windows.Forms.Design.StringCollectionEditor,  System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof( System.Drawing.Design.UITypeEditor ) )]
		[TypeConverter(typeof(PathCollectionConverter))]
		[SuppressMessage( "Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Class is serializable. Serializer doesn't work with abtract classes or interfaces." )]
		[SuppressMessage( "Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Class is serializable. Serializer doesn't work with abtract classes or interfaces." )]
		public List<string> IncludeDirectories 
		{ 
			get { return includeDirectories; }
			set 
			{ 
				if ( Object.Equals( this.includeDirectories, value ) )
					return;

				this.includeDirectories.Clear();
				foreach ( var directory in value ) {
					if ( !String.IsNullOrEmpty( directory ) ) 
						this.includeDirectories.Add( directory ); 
				}
				OnPropertyChanged( "IncludeDirectories" );
			}
		}

		[Category( "Preprocessor" )]
		[DisplayName( "Preprocess Definitions" )]
		[Editor( typeof( PreprocessorDefinitionEditor ), typeof( UITypeEditor ) )]
		[TypeConverter( typeof( PreprocessorDefinitionCollectionConverter ) )]
		[SuppressMessage( "Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Class is serializable. Serializer doesn't work with abtract classes or interfaces." )]
		[SuppressMessage( "Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Class is serializable. Serializer doesn't work with abtract classes or interfaces." )]
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
			get { return this.temporaryFile; }
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

		#region INotifyPropertyChanged Members

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
