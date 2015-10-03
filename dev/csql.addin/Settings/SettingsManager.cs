using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;
using csql.addin.Gui;
using EnvDTE;
using Sqt.DbcProvider;
using Sqt.DbcProvider.Provider;

namespace csql.addin.Settings
{
	/// <summary>
	/// Global accessor for the most recently used and current connection parameters.
	/// </summary>
	internal sealed class SettingsManager
	{
		#region Private Fields

		private readonly _DTE application;

		/// <summary>
		/// Hold a reference to the application solution event because if application.SolutionsEvents
		/// is used locally only it will be deleted by the garbage collector.
		/// </summary>
		/// <seealso cref="http://www.mztools.com/articles/2005/MZ2005012.aspx">
		/// PRB: Visual Studio .NET events being disconnected from add-in.
		/// </seealso>
		private readonly EnvDTE.SolutionEvents solutionEvents;

		private static SettingsManager instance;

		/// <summary>
		/// The name of the file that contains the most recently used connections.
		/// </summary>
		private const string MruConnectionFileName = "csqlConnection.xml";

		private MruConnections mruDbConnectionParameters;
		private ScriptParameterCollection scriptParameters;

		#endregion

		private SettingsManager( _DTE application )
		{
			this.application = application;
			this.solutionEvents = application.Events.SolutionEvents;
			solutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler( Solution_Opened );
		}

		/// <summary>
		/// Event raised whenever the settings were reloaded.
		/// </summary>
		public event EventHandler SettingsReloaded;

		public ScriptParameterCollection ScriptParameters
		{
			get
			{
				if ( this.scriptParameters == null ) {
					this.scriptParameters = LoadSolutionScriptParameter();
				}
				if ( this.scriptParameters == null ) {
					this.scriptParameters = new ScriptParameterCollection();
					var defaultParameter = ScriptParameter.CreateDefaultInstance();
					this.scriptParameters.Add( defaultParameter );
				}
				return this.scriptParameters;
			}
		}

		/// <summary>
		/// Get the currently used script processing parameter.
		/// </summary>
		public ScriptParameter CurrentScriptParameter
		{
			get { return ScriptParameters.Current; }
		}

		/// <summary>
		/// Get the currently used database connection parameters.
		/// </summary>
		public DbConnectionParameter CurrentDbConnectionParameter
		{
			get
			{
				ScriptParameterCollection scriptParameters = ScriptParameters;
				DbConnectionParameter dbConnectionParameter = scriptParameters.DbConnection;

				if ( dbConnectionParameter != null )
					return dbConnectionParameter;

				MruConnections mruConnections = MruDbConnectionParameters;
				dbConnectionParameter = MruDbConnectionParameterAdapter.GetMruDbConnectionParameter( mruConnections );
				scriptParameters.DbConnection = dbConnectionParameter;
				return dbConnectionParameter;
			}
		}

		/// <summary>
		/// Get the collection of most recently used database connection parameters.
		/// </summary>
		public MruConnections MruDbConnectionParameters
		{
			get
			{
				if ( mruDbConnectionParameters != null )
					return mruDbConnectionParameters;

				this.mruDbConnectionParameters = LoadMruConnectionParameters();

				if ( mruDbConnectionParameters == null ) {
					mruDbConnectionParameters = new MruConnections();
				}

				return mruDbConnectionParameters;
			}
		}

		/// <summary>
		/// Get or create the singleton instance of the parameter acessors.
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		public static SettingsManager GetInstance( _DTE application )
		{
			if ( application == null )
				throw new ArgumentNullException( "application" );

			if ( instance == null ) {
				instance = new SettingsManager( application );
			}
			return instance;
		}

		/// <summary>
		/// Update the most recently used db connection parameter in the application data folder.
		/// </summary>
		/// <param name="dbConnectionParameter">The db connection parameter.</param>
		internal void SaveDbConnectionParameterInMruHistory( DbConnectionParameter dbConnectionParameter )
		{
			MruConnections mruConnections = MruDbConnectionParameters;
			if ( mruConnections == null )
				return;

			bool parameterChanged = MruDbConnectionParameterAdapter.SetMruDbConnectionParameter( mruConnections, dbConnectionParameter );
			if ( !parameterChanged )
				return;

			string mruConnectionsPath = GetGlobalFilePath( MruConnectionFileName );
			if ( String.IsNullOrEmpty( mruConnectionsPath ) )
				return;


			try {
				string directory = Path.GetDirectoryName( mruConnectionsPath );
				Directory.CreateDirectory( directory );
				using ( Stream stream = new FileStream( mruConnectionsPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan ) ) {
					XmlSchema xmlSchema = MruConnections.Schema;
					XmlSerializer serializer = new XmlSerializer( typeof( MruConnections ), xmlSchema.TargetNamespace );
					serializer.Serialize( stream, mruConnections );
					stream.Close();
				}
			}
			catch ( Exception ex ) {
				string context = MethodInfo.GetCurrentMethod().Name;
				string message = String.Format( "{0}: Failed to save connection settings because [{1}].", context, ex.Message );
				Debug.WriteLine( message );
				var outputPane = Output.GetAndActivateOutputPane( (EnvDTE80.DTE2)application );
				outputPane.OutputString( message );
			}
		}

