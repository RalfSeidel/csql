using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

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
					string server = error.Server;
					string catalog = null;
					string procedure = error.Procedure;
					int lineNumber = error.LineNumber;
					string message = error.Message;
					DbMessageEventArgs eventArgs = new DbMessageEventArgs( server, catalog, procedure, lineNumber, message );
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
	}
}
