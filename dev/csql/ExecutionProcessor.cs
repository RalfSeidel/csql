using System;
using System.Data;
using System.Diagnostics;
using csql.ResultTrace;
using Sqt.DbcProvider;
using System.Text;
using System.Reflection;

namespace csql
{
	/// <summary>
	/// Processor for SQL script execution of the database server.
	/// </summary>
	internal class ExecutionProcessor : IBatchProcessor
	{
		private readonly CSqlOptions m_options;
		private readonly DbConnection m_connection;
		private ProcessorContext m_context;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutionProcessor"/> class.
		/// </summary>
		/// <param name="cmdArgs">The CMD args.</param>
		public ExecutionProcessor( CSqlOptions csqlOptions )
		{
			m_options = csqlOptions;
			DbConnectionParameter connectionParameter = csqlOptions.ConnectionParameter;
			m_connection = DbConnectionFactoryProvider.CreateConnection( connectionParameter );
			m_connection.InfoMessage += new EventHandler<DbMessageEventArgs>( InfoMessageEventHandler );

		}

		~ExecutionProcessor()
		{
			Dispose( false );
		}


		/// <summary>
		/// Emits an entry message.
		/// </summary>
		public virtual void SignIn()
		{
			if ( GlobalSettings.Verbosity.TraceInfo && !m_options.NoLogo ) {
				StringBuilder sb = new StringBuilder();
				Assembly assembly = Assembly.GetExecutingAssembly();
				Version version = assembly.GetName().Version;
				string name = GlobalSettings.CSqlProductName;
				sb.Append( name );
				sb.Append( " " );
				if ( GlobalSettings.Verbosity.TraceVerbose ) {
					sb.Append( version.ToString() );
				}
				else {
					sb.Append( version.ToString( 2 ) );
				}
				sb.Append( " (c) SQL Service GmbH" );
				sb.Append( " - Processing " ).Append( m_options.ScriptFile );

				string message = sb.ToString();
				Trace.WriteLine( message );
			}
		}

		/// <summary>
		/// Emits the an exit/finished message.
		/// </summary>
		public virtual void SignOut()
		{
		}

		public void ProcessProgress( ProcessorContext context, string progressInfo )
		{
			m_context = context;
			Trace.WriteLineIf( GlobalSettings.Verbosity.TraceInfo, progressInfo );
		}

		/// <summary>
		/// Processes the batch.
		/// </summary>
		/// <remarks>
		/// The method simply writes the batch to the output file and appends
		/// a "go" statement.
		/// </remarks>
		/// <param name="batch">The batch.</param>
		public void ProcessBatch( ProcessorContext context, string batch )
		{
			m_context = context;
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
				Exception mappedException = m_connection.GetMappedException( ex );
				if ( mappedException != null )
					throw mappedException;
				else
					throw;
			}
		}

		/// <summary>
		/// Cleanup implementation.
		/// </summary>
		/// <param name="isDisposing"></param>
		public void Dispose( bool isDisposing )
		{
			if ( isDisposing ) {
				m_connection.Dispose();
			}
		}

		/// <summary>
		/// Default dispose implementation
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Handler for the informational messages send by the database server when processing the scripts.
		/// </summary>
		/// <param name="sender">The connection.</param>
		/// <param name="e">The <see cref="csql.DbMessageEventArgs"/> instance containing the message.</param>
		private void InfoMessageEventHandler( object sender, DbMessageEventArgs e )
		{
			if ( GlobalSettings.Verbosity.Level >= e.TraceLevel ) {
				if ( GlobalSettings.Verbosity.TraceVerbose ) {
					Trace.WriteLine( e.ToString() );
				}
				else {
					if ( e.TraceLevel <= TraceLevel.Warning ) {
						Trace.WriteLine( m_context.FormatError( e.TraceLevel, e.Message, e.LineNumber ) );
					}
					else {
						Trace.WriteLine( e.Message );
					}
				}
			}
		}




		/// <summary>
		/// Traces the result of a database query.
		/// </summary>
		/// <param name="dataReader">The data reader returned by the execute call.</param>
		private static void TraceResult( IDataReader dataReader )
		{
			if ( !GlobalSettings.Verbosity.TraceInfo )
				return;

			DataReaderTracer tracer = new DataReaderTracer( dataReader );
			tracer.TraceAll();
		}
	}
}
