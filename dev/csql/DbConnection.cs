using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace csql
{
	public class DbMessageEventArgs : EventArgs
	{
		private readonly string m_server;
		private readonly string m_catalog;
		private readonly string m_procedure;
		private readonly int    m_lineNo;
		private readonly string m_message;

		public DbMessageEventArgs( string message )
		{
			this.m_message = message;
		}

		public DbMessageEventArgs( string server, string catalog, string procedure, int lineNo, string message )
		{
			this.m_server = server;
			this.m_catalog = catalog;
			this.m_procedure = procedure;
			this.m_lineNo = lineNo;
			this.m_message = message;
		}

		public string Server
		{
			get { return m_server; }
		}

		public string Catalog
		{
			get { return m_catalog; }
		}

		public string Procedure
		{
			get { return m_procedure; }
		}

		public int LineNumber
		{
			get { return m_lineNo; }
		}

		public string Message
		{
			get { return m_message; }
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			string separator = "";

			if ( !String.IsNullOrEmpty( m_server ) ) {
				sb.Append( separator );
				sb.Append( "Server: " );
				sb.Append( m_server );
				separator = ", ";
			}
			if ( !String.IsNullOrEmpty( m_catalog ) ) {
				sb.Append( separator );
				sb.Append( "Catalog: " );
				sb.Append( m_catalog );
				separator = ", ";
			}

			if ( !String.IsNullOrEmpty( m_procedure ) ) {
				sb.Append( separator );
				sb.Append( "procedure: " );
				sb.Append( m_procedure );
				separator = ", ";
			}

			if ( m_lineNo > 0 ) {
				sb.Append( separator );
				sb.Append( "Line: " );
				sb.Append( m_lineNo );
				separator = ", ";
			}

			if ( !String.IsNullOrEmpty( separator ) ) {
				separator = "\r\n";
			}
			sb.Append( separator );
			sb.Append( m_message );
			string result = sb.ToString();
			return result;
		}
	}

	/// <summary>
	/// A IDbConnection wrapper.
	/// </summary>
	public abstract class DbConnection : IDisposable
	{
		/// <summary>
		/// The inner exception that is used for the database communication.
		/// </summary>
		private readonly IDbConnection m_adoConnection;


		/// <summary>
		/// Occurs when a info message is recieved while executing sql scripts.
		/// </summary>
		public event EventHandler<DbMessageEventArgs> InfoMessage;


		protected DbConnection( IDbConnection adoConnection )
		{
			m_adoConnection = adoConnection;
		}

		/// <summary>
		/// Gets the ADO database connection object.
		/// </summary>
		/// <value>The ADO connection.</value>
		protected IDbConnection AdoConnection
		{
			get { return m_adoConnection; }
		}

		/// <summary>
		/// Executes the specified statement.
		/// </summary>
		/// <param name="statement">The statement.</param>
		/// <returns>Data reader.</returns>
		public IDataReader Execute( string statement )
		{
			using ( IDbCommand command = m_adoConnection.CreateCommand() ) {
				command.CommandText = statement;
				command.CommandType = CommandType.Text;
				IDataReader dataReader = command.ExecuteReader( CommandBehavior.SequentialAccess );
				return dataReader;
			}
		}

		/// <summary>
		/// Opens the connection using the parameter in the specified command line arguments.
		/// </summary>
		/// <param name="cmdArgs">The command line arguments.</param>
		public abstract void Open( CmdArgs cmdArgs );

		/// <summary>
		/// Traces the progress.
		/// </summary>
		/// <param name="info">The info.</param>
		public abstract void TraceProgress( string info );


		/// <summary>
		/// Raises the <see cref="E:DbMessage"/> event.
		/// </summary>
		/// <param name="args">The <see cref="csql.DbMessageEventArgs"/> instance containing the event data.</param>
		protected void OnDbMessage( DbMessageEventArgs args )
		{
			if ( this.InfoMessage != null ) {
				this.InfoMessage( this, args );
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			m_adoConnection.Dispose();
			GC.SuppressFinalize( this );
		}

		#endregion
	}


	/// <summary>
	/// Connection wrapper for native MS SQL Server connections.
	/// </summary>
	public class MsSqlConnection : DbConnection
	{
		public MsSqlConnection() : base( new SqlConnection() )
		{
			SqlConnection connection = (SqlConnection)AdoConnection;
			connection.InfoMessage += new SqlInfoMessageEventHandler( InfoMessageHandler );
		}

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

		public override void Open( CmdArgs cmdArgs )
		{
			StringBuilder sb = new StringBuilder();

			if ( cmdArgs.Server.Length != 0 ) {
				sb.Append( "Server=" ).Append( cmdArgs.Server ).Append( ";" );
			}
			if ( !String.IsNullOrEmpty( cmdArgs.Database ) ) {
				sb.Append( "Database=" ).Append( cmdArgs.Database ).Append( ";" );
			}
			if ( !String.IsNullOrEmpty( cmdArgs.User ) ) {
				sb.Append( "User ID=" ).Append( cmdArgs.User ).Append( ";" );
				sb.Append( "Password=" ).Append( cmdArgs.Password ).Append(  ";" );
				sb.Append( ";" );
			} else {
				sb.Append( "Integrated Security=SSPI;" );
			}

			string connectionString = sb.ToString();
			AdoConnection.ConnectionString = connectionString;
			AdoConnection.Open();
		}

		public override void TraceProgress( string info )
		{
			string statement = "print '" + info.Replace( "'", "''" ) + "'";
			using ( IDbCommand command = AdoConnection.CreateCommand() ) {
				command.CommandText = statement;
				command.CommandType = CommandType.Text;
				command.ExecuteNonQuery();
			}
		}

	}

	/// <summary>
	/// Connection wrapper for native OLEDB datasource connections.
	/// </summary>
	public class OleDbConnection : DbConnection
	{
		public OleDbConnection() : base( new System.Data.OleDb.OleDbConnection() )
		{
			System.Data.OleDb.OleDbConnection connection = (System.Data.OleDb.OleDbConnection)AdoConnection;
			connection.InfoMessage += new System.Data.OleDb.OleDbInfoMessageEventHandler( InfoMessageEventHandler );
		}

		public override void Open( CmdArgs cmdArgs )
		{
			throw new NotImplementedException( "TODO" );
		}

		public override void TraceProgress( string info )
		{
			Trace.WriteLineIf( Program.TraceLevel.TraceWarning, "Not implemented: OleDbConnection.TraceProgress" );
		}

		void InfoMessageEventHandler( object sender, System.Data.OleDb.OleDbInfoMessageEventArgs e )
		{
			System.Data.OleDb.OleDbErrorCollection errors = e.Errors;
			if ( errors == null || errors.Count == 0 ) {
				DbMessageEventArgs eventArgs = new DbMessageEventArgs( e.Message );
				OnDbMessage( eventArgs );
			} else {
				foreach ( System.Data.OleDb.OleDbError error in errors ) {
					string server = null;
					string catalog = null;
					string procedure = null;
					int lineNumber = 0;
					string message = error.Message;
					DbMessageEventArgs eventArgs = new DbMessageEventArgs( server, catalog, procedure, lineNumber, message );
					OnDbMessage( eventArgs );
				}
			}
		}

	}

	/// <summary>
	/// Connection wrapper for ODBC database connections.
	/// </summary>
	public class OdbcConnection : DbConnection
	{
		public OdbcConnection() : base( new System.Data.Odbc.OdbcConnection() )
		{
			System.Data.Odbc.OdbcConnection connection = (System.Data.Odbc.OdbcConnection)AdoConnection;
			connection.InfoMessage += new System.Data.Odbc.OdbcInfoMessageEventHandler( InfoMessageEventHandler );
		}

		public override void Open( CmdArgs cmdArgs )
		{
			throw new NotImplementedException( "TODO" );
		}

		public override void TraceProgress( string info )
		{
			Trace.WriteLineIf( Program.TraceLevel.TraceWarning, "Not implemented: OdbcConnection.TraceProgress" );
		}

		void InfoMessageEventHandler( object sender, System.Data.Odbc.OdbcInfoMessageEventArgs e )
		{
			System.Data.Odbc.OdbcErrorCollection errors = e.Errors;
			if ( errors == null || errors.Count == 0 ) {
				DbMessageEventArgs eventArgs = new DbMessageEventArgs( e.Message );
				OnDbMessage( eventArgs );
			} else {
				foreach ( System.Data.Odbc.OdbcError error in errors ) {
					string server = null;
					string catalog = null;
					string procedure = null;
					int lineNumber = 0;
					string message = error.Message;
					DbMessageEventArgs eventArgs = new DbMessageEventArgs( server, catalog, procedure, lineNumber, message );
					OnDbMessage( eventArgs );
				}
			}
		}
	}
}