		/// <summary>
		/// Stores the current database connection parameter in the global variables 
		/// of the visual studio environment.
		/// </summary>
		/// <param name="dbConnectionParameter">The database connection parameter.</param>
		internal void SaveDbConnectionParameterInGlobals( DbConnectionParameter dbConnectionParameter )
		{
			IWrappedDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( dbConnectionParameter.Provider );
			string providerName = factory.ProviderName;
			string connectionString = factory.GetConnectionString( dbConnectionParameter );

			Globals globals = application.Globals;
			string variableName = CSqlConnectionFactory.CSqlConnectionProviderVariableName;
			globals[variableName] = providerName;
			globals.set_VariablePersists( variableName, true );

			variableName = CSqlConnectionFactory.CSqlConnectionStringVariableName;
			globals[variableName] = connectionString;
			globals.set_VariablePersists( variableName, true );
		}

		/// <summary>
		/// Saves the current script parameter in the solution settings files.
		/// </summary>
		/// <param name="csqlParameter">The current script parameter.</param>
		internal void SaveScriptParameterInSolutionSettings( ScriptParameterCollection scriptParameters )
		{
			if ( scriptParameters == null )
				return;

			string solutionDirectory = GetSolutionDirectory( application );
			string csqlParameterName = "CSqlParameter.xml";
			string csqlParameterPath;
			if ( string.IsNullOrEmpty( solutionDirectory ) ) {
				csqlParameterPath = GetGlobalFilePath( csqlParameterName );
			}
			else {
				csqlParameterPath = GetSolutionFilePath( application, csqlParameterName );
				if ( !IsFileWritable( csqlParameterPath ) ) {
					csqlParameterName = "CSqlParameter.user.xml";
					csqlParameterPath = GetSolutionFilePath( application, csqlParameterName );
				}
			}

			if ( String.IsNullOrEmpty( csqlParameterPath ) )
				return;

			SaveScriptParameterCore( scriptParameters, csqlParameterPath );
		}

		private static void SaveScriptParameterCore( ScriptParameterCollection scriptParameters, string filePath )
		{
			try {
				using ( Stream stream = new FileStream( filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan ) ) {
					XmlSerializer serializer = new XmlSerializer( scriptParameters.GetType() );
					serializer.Serialize( stream, scriptParameters );
					stream.Close();
				}
			}
			catch ( IOException ex ) {
				string context = MethodInfo.GetCurrentMethod().Name;
				Debug.WriteLine( String.Format( "{0}: Failed to save parameter because [{1}].", context, ex.Message ) );
			}
		}


		/// <summary>
		/// Loads the parameter located in the current solution directory
		/// and updates the global current settings variable with the script
		/// parameters loaded.
		/// </summary>
		private ScriptParameterCollection LoadSolutionScriptParameter()
		{
			ScriptParameterCollection parameters = LoadSolutionScriptParameterCore();
			if ( parameters != null && parameters.DbConnection != null ) {
				SaveDbConnectionParameterInGlobals( parameters.DbConnection );
			}

			return parameters;
		}

		private ScriptParameterCollection LoadSolutionScriptParameterCore()
		{
			string solutionPath = application.Solution.FileName;
			string parametersPath;

			if ( string.IsNullOrEmpty( solutionPath ) ) {
				// If no solution is open use the global file.
				parametersPath = GetGlobalFilePath( "CSqlParameter.xml" );
				if ( !String.IsNullOrEmpty( parametersPath ) && File.Exists( parametersPath ) ) {
					ScriptParameterCollection parameters = LoadScriptParameterFromFile( parametersPath );
					if ( parameters != null )
						return parameters;
				}
			}
			else {
				parametersPath = GetSolutionFilePath( application, "CSqlParameter.user.xml" );
				if ( !String.IsNullOrEmpty( parametersPath ) && File.Exists( parametersPath ) ) {
					ScriptParameterCollection parameters = LoadScriptParameterFromFile( parametersPath );
					if ( parameters != null )
						return parameters;
				}
				parametersPath = GetSolutionFilePath( application, "CSqlParameter.xml" );
				if ( !String.IsNullOrEmpty( parametersPath ) && File.Exists( parametersPath ) ) {
					ScriptParameterCollection parameters = LoadScriptParameterFromFile( parametersPath );
					if ( parameters != null )
						return parameters;
				}
			}

			return null;
		}

