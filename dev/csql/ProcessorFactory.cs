using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
	public static class ProcessorFactory
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
