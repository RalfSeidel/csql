using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
    /// <summary>
    /// This class encapsulates the conditions under which csql will create
    /// a distribution file or executes the sql commands by creating
    /// the appropriate batch processsor.
    /// </summary>
	public static class BatchProcessorFactory
	{
		/// <summary>
		/// Creates the processor for the script.
		/// </summary>
		/// <param name="cmdArgs">The command line arguments.</param>
		/// <returns>The script processor</returns>
		public static Processor CreateProcessor( CmdArgs cmdArgs )
		{
			if ( !String.IsNullOrEmpty( cmdArgs.DistFile ) ) {
				Processor processor = new DistributionProcessor( cmdArgs );
				return processor;
			} else {
				Processor processor = new ExecutionProcessor( cmdArgs );
				return processor;
			}
		}
	}
}
