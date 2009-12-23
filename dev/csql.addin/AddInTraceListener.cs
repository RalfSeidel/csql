using EnvDTE;
using EnvDTE80;

namespace csql.addin
{
	class AddInTraceListener : System.Diagnostics.TraceListener
	{
		private DTE2 dte;
		private OutputWindowPane outputWindowPane;

		public AddInTraceListener( DTE2 dte )
		{
			this.dte = dte;
		}

		public override void Write( string message )
		{
			if ( outputWindowPane == null ) {
				if ( this.dte.ToolWindows.OutputWindow == null )
					return;
				outputWindowPane = this.dte.ToolWindows.OutputWindow.OutputWindowPanes.Add( "csql" );
			}

			outputWindowPane.OutputString( message );

			outputWindowPane.Activate();
		}

		public override void WriteLine( string message )
		{
			Write( message + "\r\n" );
		}

	}
}



