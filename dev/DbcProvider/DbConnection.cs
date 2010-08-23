using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// A IDbConnection wrapper.
	/// </summary>
	public abstract class DbConnection : IDisposable
	{
		/// <summary>
		/// The command line arguments.
		/// </summary>
        private readonly DbConnectionParameter parameter;

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
		internal DbConnection( DbConnectionParameter parameter )
		{
            this.parameter = parameter;
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
                    m_adoConnection = CreateAdoConnection(parameter);
				}
				return m_adoConnection; 
			}
		}

		/// <summary>
		/// Gets the parameter used to establish the connection.
		/// </summary>
		internal protected DbConnectionParameter Parameter
		{
			get { return this.parameter; }
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
		protected abstract IDbConnection CreateAdoConnection( DbConnectionParameter csqlOptions );

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
}
