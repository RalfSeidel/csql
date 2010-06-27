using EnvDTE;
using EnvDTE80;
using System.Diagnostics;
using System;
using System.Globalization;

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
		/// Constructor.
		/// </summary>
		public OutputPaneTraceListener( OutputWindowPane outputWindowPane )
		{
			this.outputWindowPane = outputWindowPane;
			this.threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			this.Filter = new ThreadTraceFilter( threadId );
		}

        
		/// <inheritdoc/>
		public override void Write( string message )
		{
			if ( outputWindowPane != null && this.threadId == System.Threading.Thread.CurrentThread.ManagedThreadId ) {
				outputWindowPane.OutputString( message );
			}
		}

		/// <inheritdoc/>
		public override void WriteLine( string message )
		{
			Write( message + "\r\n" );
		}



		private class ThreadTraceFilter : TraceFilter
		{
			private readonly string threadId;

			public ThreadTraceFilter( int threadId )
			{
				this.threadId = threadId.ToString( CultureInfo.InvariantCulture );
			}

			public override bool ShouldTrace( TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data )
			{
				return cache.ThreadId == this.threadId;
			}
		}
	}
}



