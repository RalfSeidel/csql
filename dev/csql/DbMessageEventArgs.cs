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
			return m_message.ToString();
		}

	}
}
