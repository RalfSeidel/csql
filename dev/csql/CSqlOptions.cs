using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Security.Permissions;

namespace csql
{
	/// <summary>
	/// Options for the script processor.
	/// </summary>
	public class CSqlOptions
	{
		#region Data member

		/// <summary>
		/// Data member for the <see cref="P:Verbosity"/> property.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly VerbositySwitch verbosity;

		/// <summary>
		/// Data member for the <see cref="P:PreprocessorOptions"/> property.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private SqtppOptions preprocessorOptions;

		#endregion

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
		/// Option to suppress the starup logo.
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
		/// The id of the target database system.
		/// </summary>
		/// <value>The database system.</value>
		public DbSystem DbSystem { get; set; }

		/// <summary>
		/// The id of the driver/provider to use for the database connection.
		/// </summary>
		/// <value>The db driver.</value>
		public DbDriver DbDriver { get; set; }

		/// <summary>
		/// The name of the database server / data source.
		/// </summary>
		/// <value>The db server.</value>
		public string DbServer { get; set; }

		/// <summary>
		/// The TCP port to use for the database server connection.
		/// </summary>
		/// <value>The db server port.</value>
		public int DbServerPort { get; set; }

		/// <summary>
		/// The name of the initial database/catalog.
		/// </summary>
		/// <value>The db database.</value>
		public string DbDatabase { get; set; }

		/// <summary>
		/// The name of the user for the database authentication. If the 
		/// user name is empty the program will try to use windows 
		/// integrated security.
		/// </summary>
		/// <value>The name of the database login.</value>
		public string DbUser { get; set; }

		/// <summary>
		/// The password for the database authentication.
		/// </summary>
		/// <value>The db password.</value>
		public string DbPassword { get; set; }

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
		/// Initializes a new instance of the <see cref="CsqlOptions"/> class.
		/// </summary>
		[SecurityPermission( SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode )]
		public CSqlOptions()
		{
			DistributionFile = "";
			TempFile = "";
			ScriptFile = "";
			BreakOnError = true;
			UsePreprocessor = true;
			DbSystem = DbSystem.MsSql;
			DbDriver = DbDriver.Default;
			DbServer = "";
			DbDatabase = "";
			DbUser = "";
			DbPassword = "";

			this.preprocessorOptions = new SqtppOptions();
			InitPreprocessorMacros( preprocessorOptions.MacroDefinitions );
			this.verbosity = new VerbositySwitch();
			verbosity.Level = System.Diagnostics.TraceLevel.Info;
		}


		public void InitPreprocessorMacros( IDictionary<string, string> macros )
		{
			var assembly = Assembly.GetExecutingAssembly();
			var assemblyName = assembly.GetName();
			var assemblyVersion = assemblyName.Version;
			var version = String.Format( CultureInfo.InvariantCulture, "{0:d2}.{1:d2}", assemblyVersion.Major, assemblyVersion.Minor );

			macros.Add( "_CSQL_SYSTEM_", this.DbSystem.ToString().ToUpper() );

			if ( !String.IsNullOrEmpty( this.DbServer ) ) {
				macros.Add( "_CSQL_SERVER=", this.DbServer );
			}
			if ( !String.IsNullOrEmpty( this.DbDatabase ) ) {
				macros.Add( "_CSQL_DATABASE=", this.DbDatabase );
			}
			if ( !String.IsNullOrEmpty( this.DbUser ) ) {
				macros.Add( "_CSQL_USER=", this.DbUser );
			}
			if ( !String.IsNullOrEmpty( version ) ) {
				macros.Add( "_CSQL_VERSION=", version );
			}
		}
	}
}
