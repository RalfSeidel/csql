using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;
using EnvDTE;
using Sqt.DbcProvider;
using System.Diagnostics.CodeAnalysis;

namespace csql.addin.Settings
{

	/// <summary>
	/// Global accessor for the most recently used and current connection parameters.
	/// </summary>
	internal class SettingsManager
	{
		#region Private Fields

		private static readonly string dbConnectionVariableName = typeof( CSqlAddin ).FullName.Replace( '.', '_' ) + "_" + typeof( DbConnectionParameter ).Name;

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

		private MruConnections mruDbConnectionParameters;
		private DbConnectionParameter dbConnectionParameter;
		private CSqlParameter currentScriptParameter;


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
		/// Get the currently used database connection parameters.
		/// </summary>
		public DbConnectionParameter CurrentDbConnectionParameter
		{
			get
			{
				if ( dbConnectionParameter != null )
					return dbConnectionParameter;

				MruConnections mruConnections = MruDbConnectionParameters;
				dbConnectionParameter = MruDbConnectionParameterAdapter.GetMruDbConnectionParameter( mruConnections );
				return dbConnectionParameter;
			}
		}

		/// <summary>
		/// Get the currently used script processing parameter.
		/// </summary>
		public CSqlParameter CurrentScriptParameter
		{
			get
			{
				if ( currentScriptParameter != null ) {
					return currentScriptParameter;
				}

				this.currentScriptParameter = LoadSolutionScriptParameter();

				if ( currentScriptParameter == null ) {
					currentScriptParameter = new CSqlParameter();
				}
				return currentScriptParameter;
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
		internal void SaveDbConnectionParameter( DbConnectionParameter dbConnectionParameter )
		{
			MruConnections mruConnections = MruDbConnectionParameters;
			if ( mruConnections == null )
				return;

			bool parameterChanged = MruDbConnectionParameterAdapter.SetMruDbConnectionParameter( mruConnections, dbConnectionParameter );
			if ( !parameterChanged )
				return;

			string mruConnectionsPath = GetGlobalFilePath( "CSqlConnection.xml" );
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
				Debug.WriteLine( String.Format( "{0}: Failed to save connections because [{1}].", context, ex.Message ) );
			}
		}

		internal void SaveScriptParameter( CSqlParameter csqlParameter )
		{
			if ( csqlParameter == null )
				return;

			string csqlParameterPath = GetSolutionFilePath( application, "CSqlParameter.user.xml" );
			if ( String.IsNullOrEmpty( csqlParameterPath ) )
				return;

			SaveScriptParameterCore( csqlParameter, csqlParameterPath );
		}

		private void SaveScriptParameterCore( CSqlParameter csqlParameter, string filePath )
		{
			try {
				using ( Stream stream = new FileStream( filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan ) ) {
					XmlSerializer serializer = new XmlSerializer( csqlParameter.GetType() );
					serializer.Serialize( stream, csqlParameter );
					stream.Close();
				}
			}
			catch ( IOException ex ) {
				string context = MethodInfo.GetCurrentMethod().Name;
				Debug.WriteLine( String.Format( "{0}: Failed to save parameter because [{1}].", context, ex.Message ) );
			}
		}


		private CSqlParameter LoadSolutionScriptParameter()
		{
			string csqlParameterPath = GetSolutionFilePath( application, "CSqlParameter.user.xml" );
			if ( !String.IsNullOrEmpty( csqlParameterPath ) && File.Exists( csqlParameterPath ) ) {
				CSqlParameter parameter = LoadScriptParameterCore( csqlParameterPath );
				if ( parameter != null )
					return parameter;
			}
			csqlParameterPath = GetSolutionFilePath( application, "CSqlParameter.xml" );
			if ( !String.IsNullOrEmpty( csqlParameterPath ) && File.Exists( csqlParameterPath ) ) {
				CSqlParameter parameter = LoadScriptParameterCore( csqlParameterPath );
				if ( parameter != null )
					return parameter;
			}

			return null;
		}

		private static CSqlParameter LoadScriptParameterCore( string csqlParameterPath )
		{
			try {
				using ( Stream stream = new FileStream( csqlParameterPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan ) ) {
					XmlSerializer serializer = new XmlSerializer( typeof( CSqlParameter ) );
					CSqlParameter parameter = (CSqlParameter)serializer.Deserialize( stream );
					stream.Close();
					return parameter;
				}
			}
			catch ( Exception ex ) {
				string context = MethodInfo.GetCurrentMethod().Name;
				Debug.WriteLine( String.Format( "{0}: Failed to load parameter because [{1}].", context, ex.Message ) );
			}
			return null;
		}

		private MruConnections LoadMruConnectionParameters()
		{
			string mruConnectionsName = "CSqlConnection.xml";
			string mruConnectionsPath = GetGlobalFilePath( mruConnectionsName );
			if ( !String.IsNullOrEmpty( mruConnectionsPath ) && File.Exists( mruConnectionsPath ) ) {
				MruConnections mruConnections = LoadMruConnectionParameters( mruConnectionsPath );
				if ( mruConnections != null ) {
					return mruConnections;
				}
			}
			mruConnectionsPath = GetSolutionFilePath( this.application, mruConnectionsName );
			if ( !String.IsNullOrEmpty( mruConnectionsPath ) && File.Exists( mruConnectionsPath ) ) {
				MruConnections mruConnections = LoadMruConnectionParameters( mruConnectionsPath );
				if ( mruConnections != null ) {
					return mruConnections;
				}
			}

			return null;
		}

		private static MruConnections LoadMruConnectionParameters( string mruConnectionsPath )
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
			string solutionPath = application.Solution.FileName;
			if ( !String.IsNullOrEmpty( solutionPath ) ) {
				string solutionDirectory = Path.GetDirectoryName( solutionPath );
				string mruConnectionsPath = Path.Combine( solutionDirectory, name );
				return mruConnectionsPath;
			}
			else {
				return null;
			}
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
			CSqlParameter csqlParameter = LoadSolutionScriptParameter();
			if ( csqlParameter != null ) {
				currentScriptParameter = csqlParameter;
				RaiseSettingsReloaded();
			}
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
