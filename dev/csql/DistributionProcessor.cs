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
	public class DistributionProcessor : Processor //, IBatchProcessor
	{
		private readonly TextWriter m_outputFileWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DistributionProcessor"/> class.
		/// </summary>
		/// <param name="cmdArgs">The CMD args.</param>
        public DistributionProcessor(CsqlOptions csqlOptions)
            : base(csqlOptions)
		{
            string outputFilePath = csqlOptions.DistributionFile;
			Stream stream = new FileStream( outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read );
			m_outputFileWriter = new StreamWriter( stream, Encoding.Unicode );
		}


        /// <summary>
        /// Emits an entry message.
        /// </summary>
        public override void SignIn()
        {
            // Emit console informations as defined in the base class.
            base.SignIn();

            String scriptFile = Options.ScriptFile;
            
            
            scriptFile = Path.GetFullPath( scriptFile );

            String distFile = Options.DistributionFile;
            distFile = Path.GetFullPath( distFile );

            // Create the dstribution file header
            StringBuilder headerBuilder = new StringBuilder();

            headerBuilder.AppendLine( "/* ****************************************************************************" );
            headerBuilder.AppendLine( "**" );
            headerBuilder.Append( "** Source script    : " ).AppendLine( scriptFile );
            headerBuilder.Append( "** Distribution file: " ).AppendLine( distFile );
            headerBuilder.Append( "** Created          : " ).AppendLine( DateTime.Now.ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern, CultureInfo.InvariantCulture ) );
            headerBuilder.Append( "** Preprocessed with: " ).Append( PreprocessorPath ).Append( ' ' ).AppendLine( PreprocessorArguments );
	        headerBuilder.AppendLine( "**");
	        headerBuilder.AppendLine( "**************************************************************************** */" );

            m_outputFileWriter.Write( headerBuilder.ToString() );
        }

        /// <summary>
        /// Emits the an exit/finished message.
        /// </summary>
        public override void SignOut()
        {
            StringBuilder footerBuilder = new StringBuilder();
            footerBuilder.AppendLine( "/* ****************************************************************************" );
            footerBuilder.AppendLine( "**" );
            footerBuilder.AppendLine( "** end of script" );
            footerBuilder.AppendLine( "**" );
            footerBuilder.AppendLine( "**************************************************************************** */" );
            m_outputFileWriter.Write( footerBuilder.ToString() );

            String distFile = Options.DistributionFile;
            distFile = Path.GetFullPath( distFile );
            Trace.WriteLineIf(GlobalSettings.Verbosity.TraceInfo, distFile + "(1): file created.");
        }



		protected override void ProcessProgress( string progressInfo )
		{
			string statement = "print '" + progressInfo.Replace( "'", "''" ) + "'\r\n";
			ProcessBatch( statement );
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
                } else {
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
		protected override void Dispose( bool isDisposing )
		{
			base.Dispose( isDisposing );
			if ( isDisposing ) {
				m_outputFileWriter.Dispose();
			}
		}

	}
}
