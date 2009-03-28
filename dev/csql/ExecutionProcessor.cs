using System;
using System.Data;
using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// Processor for SQL script execution of the database server.
	/// </summary>
	public class ExecutionProcessor : Processor
	{
		private readonly DbConnection m_connection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutionProcessor"/> class.
		/// </summary>
		/// <param name="cmdArgs">The CMD args.</param>
		public ExecutionProcessor( CmdArgs cmdArgs )
		: base ( cmdArgs )
		{
			m_connection = ConnectionFactory.CreateConnection( cmdArgs.System, cmdArgs.Driver );
			m_connection.Open( cmdArgs );
			m_connection.InfoMessage += new EventHandler<DbMessageEventArgs>( InfoMessageEventHandler );
		}

		void InfoMessageEventHandler( object sender, DbMessageEventArgs e )
		{
			if ( Program.TraceLevel.TraceVerbose ) {
				Trace.WriteLine( e.ToString() );
			} else {
				Trace.WriteLineIf( Program.TraceLevel.TraceInfo, e.Message );
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
			m_connection.TraceProgress( progressInfo );
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
			using ( IDataReader dataReader = m_connection.Execute( batch ) ) {
				while ( dataReader != null && !dataReader.IsClosed ) {

					if ( !dataReader.NextResult() )
						break;
				}

			}
		}


	}
}
