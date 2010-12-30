using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using CommandLine;
using Sqt.DbcProvider;

// Disable the not initialized and not used warning because the field
// values are managed by the command line parser.
#pragma warning disable 0169
#pragma warning disable 0649

namespace csql.exe
{
	/// <summary>
	/// The command line arguments for the program.
	/// </summary>
	public class CmdArgs
	{
		/// <summary>
		/// The configuration file containing further arguments.
		/// </summary>
		/// <remarks>
		/// This option is deprecated. You should use the @[filepath] instead.
		/// </remarks>
		/// <value>The path of the configuration file.</value>
		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The configuration file containing further arguments. This option is deprecated. You should use the @[filepath] instead.", ShortName = "cf" )]
		public string ConfigFile;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.Required, HelpText = "The input file that has to be processed.", LongName = "InputFile", ShortName = "i" )]
		public string ScriptFile;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "A temporary file for the preprocessor output. If you specify the temporary file it will not be deleted automaticly. If no temporary file name is specified the program will use named pipes to retrieve the output of the pre processor.", ShortName = "tf" )]
		public string TempFile;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The distribution file containing the output.", ShortName = "o" )]
		public string DistFile;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The database system.", ShortName = "ds" )]
		public ProviderType System;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.Required | ArgumentType.AtMostOnce, HelpText="The server/datasource name.", ShortName="S" )]
		public string Server;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The server IP port.", ShortName = "R" )]
		public int ServerPort;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification="Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText="The initial server database/catalog.", ShortName="D" )]
		public string Database;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The user name. This parameter is optional. If not specified the program will use integrated security.", ShortName = "U" )]
		public string User;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The server login password.", ShortName = "P" )]
		public string Password;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification= "This attribute is for backward compatibilty only. It will be replace by codepage in future." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The character set used when creating output files. This option is deprected.", ShortName = "cs", DefaultValue = "Ansi" )]
		public string Charset;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "Option to use the preprocessor or not.", ShortName = "UP", DefaultValue = true )]
		public bool UsePreprocessor;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "Option to stop execution on error.", ShortName = "b", DefaultValue = true )]
		public bool BreakOnError;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The arguments for the preprocess sqtpp.", ShortName = "PPA", DefaultValue = "" )]
		public string PreprocessorArgs;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The verbosity/trace level.", LongName = "Trace", ShortName = "t", DefaultValue = ((int)TraceLevel.Info) )]
		public int Verbose;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "Suppresses the output of the logo/version information.", ShortName = "N", DefaultValue = false )]
		public bool NoLogo;

		/// <summary>
		/// Gets a value indicating whether to use windows authentication (integrated security).
		/// </summary>
		/// <value>
		/// 	<c>true</c> if using windows authentication. <c>false</c> otherwise.
		/// </value>
		public bool UseWindowsAuthentication
		{
			get { return String.IsNullOrEmpty( User ); }
		}


		/// <summary>
		/// Creates the options for the script processor.
		/// </summary>
		/// <returns>The options instance created based on the command line arguments provided.</returns>
		[SecurityPermission( SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode )]
		public CSqlOptions CreateCsqlOptions()
		{
			DbConnectionParameter connectionParameter = new DbConnectionParameter();
			connectionParameter.Provider = this.System;
			connectionParameter.DatasourceAddress = this.Server;
			connectionParameter.DatasourcePort = this.ServerPort;
			connectionParameter.Catalog = this.Database;
			connectionParameter.UserId = this.User;
			connectionParameter.Password = this.Password;
			connectionParameter.IntegratedSecurity = string.IsNullOrEmpty( this.User );

			CSqlOptions csqlOptions = new CSqlOptions();
			csqlOptions.ConnectionParameter = connectionParameter;
			csqlOptions.ScriptFile = this.ScriptFile;
			csqlOptions.TempFile = this.TempFile;
			csqlOptions.DistributionFile = this.DistFile;
			csqlOptions.UsePreprocessor = this.UsePreprocessor;
			csqlOptions.BreakOnError = this.BreakOnError;
			csqlOptions.NoLogo = this.NoLogo;
			csqlOptions.Verbosity.Level = (TraceLevel)Verbose;
			csqlOptions.PreprocessorOptions.AdvancedArguments = this.PreprocessorArgs;


			csqlOptions.AddPreprocessorMacros();

			return csqlOptions;
		}


		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			Type thisType = this.GetType();
			StringBuilder sb = new StringBuilder();
			string separator = "";
			foreach ( FieldInfo field in thisType.GetFields() ) {
				sb.Append( separator );
				sb.Append( field.Name );
				sb.Append( ':' );
				sb.Append( field.GetValue( this ) );
				separator = " ";
			}
			string result = sb.ToString();
			return result;
		}
	}
}
