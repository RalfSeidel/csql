using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace csql
{
	/// <summary>
	/// A IDbConnection wrapper.
	/// </summary>
	public abstract class DbConnection : IDisposable
	{
		/// <summary>
		/// The command line arguments.
		/// </summary>
		private readonly CmdArgs m_cmdArgs;

		/// <summary>
		/// The inner exception that is used for the database communication.
		/// </summary>
		private IDbConnection m_adoConnection;


		/// <summary>
		/// Occurs when a info message is recieved while executing sql scripts.
		/// </summary>
		public event EventHandler<DbMessageEventArgs> InfoMessage;


		/// <summary>
		/// Initializes a new instance of the <see cref="DbConnection"/> class.
		/// </summary>
		/// <param name="cmdArgs">The object holding the command line arguments of the program instance.</param>
		protected DbConnection( CmdArgs cmdArgs )
		{
			m_cmdArgs = cmdArgs;
		}

		/// <summary>
		/// Gets the ADO database connection object.
		/// </summary>
		/// <value>The ADO connection.</value>
		protected IDbConnection AdoConnection
		{
			get 
			{
				if ( m_adoConnection == null ) {
					m_adoConnection = CreateAdoConnection( m_cmdArgs );
				}
				return m_adoConnection; 
			}
		}

		/// <summary>
		/// Create a command object for the given statement (batch).
		/// </summary>
		/// <param name="statement">The SQL statement.</param>
		/// <returns>Initialized command object.</returns>
		public IDbCommand CreateCommand( string statement )
		{
			IDbConnection adoConnection = AdoConnection;
			IDbCommand command = adoConnection.CreateCommand();
			command.CommandTimeout = 0;
			command.CommandType = CommandType.Text;
			command.CommandText = statement;
			return command;
		}

		/// <summary>
		/// Executes the specified statement.
		/// </summary>
		/// <param name="command">The command created by CreateCommand.</param>
		/// <returns>A data reader for the query results.</returns>
		public virtual IDataReader Execute( IDbCommand command )
		{
			IDataReader dataReader = command.ExecuteReader( CommandBehavior.SequentialAccess );
			return dataReader;
		}

		public virtual Exception GetMappedException( Exception ex )
		{
			return null;
		}

		/// <summary>
		/// Creates the connection using the parameters in the specified command line arguments.
		/// </summary>
		/// <remarks>
		/// This method will be called to create the ADO connection object when the database
		/// is accessed for the first time. 
		/// </remarks>
		/// <param name="cmdArgs">The command line arguments.</param>
		/// <returns>The method has to return an open database connection object.</returns>
		protected abstract IDbConnection CreateAdoConnection( CmdArgs cmdArgs );

		/// <summary>
		/// Create a statement batch that will just echo the given messages texts.
		/// </summary>
		/// <param name="messages">The message texts.</param>
		/// <returns>
		/// Statement that will print the messages when executed.
		/// </returns>
		public abstract string GetPrintStatements( IEnumerable<string> messages );


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

		[SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification="No need for coding overhead" )]
		public void Dispose()
		{
			if ( m_adoConnection != null ) {
				m_adoConnection.Dispose();
				m_adoConnection = null;
			}
			GC.SuppressFinalize( this );
		}

		#endregion
	}
	

	/// <summary>
	/// Connection wrapper for native OLEDB datasource connections.
	/// </summary>
	public class OleDbConnection : DbConnection
	{
		public OleDbConnection( CmdArgs cmdArgs )
			: base( cmdArgs )
		{
			System.Data.OleDb.OleDbConnection connection = (System.Data.OleDb.OleDbConnection)AdoConnection;
			connection.InfoMessage += new System.Data.OleDb.OleDbInfoMessageEventHandler( InfoMessageEventHandler );
		}

		protected override IDbConnection CreateAdoConnection( CmdArgs cmdArgs )
		{
			throw new NotImplementedException( "TODO" );
		}

		public override string GetPrintStatements( IEnumerable<string> messages )
		{
			Trace.WriteLineIf( Program.TraceLevel.TraceWarning, "Not implemented: OleDbConnection.GetPrintStatements" );
			return null;
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
					DbMessageEventArgs eventArgs = new DbMessageEventArgs( TraceLevel.Info, server, catalog, procedure, lineNumber, message );
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
		public OdbcConnection( CmdArgs cmdArgs )
			: base( cmdArgs )
		{
			System.Data.Odbc.OdbcConnection connection = (System.Data.Odbc.OdbcConnection)AdoConnection;
			connection.InfoMessage += new System.Data.Odbc.OdbcInfoMessageEventHandler( InfoMessageEventHandler );
		}

		protected override IDbConnection CreateAdoConnection( CmdArgs cmdArgs )
		{
			throw new NotImplementedException( "TODO" );
			//connection.InfoMessage += new System.Data.Odbc.OdbcInfoMessageEventHandler( InfoMessageEventHandler );
		}

		public override string GetPrintStatements( IEnumerable<string> messages )
		{
			Trace.WriteLineIf( Program.TraceLevel.TraceWarning, "Not implemented: OleDbConnection.GetPrintStatements" );
			return null;
		}


		void InfoMessageEventHandler( object sender, System.Data.Odbc.OdbcInfoMessageEventArgs e )
		{
			System.Data.Odbc.OdbcErrorCollection errors = e.Errors;
			if ( errors == null || errors.Count == 0 ) {
				DbMessageEventArgs eventArgs = new DbMessageEventArgs( e.Message );
				OnDbMessage( eventArgs );
			} else {
				foreach ( System.Data.Odbc.OdbcError error in errors ) {
					TraceLevel severity = TraceLevel.Info;
					string server = null;
					string catalog = null;
					string procedure = null;
					int lineNumber = 0;
					string message = error.Message;
					DbMessageEventArgs eventArgs = new DbMessageEventArgs( severity, server, catalog, procedure, lineNumber, message );
					OnDbMessage( eventArgs );
				}
			}
		}
	}
}
