using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace csql
{
	/// <summary>
	/// Processor for the creation of a distribution file i.e. a script
	/// that contains the ouput of the preprosessor.
	/// </summary>
	internal class DistributionProcessor : IBatchProcessor
	{
		private readonly CSqlOptions options;
		private Stream outputStream;
		private TextWriter outputFileWriter;
		private bool isCanceled;

		/// <summary>
		/// Initializes a new instance of the <see cref="DistributionProcessor"/> class.
		/// </summary>
		/// <param name="csqlOptions">The script processor parameter.</param>
		public DistributionProcessor( CSqlOptions options )
		{
			this.options = options;
			string outputFilePath = Path.GetFullPath( options.DistributionFile );

		}

		~DistributionProcessor()
		{
			Dispose( false );
		}

		/// <summary>
		/// Validate the options.
		/// </summary>
		public void Validate()
		{
			string inputFilePath = Path.GetFullPath( this.options.ScriptFile );
			string outputFilePath = Path.GetFullPath( options.DistributionFile );

			if ( string.Equals( inputFilePath, outputFilePath, StringComparison.OrdinalIgnoreCase ) ) {
				this.isCanceled = true;
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceError, "Error: The input and output file are the same." );
				throw new TerminateException( ExitCode.ArgumentsError );
			}
		}

		/// <summary>
		/// Emits an entry message.
		/// </summary>
		public void SignIn()
		{
			PreProcessor preProcessor = new PreProcessor( this.options );
			string inputFilePath = this.options.ScriptFile;
			inputFilePath = Path.GetFullPath( inputFilePath );

			string outputFilePath = this.options.DistributionFile;
			outputFilePath = Path.GetFullPath( outputFilePath );

			this.outputStream = new FileStream( outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read );
			this.outputFileWriter = new StreamWriter( outputStream, Encoding.Unicode );

			Trace.WriteLineIf( GlobalSettings.Verbosity.TraceInfo, "Creating distribution file " + outputFilePath + " from " + inputFilePath + "." );

			// Create the dstribution file header
			StringBuilder headerBuilder = new StringBuilder();

			headerBuilder.AppendLine( "/* ****************************************************************************" );
			headerBuilder.AppendLine( "**" );
			headerBuilder.Append( "** Source script    : " ).AppendLine( inputFilePath );
			headerBuilder.Append( "** Distribution file: " ).AppendLine( outputFilePath );
			headerBuilder.Append( "** Created          : " ).AppendLine( DateTime.Now.ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern, CultureInfo.InvariantCulture ) );
			headerBuilder.Append( "** Preprocessed with: " ).Append( PreProcessor.Command ).Append( ' ' ).AppendLine( preProcessor.Arguments );
			headerBuilder.AppendLine( "**" );
			headerBuilder.AppendLine( "**************************************************************************** */" );

			this.outputFileWriter.Write( headerBuilder.ToString() );
		}

		/// <summary>
		/// Emits the an exit/finished message.
		/// </summary>
		public void SignOut()
		{
			if ( isCanceled ) {
				Trace.WriteLineIf( GlobalSettings.Verbosity.TraceInfo, "Script processing was canceled." );
				return;
			}

			StringBuilder footerBuilder = new StringBuilder();
			footerBuilder.AppendLine( "/* ****************************************************************************" );
			footerBuilder.AppendLine( "**" );
			footerBuilder.AppendLine( "** end of script" );
			footerBuilder.AppendLine( "**" );
			footerBuilder.AppendLine( "**************************************************************************** */" );
			this.outputFileWriter.Write( footerBuilder.ToString() );

			string distFile = this.options.DistributionFile;
			distFile = Path.GetFullPath( distFile );
			Trace.WriteLineIf( GlobalSettings.Verbosity.TraceInfo, distFile + "(1): file created." );

			this.outputFileWriter.Close();
			this.outputStream.Close();
		}

		public void ProcessProgress( ProcessorContext processorContext, string progressInfo )
		{
			string statement = "print '" + progressInfo.Replace( "'", "''" ) + "'\r\n";
			ProcessBatch( processorContext, statement );
		}

		/// <summary>
		/// Processes the batch.
		/// </summary>
		/// <remarks>
		/// The method simply writes the batch to the output file and appends
		/// a "go" statement.
		/// </remarks>
		/// <param name="batch">The batch.</param>
		public void ProcessBatch( ProcessorContext processorContext, string batch )
		{
			StringBuilder outputBuilder = new StringBuilder();
			StringBuilder lineBuilder = new StringBuilder();
			string line;

			foreach ( char c in batch ) {
				if ( c == '\r' || c == '\n' ) {
					line = lineBuilder.ToString().TrimEnd();
					if ( line.Length != 0 ) {
						outputBuilder.AppendLine( line );
					}
					lineBuilder = new StringBuilder();
				}
				else {
					lineBuilder.Append( c );
				}
			}
			line = lineBuilder.ToString().TrimEnd();
			if ( line.Length != 0 ) {
				outputBuilder.AppendLine( line );
			}

			string output = outputBuilder.ToString();
			if ( output.Length != 0 ) {
				this.outputFileWriter.Write( output );
				this.outputFileWriter.WriteLine( "go" );
			}
		}

		/// <summary>
		/// Cancels the execution of the current batch.
		/// </summary>
		/// <remarks>
		/// The method does nothing because it is assumed that this processor execute fast enough
		/// to check the cancel condition in the outer processor only.
		/// </remarks>
		public void Cancel()
		{
			this.isCanceled = true;
		}

		/// <summary>
		/// Cleanup implementation.
		/// </summary>
		public void Dispose( bool isDisposing )
		{
			if ( isDisposing ) {
				if ( this.outputFileWriter != null )
					this.outputFileWriter.Dispose();
				if ( this.outputStream	 != null )
					this.outputStream.Dispose();
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
	}
}
