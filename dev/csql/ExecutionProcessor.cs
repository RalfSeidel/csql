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
			m_connection = ConnectionFactory.CreateConnection( cmdArgs );
			m_connection.InfoMessage += new EventHandler<DbMessageEventArgs>( InfoMessageEventHandler );
		}

		/// <summary>
		/// Handler for the informational messages send by the database server when processing the scripts.
		/// </summary>
		/// <param name="sender">The connection.</param>
		/// <param name="e">The <see cref="csql.DbMessageEventArgs"/> instance containing the message.</param>
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
			Trace.WriteLineIf( Program.TraceLevel.TraceInfo, progressInfo );
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
                    if ( dataReader.FieldCount != 0 ) {
                        TraceResult( dataReader );
                    }
					if ( !dataReader.NextResult() )
						break;
				}
				dataReader.Close();
			}
		}

        private void TraceResult( IDataReader dataReader )
        {
            if ( !Program.TraceLevel.TraceInfo )
                return;

            int fieldCount = dataReader.FieldCount;
            int lineWidth = 0;
            string separator = "";

            for ( int i = 0; i < fieldCount; ++i ) {
                String fieldName = dataReader.GetName( i );
                if ( String.IsNullOrEmpty( fieldName ) ) {
                    fieldName = "Column_" + i.ToString();
                }
                Trace.Write( separator );
                Trace.Write( fieldName );
                lineWidth+= separator.Length + fieldName.Length;
                separator = " ";
            }

            Trace.WriteLine( "" );
            Trace.WriteLine( new String( '-', lineWidth ) );

            int lineCount = 0;
            while ( dataReader.Read() ) {
                separator = "";
                for ( int i = 0; i < fieldCount; ++i ) {
                    string fieldValue;
                    if ( dataReader.IsDBNull( i ) ) {
                        fieldValue = "null";
                    } else {
                        fieldValue = dataReader[i].ToString();
                    }
                    Trace.Write( separator );
                    Trace.Write( fieldValue );
                    separator = " ";
                }
                Trace.WriteLine( "" );
                ++lineCount;
            }
            if ( lineCount == 0 ) {
                Trace.WriteLine( "[empty]" );
            }
        }
    }
}
