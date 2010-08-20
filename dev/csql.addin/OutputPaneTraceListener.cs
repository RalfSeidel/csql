using EnvDTE;

namespace csql.addin
{
	/// <summary>
	/// Trace listener that emit the trace messages to the csql output window pane.
	/// </summary>
	internal class OutputPaneTraceListener : System.Diagnostics.TraceListener
	{
		private readonly OutputWindowPane outputWindowPane;
		private readonly int threadId;

		/// <summary>
		/// Constructor with specified output window pane.
		/// </summary>
		public OutputPaneTraceListener( OutputWindowPane outputWindowPane )
		{
			this.outputWindowPane = outputWindowPane;
			this.threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
		}

        
		/// <inheritdoc/>
		public override void Write( string message )
		{
			//if ( outputWindowPane != null && this.threadId == System.Threading.Thread.CurrentThread.ManagedThreadId ) {
				outputWindowPane.OutputString( message );
			//}
		}

		/// <inheritdoc/>
		public override void WriteLine( string message )
		{
			Write( message + "\r\n" );
		}
	}
}



