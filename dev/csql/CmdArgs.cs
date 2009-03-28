using System;
using CommandLine;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

// Disable the not initialized and not used warning because the field
// values are managed by the command line parser.
#pragma warning disable 0169
#pragma warning disable 0649

namespace csql
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
		[Argument( ArgumentType.AtMostOnce, HelpText = "A temporary file for the preprocessor output. If you specify the temporary file it will not be deleted automaticly.", ShortName = "tf" )]
		public string TempFile;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The distribution file containing the output.", ShortName = "o" )]
		public string DistFile;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The database system.", ShortName = "ds" )]
		public DbSystem System;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The driver to use for the server connections.", ShortName = "dr" )]
		public DbDriver Driver;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The server/datasource name.", ShortName = "S" )]
		public string Server;

		[SuppressMessage( "Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Command line parser does not support properties." )]
		[Argument( ArgumentType.AtMostOnce, HelpText = "The initial server database/catalog.", ShortName = "D" )]
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

		public string PreprocessorDefines
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				string separator = " ";

				Assembly assembly = Assembly.GetExecutingAssembly();
				AssemblyName assemblyName = assembly.GetName();
				Version assemblyVersion = assemblyName.Version;
				string version = String.Format( CultureInfo.InvariantCulture, "{0:d2}.{1:d2}"
					, assemblyVersion.Major, assemblyVersion.Minor );

				if ( !String.IsNullOrEmpty( version ) ) {
					sb.Append( separator );
					sb.Append( "/D_CSQL_VERSION=" );
					sb.Append( version );
					separator = " ";
				}
				if ( !String.IsNullOrEmpty( Server ) ) {
					sb.Append( separator );
					sb.Append( "/D_CSQL_SERVER=" );
					sb.Append( Server );
					separator = " ";
				}
				if ( !String.IsNullOrEmpty( Database ) ) {
					sb.Append( separator );
					sb.Append( "/D_CSQL_DATABASE=" );
					sb.Append( Database );
					separator = " ";
				}
				if ( !String.IsNullOrEmpty( User ) ) {
					sb.Append( separator );
					sb.Append( "/D_CSQL_USER=" );
					sb.Append( User );
					separator = " ";
				}
				string result = sb.ToString();
				return result;
			}
		}


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
				separator = "\r\n";
			}
			string result = sb.ToString();
			return result;
		}
	}
}
