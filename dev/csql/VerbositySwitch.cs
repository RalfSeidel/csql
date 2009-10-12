using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// Trace switch for the verbosity level passed in the command line arguments.
	/// </summary>
	public class VerbositySwitch : System.Diagnostics.TraceSwitch
	{
		public VerbositySwitch()
		: base( "Verbosity", "Trace switch for the \"Verbose\" command line argument.", TraceLevel.Warning.ToString() )
		{
		}
	}
}
