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
	internal static class BatchProcessorFactory
	{
		/// <summary>
		/// Creates the processor for the script.
		/// </summary>
		/// <param name="cmdArgs">The command line arguments.</param>
		/// <returns>The script processor</returns>
		internal static IBatchProcessor CreateProcessor( CSqlOptions csqlOptions )
		{
            //Setting the Global Trace Level
            GlobalSettings.Verbosity.Level = csqlOptions.Verbosity.Level;

			if ( !String.IsNullOrEmpty( csqlOptions.DistributionFile ) ) {
				DistributionProcessor processor = new DistributionProcessor( csqlOptions );
				return processor;
			} else {
				ExecutionProcessor processor = new ExecutionProcessor( csqlOptions );
				return processor;
			}
		}
	}
}
