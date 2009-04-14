using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
	/// <summary>
	/// An info message send by the SQL server encapsulated into an event argument structure.
	/// </summary>
	public class DbMessageEventArgs : EventArgs
	{
		private readonly DbMessage m_message;

		public DbMessageEventArgs( string message )
		{
			m_message = new DbMessage( message );
		}

		public DbMessageEventArgs( string server, string catalog, string procedure, int lineNo, string message )
		{
			m_message = new DbMessage( server, catalog, procedure, lineNo, message );
		}

		public string Server
		{
			get { return m_message.Server; }
		}

		public string Catalog
		{
			get { return m_message.Catalog; }
		}

		public string Procedure
		{
			get { return m_message.Procedure; }
		}

		public int LineNumber
		{
			get { return m_message.LineNumber; }
		}

		/// <summary>
		/// Gets the message text.
		/// </summary>
		/// <value>The message text.</value>
		public string Message
		{
			get { return m_message.Message; }
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			string separator = "";

			if ( !String.IsNullOrEmpty( Server ) ) {
				sb.Append( separator );
				sb.Append( "Server: " );
				sb.Append( Server );
				separator = ", ";
			}
			if ( !String.IsNullOrEmpty( Catalog ) ) {
				sb.Append( separator );
				sb.Append( "Catalog: " );
				sb.Append( Catalog );
				separator = ", ";
			}

			if ( !String.IsNullOrEmpty( Procedure ) ) {
				sb.Append( separator );
				sb.Append( "Procedure: " );
				sb.Append( Procedure );
				separator = ", ";
			}

			if ( LineNumber > 0 ) {
				sb.Append( separator );
				sb.Append( "Line: " );
				sb.Append( LineNumber );
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
