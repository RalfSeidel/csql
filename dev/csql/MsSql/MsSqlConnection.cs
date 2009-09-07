using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace csql.MsSql
{
	/// <summary>
	/// Connection wrapper for native MS SQL Server connections.
	/// </summary>
	public class MsSqlConnection : DbConnection
	{
		public MsSqlConnection( CmdArgs cmdArgs ) : base( cmdArgs )
		{
		}

		protected override IDbConnection CreateAdoConnection( CmdArgs cmdArgs )
		{
			StringBuilder sb = new StringBuilder();

			if ( cmdArgs.Server.Length != 0 ) {
				sb.Append( "Server=" ).Append( cmdArgs.Server ).Append( ";" );
			}
			if ( cmdArgs.ServerPort != 0 ) {
				sb.Append( "Server Port=" ).Append( cmdArgs.ServerPort ).Append( ";" );
			}
			if ( !String.IsNullOrEmpty( cmdArgs.Database ) ) {
				sb.Append( "Database=" ).Append( cmdArgs.Database ).Append( ";" );
			}
			if ( !String.IsNullOrEmpty( cmdArgs.User ) ) {
				sb.Append( "User ID=" ).Append( cmdArgs.User ).Append( ";" );
				sb.Append( "Password=" ).Append( cmdArgs.Password ).Append( ";" );
			} else {
				sb.Append( "Integrated Security=SSPI;" );
			}
			sb.Append( "Application Name=" ).Append( "csql" ).Append( ";" );

			string connectionString = sb.ToString();
			SqlConnection adoConnection = new SqlConnection( connectionString );
			adoConnection.InfoMessage += new SqlInfoMessageEventHandler( InfoMessageHandler );
			adoConnection.Open();
			return adoConnection;
		}

		/// <summary>
		/// Handle for the server messages.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		/// The <see cref="System.Data.SqlClient.SqlInfoMessageEventArgs"/> instance containing the message data.
		/// </param>
		private void InfoMessageHandler( object sender, SqlInfoMessageEventArgs e )
		{
			SqlErrorCollection errors = e.Errors;
			if ( errors == null || errors.Count == 0 ) {
				DbMessageEventArgs eventArgs = new DbMessageEventArgs( e.Message );
				OnDbMessage( eventArgs );
			} else {
				foreach ( SqlError error in errors ) {
					TraceLevel severity = GetSeverity( error );
					string server = error.Server;
					string catalog = null;
					string procedure = error.Procedure;
					int lineNumber = error.LineNumber;
					string message = error.Message;
					DbMessageEventArgs eventArgs = new DbMessageEventArgs( severity, server, catalog, procedure, lineNumber, message );
					OnDbMessage( eventArgs );
				}
			}
		}

		/// <summary>
		/// Create a statement batch that will just echo the given messages texts.
		/// </summary>
		/// <param name="messages">The message texts.</param>
		/// <returns>
		/// Batch with some print messages.
		/// </returns>
		public override string GetPrintStatements( IEnumerable<string> messages )
		{
			StringBuilder sb = new StringBuilder();
			foreach ( string message in messages ) {
				sb.Append( "print '" ).Append( message.Replace( "'", "''" ) ).AppendLine( "';" );
			}
			string statement = sb.ToString();
			return statement;
		}

		/// <summary>
		/// Determine the tracelevel of the sql server message.
		/// </summary>
		/// <param name="error">The message data send by the SQL Server.</param>
		/// <returns>The internal trace level for the message.</returns>
		private static TraceLevel GetSeverity( SqlError error )
		{
			switch ( error.Number ) {
				case 2007:
					// Cannot add rows to sys.sql_dependencies for the stored procedure because it depends on...
					return TraceLevel.Warning;
				case 5701:
					// Changed database context to '...'.
					//return TraceLevel.Verbose;
					return TraceLevel.Info;
				default:
					return TraceLevel.Info;

			}
		}
	}

}
