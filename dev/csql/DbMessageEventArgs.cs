using System;
using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// An info message send by the SQL server encapsulated into an event argument structure.
	/// </summary>
	public class DbMessageEventArgs : EventArgs
	{
		private readonly DbMessage message;

		public DbMessageEventArgs( string message )
		{
			this.message = new DbMessage( message );
		}

		public DbMessageEventArgs( TraceLevel severity, string server, string catalog, string procedure, int lineNo, string message )
		{
			this.message = new DbMessage( severity, server, catalog, procedure, lineNo, message );
		}

		public string Server
		{
			get { return this.message.Server; }
		}

		public string Catalog
		{
			get { return this.message.Catalog; }
		}

		public string Procedure
		{
			get { return this.message.Procedure; }
		}

		public int LineNumber
		{
			get { return this.message.LineNumber; }
		}

		public TraceLevel TraceLevel
		{
			get { return this.message.TraceLevel; }
		}

		/// <summary>
		/// Gets the message text.
		/// </summary>
		/// <value>The message text.</value>
		public string Message
		{
			get { return this.message.Message; }
		}

		public override string ToString()
		{
			return this.message.ToString();
		}
	}
}
