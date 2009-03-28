using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// Trace switch for the verbosity level passed in the command line arguments.
	/// </summary>
	public class VerbosityLevel : System.Diagnostics.TraceSwitch
	{
		public VerbosityLevel()
		: base( "Verbosity", "Trace switch for the \"Verbose\" command line argument.", TraceLevel.Warning.ToString() )
		{
		}
	}
}
