using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace csql
{
	[Serializable]
	public class DbException : System.Data.Common.DbException
	{
		private readonly DbMessage m_message;

		public DbException()
		{
		}

		public DbException( string message )
			: base( message )
		{
			m_message = new DbMessage( message );
		}

		public DbException( string message, Exception innerException )
			: base( message, innerException )
		{
			m_message = new DbMessage( message );
		}

		protected DbException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
			: base( info, context ) 
		{ 
		}

		public DbException( DbMessage message )
			: base( message.Message )
		{
			m_message = message;
		}

		public DbException( DbMessage message, Exception innerException )
			: base( message.Message, innerException )
		{
			m_message = message;
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
		public override string Message
		{
			get { return m_message.Message; }
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
			base.GetObjectData( info, context );

            info.AddValue( "Message", m_message );
        }
	}

}

