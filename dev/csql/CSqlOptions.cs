using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using Sqt.DbcProvider;

namespace csql
{
	/// <summary>
	/// Options for the script processor.
	/// </summary>
	[SuppressMessage( "Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CSql", Justification = "CSql is the best readable spelling for the product." )]
	[SuppressMessage( "Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix", Justification = "CSql is the best readable spelling for the product." )]
	public class CSqlOptions
	{
		#region Data member

		/// <summary>
		/// Data member for the <see cref="P:PreprocessorOptions"/> property.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private SqtppOptions preprocessorOptions;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DbConnectionParameter connectionParameter;

		/// <summary>
		/// Data member for the <see cref="P:Verbosity"/> property.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly VerbositySwitch verbosity;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="CsqlOptions"/> class.
		/// </summary>
		[SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
		public CSqlOptions()
		{
			this.connectionParameter = new DbConnectionParameter();
			this.preprocessorOptions = new SqtppOptions();
			this.verbosity = new VerbositySwitch();

			DistributionFile = String.Empty;
			TempFile = String.Empty;
			ScriptFile = String.Empty;
			BreakOnError = true;
			UsePreprocessor = true;
			MaxResultColumnWidth = 40;
		}

		/// <summary>
		/// Gets or sets the path to the main input file.
		/// </summary>
		/// <value>The main input script file to process.</value>
		public string ScriptFile { get; set; }

		/// <summary>
		/// The path of the distribution (output) file if one is to be created.
		/// </summary>
		/// <value>The distribution file path.</value>
		public string DistributionFile { get; set; }

		/// <summary>
		/// The temporary file used to store the pre processor output.
		/// If it is not specified the program will use a named pipe.
		/// </summary>
		/// <value>The temporary file path to hold the pre processor ouput.</value>
		public string TempFile { get; set; }

		/// <summary>
		/// Option to suppress the startup logo.
		/// </summary>
		/// <value><c>true</c> if the logo is not shown; <c>false</c> otherwise.</value>
		public bool NoLogo { get; set; }

		/// <summary>
		/// Option to stop script execution after the first error was raised.
		/// </summary>
		/// <value><c>true</c> if the execution has to stop after the first error raised; <c>false</c> otherwise.</value>
		public bool BreakOnError { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the input has to be processed sqtpp.
		/// </summary>
		/// <value><c>true</c> if pre processor is run before execution of the script batches (default); <c>false</c> otherwise.</value>
		public bool UsePreprocessor { get; set; }


		/// <summary>
		/// Gets or sets the maximal width of a single result column when traceing query results.
		/// </summary>
		public int MaxResultColumnWidth { get; set; }

		/// <summary>
		/// Gets or sets the parameter used to establish the database connection.
		/// </summary>
		public DbConnectionParameter ConnectionParameter
		{
			get
			{
				if ( String.IsNullOrEmpty( this.connectionParameter.ApplicationName ) ) {
					this.connectionParameter.ApplicationName = GlobalSettings.CSqlProductName;
				}
				this.connectionParameter.VerbosityLevel = GlobalSettings.Verbosity.Level;
				return connectionParameter;
			}
			set
			{
				this.connectionParameter = value;
			}
		}

		/// <summary>
		/// Options for the preprocessor sqtpp.
		/// </summary>
		public SqtppOptions PreprocessorOptions
		{
			get { return this.preprocessorOptions; }
			set { this.preprocessorOptions = value; }
		}

		/// <summary>
		/// Gets or sets the trace/verbosity setting.
		/// </summary>
		/// <value>The trace level.</value>
		public VerbositySwitch Verbosity { get { return this.verbosity; } }



		/// <summary>
		/// Flag indicating if a named pipe is used to communicate with the pre processor.
		/// </summary>
		/// <value>Flag for the named pipe usage option.</value>
		internal bool UseNamedPipes
		{
			get { return String.IsNullOrEmpty( TempFile ); }
		}



		/// <summary>
		/// Add the preprocessor definitions predefined by csql specifying certain 
		/// runtime options.
		/// </summary>
		public void AddPreprocessorMacros()
		{
			var macros = this.PreprocessorOptions.MacroDefinitions;
			var assembly = Assembly.GetExecutingAssembly();
			var assemblyName = assembly.GetName();
			var assemblyVersion = assemblyName.Version;
			var version = String.Format( CultureInfo.InvariantCulture, "{0:d2}.{1:d2}", assemblyVersion.Major, assemblyVersion.Minor );

			// Specify the database system used e.g. _CSQL_SYSTEM_MSSQL
			macros.Add( "_CSQL_SYSTEM_" + ConnectionParameter.Provider.ToString().ToUpper(), "" );

			if ( !String.IsNullOrEmpty( ConnectionParameter.DatasourceAddress ) ) {
				macros.Add( "_CSQL_SERVER", ConnectionParameter.DatasourceAddress );
			}
			if ( !String.IsNullOrEmpty( ConnectionParameter.Catalog ) ) {
				macros.Add( "_CSQL_DATABASE", ConnectionParameter.Catalog );
			}
			if ( !String.IsNullOrEmpty( ConnectionParameter.UserId ) ) {
				macros.Add( "_CSQL_USER", ConnectionParameter.UserId );
			}
			if ( !String.IsNullOrEmpty( version ) ) {
				macros.Add( "_CSQL_VERSION", version );
			}
			if ( !String.IsNullOrEmpty( DistributionFile ) ) {
				string value = DistributionFile;
				if ( value.Contains( " " ) ) {
					value = '"' + value + '"';
				}
				macros.Add( "_CSQL_OUTPUT", value );
			}
		}
	}
}
