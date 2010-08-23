using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;

namespace csql
{
	/// <summary>
	/// Processor for the creation of a distribution file i.e. a script
	/// that contains the ouput of the preprosessor.
	/// </summary>
	internal class DistributionProcessor : IBatchProcessor
	{
		private readonly CSqlOptions m_options;
		private readonly TextWriter m_outputFileWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DistributionProcessor"/> class.
		/// </summary>
		/// <param name="csqlOptions">The script processor parameter.</param>
		public DistributionProcessor( CSqlOptions csqlOptions )
		{
			m_options = csqlOptions;
			string outputFilePath = csqlOptions.DistributionFile;
			Stream stream = new FileStream( outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read );
			m_outputFileWriter = new StreamWriter( stream, Encoding.Unicode );
		}

		~DistributionProcessor()
		{
			Dispose( false );
		}

		/// <summary>
		/// Emits an entry message.
		/// </summary>
		public void SignIn()
		{
			string scriptFile = m_options.ScriptFile;
			PreProcessor preProcessor = new PreProcessor( m_options );


			scriptFile = Path.GetFullPath( scriptFile );

			string distFile = m_options.DistributionFile;
			distFile = Path.GetFullPath( distFile );

			// Create the dstribution file header
			StringBuilder headerBuilder = new StringBuilder();

			headerBuilder.AppendLine( "/* ****************************************************************************" );
			headerBuilder.AppendLine( "**" );
			headerBuilder.Append( "** Source script    : " ).AppendLine( scriptFile );
			headerBuilder.Append( "** Distribution file: " ).AppendLine( distFile );
			headerBuilder.Append( "** Created          : " ).AppendLine( DateTime.Now.ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern, CultureInfo.InvariantCulture ) );
			headerBuilder.Append( "** Preprocessed with: " ).Append( PreProcessor.Command ).Append( ' ' ).AppendLine( preProcessor.Arguments );
			headerBuilder.AppendLine( "**" );
			headerBuilder.AppendLine( "**************************************************************************** */" );

			m_outputFileWriter.Write( headerBuilder.ToString() );
		}

		/// <summary>
		/// Emits the an exit/finished message.
		/// </summary>
		public void SignOut()
		{
			StringBuilder footerBuilder = new StringBuilder();
			footerBuilder.AppendLine( "/* ****************************************************************************" );
			footerBuilder.AppendLine( "**" );
			footerBuilder.AppendLine( "** end of script" );
			footerBuilder.AppendLine( "**" );
			footerBuilder.AppendLine( "**************************************************************************** */" );
			m_outputFileWriter.Write( footerBuilder.ToString() );

			string distFile = m_options.DistributionFile;
			distFile = Path.GetFullPath( distFile );
			Trace.WriteLineIf( GlobalSettings.Verbosity.TraceInfo, distFile + "(1): file created." );
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
				m_outputFileWriter.Write( output );
				m_outputFileWriter.WriteLine( "go" );
			}
		}



		/// <summary>
		/// Cleanup implementation.
		/// </summary>
		public void Dispose( bool isDisposing )
		{
			if ( isDisposing ) {
				m_outputFileWriter.Dispose();
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
