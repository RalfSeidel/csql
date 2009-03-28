using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace csql
{
	/// <summary>
	/// Processor for the creation of a distribution file i.e. a script
	/// that contains the ouput of the preprosessor.
	/// </summary>
	public class DistributionProcessor : Processor
	{
		private readonly TextWriter m_outputFileWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DistributionProcessor"/> class.
		/// </summary>
		/// <param name="cmdArgs">The CMD args.</param>
		public DistributionProcessor( CmdArgs cmdArgs )
		: base ( cmdArgs )
		{
			string outputFilePath = cmdArgs.DistFile;
			Stream stream = new FileStream( outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read );
			m_outputFileWriter = new StreamWriter( stream );
		}


		protected override void ProcessProgress( string progressInfo )
		{
			string statement = "print '" + progressInfo.Replace( "'", "''" ) + "'";
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
			m_outputFileWriter.Write( batch );
			m_outputFileWriter.WriteLine( "go" );
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
