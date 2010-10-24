using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using csql.ResultTrace;
using Sqt.DbcProvider;

namespace csql
{
	/// <summary>
	/// Processor for SQL script execution of the database server.
	/// </summary>
	internal class ExecutionProcessor : IBatchProcessor
	{
		private readonly CSqlOptions options;
		private readonly DbConnection connection;
		private ProcessorContext m_context;
		private IDbCommand currentCommand;
		private bool isCanceled;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutionProcessor"/> class.
		/// </summary>
		/// <param name="cmdArgs">The CMD args.</param>
		public ExecutionProcessor( CSqlOptions csqlOptions )
		{
			this.options = csqlOptions;
			DbConnectionParameter connectionParameter = csqlOptions.ConnectionParameter;
			this.connection = DbConnectionFactoryProvider.CreateConnection( connectionParameter );
			this.connection.InfoMessage += new EventHandler<DbMessageEventArgs>( InfoMessageEventHandler );

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
			if ( GlobalSettings.Verbosity.TraceInfo && !this.options.NoLogo ) {
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
				sb.Append( " - Processing " ).Append( this.options.ScriptFile );

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
				using ( IDbCommand command = this.connection.CreateCommand( batch ) )
				using ( IDataReader dataReader = this.connection.Execute( command ) ) {
					this.currentCommand = command;
					while ( dataReader != null && !dataReader.IsClosed && !isCanceled ) {
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
				Exception mappedException = this.connection.GetMappedException( ex );
				if ( mappedException != null )
					throw mappedException;
				else
					throw;
			}
			finally {
				lock ( this ) {
					this.currentCommand = null;
				}
			}
		}

		/// <summary>
		/// Cancels the execution of the current batch.
		/// </summary>
		public void Cancel()
		{
			lock ( this ) {
				this.isCanceled = true;
				if ( currentCommand != null ) {
					currentCommand.Cancel();
				}
			}
		}

		/// <summary>
		/// Cleanup implementation.
		/// </summary>
		/// <param name="isDisposing"></param>
		public void Dispose( bool isDisposing )
		{
			if ( isDisposing ) {
				this.connection.Dispose();
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
		private void TraceResult( IDataReader dataReader )
		{
			if ( !GlobalSettings.Verbosity.TraceInfo )
				return;

			DataReaderTraceOptions options = new DataReaderTraceOptions();
			options.MaxResultColumnWidth = this.options.MaxResultColumnWidth;
			DataReaderTracer tracer = new DataReaderTracer( dataReader, options );
			tracer.TraceAll();
		}
	}
}
