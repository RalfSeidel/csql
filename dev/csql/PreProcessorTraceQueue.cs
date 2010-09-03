using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// Buffer to queue the messages recieved from stdout or stderr of the
	/// the preprocessor queue;
	/// </summary>
	/// <remarks>
	/// Any message recieved from the pre processor is stored in 
	/// this trace queue. The messages are traced by the main 
	/// thread to be able to identify the trace messages by
	/// the thread id.
	/// </remarks>
	internal class PreProcessorTraceQueue
	{
		private readonly List<string> messages = new List<string>();

		public PreProcessorTraceQueue()
		{
		}

		public void AddInfoMessage( string message )
		{
			if ( GlobalSettings.Verbosity.TraceInfo ) {
				AddMessage( message );
			}
		}

		public void AddErrorMessage( string message )
		{
			if ( GlobalSettings.Verbosity.TraceError ) {
				AddMessage( message );
			}
		}

		public void Flush()
		{
			if ( this.messages.Count == 0 )
				return;

			lock ( this.messages ) {
				foreach ( var item in this.messages ) {
					Trace.WriteLine( item );
				}
				this.messages.Clear();
			}
		}

		private void AddMessage( string message )
		{
			if ( String.IsNullOrEmpty( message ) )
				return;

			lock ( this.messages ) {
				this.messages.Add( message );
			}
		}
	}
}
