using System;
using System.Diagnostics;
using System.Text;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Class for info or error messages send by the servers.
	/// </summary>
	[Serializable]
	public class DbMessage
	{
		private readonly TraceLevel m_severity;
		private readonly string m_server;
		private readonly string m_catalog;
		private readonly string m_procedure;
		private readonly int    m_lineNumber;
		private readonly string m_message;

		public DbMessage( string message )
		{
			this.m_severity = TraceLevel.Info;
			this.m_server = "";
			this.m_catalog = "";
			this.m_procedure = "";
			this.m_lineNumber = -1;
			this.m_message = message;
		}

		public DbMessage( TraceLevel severity, string server, string catalog, string procedure, int lineNo, string message )
		{
			this.m_severity = severity;
			this.m_server = server;
			this.m_catalog = catalog;
			this.m_procedure = procedure;
			this.m_lineNumber = lineNo;
			this.m_message = message;
		}

		/// <summary>
		/// Gets the name of the server which emitted the message.
		/// </summary>
		/// <value>The server name.</value>
		public string Server { get { return m_server; } }


		/// <summary>
		/// Gets the related catalog/database if available.
		/// </summary>
		/// <value>The catalog name.</value>
		public string Catalog { get { return m_catalog; } }

		/// <summary>
		/// Gets the name of the procedure in which the error occurred.
		/// </summary>
		/// <value>The procedure name.</value>
		public string Procedure { get { return m_procedure; } }

		/// <summary>
		/// Gets the line number where the error occurred.
		/// </summary>
		/// <remarks>
		/// The the error was raised by a procedure or trigger the value
		/// specifies the line number in the source code of the object.
		/// Otherwise the value is the line number of the current batch.
		/// </remarks>
		/// <value>The line number.</value>
		public int LineNumber { get { return m_lineNumber; } }

		/// <summary>
		/// Gets the message send by the sybase SQL server.
		/// </summary>
		/// <value>The message.</value>
		public string Message { get { return m_message; } }

		/// <summary>
		/// Gets the trace / verbosity level of the message.
		/// </summary>
		/// <value>The trace level.</value>
		public TraceLevel TraceLevel { get { return m_severity; } }


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			string separator = "";

			if ( !String.IsNullOrEmpty( m_server ) ) {
				sb.Append( separator );
				sb.Append( "Server: " );
				sb.Append( m_server );
				separator = ", ";
			}
			if ( !String.IsNullOrEmpty( m_catalog ) ) {
				sb.Append( separator );
				sb.Append( "Catalog: " );
				sb.Append( m_catalog );
				separator = ", ";
			}

			if ( !String.IsNullOrEmpty( m_procedure ) ) {
				sb.Append( separator );
				sb.Append( "procedure: " );
				sb.Append( m_procedure );
				separator = ", ";
			}

			if ( m_lineNumber > 0 ) {
				sb.Append( separator );
				sb.Append( "Line: " );
				sb.Append( m_lineNumber );
				separator = ", ";
			}

			if ( !String.IsNullOrEmpty( separator ) ) {
				separator = "\r\n";
			}
			sb.Append( separator );
			sb.Append( m_message );
			string result = sb.ToString();
			return result;
		}
	}
}
