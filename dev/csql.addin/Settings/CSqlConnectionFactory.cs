using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using EnvDTE;
using System.Diagnostics.CodeAnalysis;

namespace csql.addin.Settings
{
	/// <summary>
	/// Sample to create a database connection based on the connection parameters
	/// configured in the csql addin.
	/// </summary>
	[SuppressMessage( "Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix", Justification="Derived from product name."  )]
	public static class CSqlConnectionFactory
	{
		private const string defaultProviderName = "System.Data.SqlClient";

		/// <summary>
		/// Gets the name of the visual studio variable name where csql stores the current 
		/// database factory provider name.
		/// </summary>
		public static string CSqlConnectionProviderVariableName
		{
			get { return "Sqt_CSqlAddin_DbConnectionProvider"; }
		}

		/// <summary>
		/// Gets the name of the visual studio variable name where csql stores the current 
		/// database connection string.
		/// </summary>
		public static string CSqlConnectionStringVariableName
		{
			get { return "Sqt_CSqlAddin_DbConnectionString"; }
		}

		public static IDbConnection CreateAndOpenConnection( _DTE application )
		{
			string providerName = GetProviderName( application );
			string connectionString = GetConnectionString( application );

			DbProviderFactory factory = System.Data.Common.DbProviderFactories.GetFactory( providerName );
			IDbConnection connection = factory.CreateConnection();
			connection.ConnectionString = connectionString;
			connection.Open();
			return connection;
		}

		/// <summary>
		/// Gets the database provider currently configured in the csql addin.
		/// </summary>
		public static string GetProviderName( _DTE application )
		{
			string providerName;

			if ( TryGetConnectionProviderFomGlobals( application, out providerName ) ) {
				return providerName;
			}

			if ( TryGetConnectionProviderFromWebConfig( application, out providerName ) ) {
				return providerName;
			}

			if ( TryGetConnectionProviderFromAppConfig( application, out providerName ) ) {
				return providerName;
			}

			string message = "Can't determine connection string. Please install the csql addin and configure the connection to use.";
			Trace.TraceError( message );
			throw new NotSupportedException( message );
		}

		/// <summary>
		/// Gets the connection string currently configured in the csql addin.
		/// </summary>
		public static string GetConnectionString( _DTE application )
		{
			string connectionString;

			if ( TryGetConnectionStringFromGlobals( application, out connectionString ) ) {
				return connectionString;
			}

			if ( TryGetConnectionStringFromWebConfig( application, out connectionString ) ) {
				return connectionString;
			}

			if ( TryGetConnectionStringFromAppConfig( application, out connectionString ) ) {
				return connectionString;
			}

			string message = "Can't determine connection string. Please install the csql addin and configure the connection to use.";
			Trace.TraceError( message );
			throw new NotSupportedException( message );
		}

		/// <summary>
		/// Tries to get the connection provider from the visual studio global variable pool.
		/// </summary>
		/// <returns><c>true</c> if the connection string was found. <c>false</c> if not.</returns>
		private static bool TryGetConnectionProviderFomGlobals( _DTE application, out string connectionProvider )
		{
			Globals globals = application.Globals;
			string variableName = CSqlConnectionProviderVariableName;
			if ( globals.get_VariableExists( variableName ) ) {
				connectionProvider = globals[variableName].ToString();
				return true;
			}
			else {
				connectionProvider = null;
				return false;
			}
		}


		/// <summary>
		/// Tries to get the connection string from the visual studio global variable pool.
		/// </summary>
		/// <returns><c>true</c> if the connection string was found. <c>false</c> if not.</returns>
		private static bool TryGetConnectionStringFromGlobals( _DTE application, out string connectionString )
		{
			Globals globals = application.Globals;
			string variableName = CSqlConnectionStringVariableName;
			if ( globals.get_VariableExists( variableName ) ) {
				connectionString = globals[variableName].ToString();
				return true;
			}
			else {
				connectionString = null;
				return false;
			}
		}

		/// <summary>
		/// TODO: Try getting the connection provider in the web.config of a web application in the current solution.
		/// </summary>
		private static bool TryGetConnectionProviderFromWebConfig( _DTE application, out string connectionProvider )
		{
			connectionProvider = null;

			return false;
		}

		/// <summary>
		/// TODO: Try getting the connection string in the web.config of a web application in the current solution.
		/// </summary>
		private static bool TryGetConnectionStringFromWebConfig( _DTE application, out string connectionString )
		{
			connectionString = null;

			return false;
		}

		/// <summary>
		/// TODO: Try getting the connection provider in the first app.config of a application in the current solution.
		/// </summary>
		private static bool TryGetConnectionProviderFromAppConfig( _DTE application, out string connectionProvider )
		{
			connectionProvider = null;
			return false;
		}

		/// <summary>
		/// TODO: Try getting the connection string in the first app.config of a application in the current solution.
		/// </summary>
		private static bool TryGetConnectionStringFromAppConfig( _DTE application, out string connectionString )
		{
			connectionString = null;
			return false;
		}


		private static string FindFileInSolution( _DTE application, string fileName )
		{
			Solution solution = application.Solution;

			if ( solution == null )
				return null;

			ProjectItem projectItem = solution.FindProjectItem( fileName );
			if ( projectItem == null )
				return null;

			if ( projectItem.FileCount == 0 )
				return null;

			string itemFileName = projectItem.get_FileNames( 0 );
			if ( itemFileName == null )
				return null;

			throw new NotImplementedException( "TODO" );
		}
	}
}
