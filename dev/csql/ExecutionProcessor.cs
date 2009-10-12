using System;
using System.Data;
using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// Processor for SQL script execution of the database server.
	/// </summary>
    public class ExecutionProcessor : Processor// , IBatchProcessor
	{
		private readonly DbConnection m_connection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutionProcessor"/> class.
		/// </summary>
		/// <param name="cmdArgs">The CMD args.</param>
        public ExecutionProcessor(CsqlOptions csqlOptions)
            : base(csqlOptions)
		{
            m_connection = ConnectionFactory.CreateConnection(csqlOptions);
			m_connection.InfoMessage += new EventHandler<DbMessageEventArgs>( InfoMessageEventHandler );
            
		}


		/// <summary>
		/// Handler for the informational messages send by the database server when processing the scripts.
		/// </summary>
		/// <param name="sender">The connection.</param>
		/// <param name="e">The <see cref="csql.DbMessageEventArgs"/> instance containing the message.</param>
		void InfoMessageEventHandler( object sender, DbMessageEventArgs e )
		{
            if (GlobalSettings.Verbosity.Level >= e.TraceLevel)
            {
                if (GlobalSettings.Verbosity.TraceVerbose)
                {
					Trace.WriteLine( e.ToString() );
				} else {
					if ( e.TraceLevel <= TraceLevel.Warning ) {
						Trace.WriteLine( FormatError( e.TraceLevel, e.Message, e.LineNumber ) );
					} else {
						Trace.WriteLine( e.Message );
					}
				}
			}
		}

		/// <summary>
		/// Cleanup implementation.
		/// </summary>
		/// <param name="isDisposing"></param>
		protected override void Dispose( bool isDisposing )
		{
			base.Dispose( isDisposing );
			if ( isDisposing ) {
				m_connection.Dispose();
			}
		}

		protected override void ProcessProgress( string progressInfo )
		{
            Trace.WriteLineIf(GlobalSettings.Verbosity.TraceInfo, progressInfo);
		}

		/// <summary>
		/// Processes the batch.
		/// </summary>
		/// <remarks>
		/// The method simply writes the batch to the output file and appends
		/// a "go" statement.
		/// </remarks>
		/// <param name="batch">The batch.</param>
		protected override void ProcessBatch( string batch )
		{
			try {
				using ( IDbCommand command = m_connection.CreateCommand( batch ) ) 
				using ( IDataReader dataReader = m_connection.Execute( command ) ) {
					while ( dataReader != null && !dataReader.IsClosed ) {
						if ( dataReader.FieldCount != 0 ) {
							TraceResult( dataReader );
						}
						if ( !dataReader.NextResult() )
							break;
					}
					dataReader.Close();
				}
			}
			catch ( Exception ex ) {
				Exception mappedException = m_connection.GetMappedException(ex);
				if ( mappedException != null ) 
					throw mappedException;
				else 
					throw;
			}
		}


		/// <summary>
		/// Traces the result of a database query.
		/// </summary>
		/// <param name="dataReader">The data reader returned by the execute call.</param>
        private static void TraceResult( IDataReader dataReader )
        {
            if (!GlobalSettings.Verbosity.TraceInfo)
                return;

			DataReaderTracer tracer = new DataReaderTracer( dataReader );
			tracer.TraceAll();
        }
    }
}