		/// <summary>
		/// Try to load the collection of pramters. If that fails try to load a single parameter set (backward compatibility:
		/// </summary>
		/// <param name="csqlParameterPath">The parameter file path.</param>
		/// <returns></returns>
		private static ScriptParameterCollection LoadScriptParameterFromFile( string parameterPath )
		{
			try {
				using ( Stream stream = new FileStream( parameterPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan ) ) {
					XmlSerializer serializer = new XmlSerializer( typeof( ScriptParameterCollection ) );
					ScriptParameterCollection parameters = (ScriptParameterCollection)serializer.Deserialize( stream );
					stream.Close();
					return parameters;
				}
			}
			catch ( Exception ex ) {
				string context = MethodInfo.GetCurrentMethod().Name;
				Debug.WriteLine( String.Format( "{0}: Failed to load parameter because [{1}].", context, ex.Message ) );
			}
			try {
				using ( Stream stream = new FileStream( parameterPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan ) ) {
					XmlSerializer serializer = new XmlSerializer( typeof( ScriptParameter ) );
					ScriptParameter parameter = (ScriptParameter)serializer.Deserialize( stream );
					stream.Close();
					ScriptParameterCollection parameters = new ScriptParameterCollection();
					parameters.Add( parameter );
					return parameters;
				}
			}
			catch ( Exception ex ) {
				string context = MethodInfo.GetCurrentMethod().Name;
				Debug.WriteLine( String.Format( "{0}: Failed to load parameter because [{1}].", context, ex.Message ) );
			}
			return null;
		}

		/// <summary>
		/// Loads the history of most recently used database connection parameters.
		/// </summary>
		private MruConnections LoadMruConnectionParameters()
		{
			string mruConnectionsName = MruConnectionFileName;
			string mruConnectionsPath = GetGlobalFilePath( mruConnectionsName );
			if ( !String.IsNullOrEmpty( mruConnectionsPath ) && File.Exists( mruConnectionsPath ) ) {
				MruConnections mruConnections = LoadMruConnectionParametersFromFile( mruConnectionsPath );
				if ( mruConnections != null ) {
					return mruConnections;
				}
			}
			mruConnectionsPath = GetSolutionFilePath( this.application, mruConnectionsName );
			if ( !String.IsNullOrEmpty( mruConnectionsPath ) && File.Exists( mruConnectionsPath ) ) {
				MruConnections mruConnections = LoadMruConnectionParametersFromFile( mruConnectionsPath );
				if ( mruConnections != null ) {
					return mruConnections;
				}
			}

			return null;
		}

		private static MruConnections LoadMruConnectionParametersFromFile( string mruConnectionsPath )
		{
			if ( !String.IsNullOrEmpty( mruConnectionsPath ) && File.Exists( mruConnectionsPath ) ) {
				try {
					MruConnections mruConnections = MruConnections.LoadFromFile( mruConnectionsPath );
					return mruConnections;
				}
				catch ( Exception e ) {
					string context = MethodInfo.GetCurrentMethod().Name;
					Debug.WriteLine( String.Format( "{0}: Failed to load connections because [{1}].", context, e.Message ) );
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the absolute file path for the given file name in the current solution directory.
		/// </summary>
		/// <param name="name">The file name.</param>
		/// <returns>The absolute path or <c>null</c> if no solution is loaded.</returns>
		private static string GetSolutionFilePath( _DTE application, string name )
		{
			string solutionDirectory = GetSolutionDirectory( application );
			if ( !String.IsNullOrEmpty( solutionDirectory ) ) {
				string mruConnectionsPath = Path.Combine( solutionDirectory, name );
				return mruConnectionsPath;
			}
			else {
				return null;
			}
		}

		private static string GetSolutionDirectory( _DTE application )
		{
			string solutionPath = application.Solution.FileName;
			if ( string.IsNullOrEmpty( solutionPath ) )
				return null;

			string solutionDirectory = Path.GetDirectoryName( solutionPath );
			return solutionDirectory;
		}

		private static string GetGlobalFilePath( string name )
		{
			string directory = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
			directory = Path.Combine( directory, @"SqlService\csql\" );
			string result = Path.Combine( directory, name );
			return result;
		}


		private void Solution_Opened()
		{
			ScriptParameterCollection scriptParameters = LoadSolutionScriptParameter();
			if ( scriptParameters != null ) {
				this.scriptParameters = scriptParameters;
				RaiseSettingsReloaded();
			}
		}

		private static bool IsFileWritable( string filePath )
		{
			if ( !File.Exists( filePath ) )
				return true;

			FileAttributes attributes = File.GetAttributes( filePath );
			if ( (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly )
				return false;

			try {
				using ( var stream = File.OpenWrite( filePath ) ) {
				}
			}
			catch ( System.UnauthorizedAccessException ) {
				return false;
			}
			catch ( IOException ) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// Raise the SettingsReloaded event.
		/// </summary>
		[SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "The method raises the event." )]
		private void RaiseSettingsReloaded()
		{
			if ( this.SettingsReloaded != null ) {
				this.SettingsReloaded( this, EventArgs.Empty );
			}
		}
	}
}
