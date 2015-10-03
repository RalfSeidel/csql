using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using csql.addin.Settings.Gui;

namespace csql.addin.Settings
{
	/// <summary>
	/// An item of the settings collection i.e. one of the settings the user has created
	/// and is able to select as the active csql configuration.
	/// </summary>
	[DefaultProperty( "Name" )]
	[Serializable]
	[SuppressMessage( "Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix", Justification = "The letter C is not a prefix but part of the product name." )]
	[XmlRoot( "CSqlParameter" )]
	public class ScriptParameter : INotifyPropertyChanged
	{
		#region Data fields

		private const int DefaultResultColumnWidth = 80;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private const string StringCollectionEditorTypeReferences = @"System.Windows.Forms.Design.StringCollectionEditor,  System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly List<string> includeDirectories = new List<string>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly List<string> scriptExtensions = new List<string>();

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
		public ScriptParameter()
		{
			Name = "New";
			Verbosity = TraceLevel.Info;
			AdvancedPreprocessorParameter = String.Empty;

			OutputFile = String.Empty;
			MaxResultColumnWidth = DefaultResultColumnWidth;
			IsOutputFileEnabled = false;
			IsPreprocessorEnabled = true;
			IsBreakOnErrorEnabled = true;
			TemporaryFile = String.Empty;
			IsTemporaryFileEnabled = false;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="that">The parameter to copy.</param>
		public ScriptParameter( ScriptParameter that )
			: this()
		{
			this.Name = that.Name;
			this.Verbosity = that.Verbosity;
			this.MaxResultColumnWidth = that.MaxResultColumnWidth;

			this.IncludeDirectories.Clear();
			this.IncludeDirectories.AddRange( that.IncludeDirectories );

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

		internal static ScriptParameter CreateDefaultInstance()
		{
			var result = new ScriptParameter();
			result.Name = "Default";

			return result;
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;


		[Category( "CSql" )]
		[DefaultValue( "Default" )]
		[DisplayName( "Configuration Name" )]
		[Description( "The name of the script parameter configuration" )]
		public string Name
		{
			get { return this.name; }
			set
			{
				if ( string.Equals( this.name, value ) )
					return;

				this.name = value;
				RaisePropertyChanged();
			}
		}

		[Category( "CSql" )]
		[DefaultValue( false )]
		[DisplayName( "Distribution File Enabled" )]
		[Description( "Enabled or disables the creating of a single SQL script instead of executing the statements directly." )]
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

				if ( string.Equals( this.outputFile, value ) )
					return;

				this.outputFile = value;
				RaisePropertyChanged();
			}
		}

		[Category( "CSql" )]
		[DisplayName( "Script Extensions" )]
		[Description( "The file extensions which are considered to be csql scripts. The execute script command is only enabled for such files." )]
		[Editor( StringCollectionEditorTypeReferences, typeof( System.Drawing.Design.UITypeEditor ) )]
		[TypeConverter( typeof( PathCollectionConverter ) )]
		[SuppressMessage( "Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Class is serializable. Serializer doesn't work with abtract classes or interfaces." )]
		[SuppressMessage( "Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Class is serializable. Serializer doesn't work with abtract classes or interfaces." )]
		[DefaultValue( ".csql;.sql;.ins" )]
		public List<string> ScriptExtensions
		{
			get { return this.scriptExtensions; }
			set
			{
				if ( value == this.scriptExtensions )
					return;

				if ( value == null )
					throw new ArgumentNullException( "value" );

				this.scriptExtensions.Clear();
				this.scriptExtensions.AddRange( value );
				RaisePropertyChanged();
			}
		}

		[Category( "CSql" )]
		[DefaultValue( true )]
		[DisplayName( "Stop on First Error" )]
		[Description( "Option to stop when the script execution encountered an error. If false the execution will continue until the script has finished." )]
		public bool IsBreakOnErrorEnabled { get; set; }

		[Category( "CSql" )]
		[DefaultValue( TraceLevel.Info )]
		[DisplayName( "Verbosity" )]
		[Description( "The verbosity of the trace output of this addin." )]
		public TraceLevel Verbosity { get; set; }

		[Category( "CSql" )]
		[DisplayName( "Max Result Column Width" )]
		[Description( "The maximal width of a single result column when traceing query results." )]
		[DefaultValue( DefaultResultColumnWidth )]
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
				RaisePropertyChanged();
			}
		}

		[Category( "Preprocessor" )]
		[DefaultValue( true )]
		[DisplayName( "Enable Preprocessing" )]
		public bool IsPreprocessorEnabled { get; set; }

		[Category( "Preprocessor" )]
		[DisplayName( "Include Directories" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( StringCollectionEditorTypeReferences, typeof( System.Drawing.Design.UITypeEditor ) )]
		[TypeConverter( typeof( PathCollectionConverter ) )]
		[SuppressMessage( "Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Class is serializable. Serializer doesn't work with abtract classes or interfaces." )]
		[SuppressMessage( "Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Class is serializable. Serializer doesn't work with abtract classes or interfaces." )]
		public List<string> IncludeDirectories
		{
			get { return this.includeDirectories; }
			set
			{
				if ( Object.Equals( this.includeDirectories, value ) )
					return;

				this.includeDirectories.Clear();
				foreach ( var directory in value ) {
					if ( !String.IsNullOrEmpty( directory ) )
						this.includeDirectories.Add( directory );
				}
				RaisePropertyChanged();
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
			get { return this.preprocessorDefinitions; }
			set
			{
				if ( Object.Equals( this.preprocessorDefinitions, value ) )
					return;

				this.preprocessorDefinitions.Clear();
				foreach ( var pd in value ) {
					if ( !String.IsNullOrEmpty( pd.Name ) )
						this.preprocessorDefinitions.Add( pd );
				}
				RaisePropertyChanged();
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
					value = string.Empty;

				if ( String.Equals( this.temporaryFile, value ) )
					return;

				this.temporaryFile = value;
				RaisePropertyChanged();
			}
		}

		[Category( "Preprocessor" )]
		[DisplayName( "Advanced Preprocess Parameter" )]
		[DefaultValue( "" )]
		public string AdvancedPreprocessorParameter { get; set; }

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		[SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "The method is a wrapper to raise the event." )]
		protected internal void RaisePropertyChanged( [CallerMemberName] string propertyName = null )
		{
			if ( PropertyChanged != null ) {
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}
	}
}
