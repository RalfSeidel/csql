using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// Options for the script processor.
	/// </summary>
	public class CsqlOptions
	{
		/// <summary>
		/// Gets or sets the path to the main input file.
		/// </summary>
		/// <value>The main input script file to process.</value>
		public string ScriptFile { get; set; }

		/// <summary>
		/// The path of the distribution (output) file if one is to be created.
		/// </summary>
		/// <value>The distibution file path.</value>
		public string DistibutionFile { get; set; }

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

		public string AdditionalPreprocessorArguments { get; set; }

		/// <summary>
		/// Data member for the <see cref="P:Verbosity"/> property.
		/// </summary>
		private readonly VerbositySwitch m_verbosity;

		/// <summary>
		/// Gets or sets the trace/verbosity setting.
		/// </summary>
		/// <value>The trace level.</value>
		public VerbositySwitch Verbosity { get { return m_verbosity; } }

		/// <summary>
		/// Options for the pre processor.
		/// </summary>
		public SqtppOptions SqtppOptions { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CsqlOptions"/> class.
		/// </summary>
		public CsqlOptions()
		{
			DistibutionFile = "";
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

			AdditionalPreprocessorArguments = "";

			m_verbosity = new VerbositySwitch();
			m_verbosity.Level = System.Diagnostics.TraceLevel.Info;
		}


		public string GeneratePreprocessorArguments()
		{
			StringBuilder sb = new StringBuilder();
			string separator = " ";

			Assembly assembly = Assembly.GetExecutingAssembly();
			AssemblyName assemblyName = assembly.GetName();
			Version assemblyVersion = assemblyName.Version;
			string version = String.Format( CultureInfo.InvariantCulture, "{0:d2}.{1:d2}", assemblyVersion.Major, assemblyVersion.Minor );

			#region Database Information Generation

			sb.Append( separator );
			sb.Append( "/D_CSQL_SYSTEM_" );
			sb.Append( this.DbSystem.ToString().ToUpper() );
			separator = " ";

			if ( !String.IsNullOrEmpty( this.DbServer ) ) {
				sb.Append( separator );
				sb.Append( "/D_CSQL_SERVER=" );
				sb.Append( this.DbServer );
				separator = " ";
			}
			if ( !String.IsNullOrEmpty( this.DbDatabase ) ) {
				sb.Append( separator );
				sb.Append( "/D_CSQL_DATABASE=" );
				sb.Append( this.DbDatabase );
				separator = " ";
			}
			if ( !String.IsNullOrEmpty( this.DbUser ) ) {
				sb.Append( separator );
				sb.Append( "/D_CSQL_USER=" );
				sb.Append( this.DbUser );
				separator = " ";
			}
			if ( !String.IsNullOrEmpty( version ) ) {
				sb.Append( separator );
				sb.Append( "/D_CSQL_VERSION=" );
				sb.Append( version );
				separator = " ";
			}

			#endregion


			//Appending the pre processor arguments spefied by the user.
			sb.Append( separator );
			sb.Append( this.AdditionalPreprocessorArguments );

			string result = sb.ToString();
			return result;
		}
	}
}
