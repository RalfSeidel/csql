using System;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace csql.Sybase
{
	[DebuggerDisplay( "{Message}" )]
	public class SybaseError : DbMessage
	{
		private readonly int    m_errorNumber;


		/// <summary>
		/// Initializes a new instance of the <see cref="SybaseInfoMessage"/> class.
		/// </summary>
		/// <param name="aseError">
		/// The unknown error/info message.
		/// </param>
		public SybaseError( object aseError )
			: base( GetServer( aseError ), GetCatalog( aseError ), GetProcedure( aseError ), GetLineNumber( aseError ), GetMessage( aseError ) )
		{
			if ( aseError == null )
				throw new ArgumentNullException( "aseError" );

			Type infoMessageType = aseError.GetType();
			PropertyInfo property;

			property = infoMessageType.GetProperty( "MessageNumber", typeof( int ) );
			m_errorNumber = (int)property.GetValue( aseError, null );
		}

		private static string GetServer( object aseError )
		{
			Type infoMessageType = aseError.GetType();
			PropertyInfo property = infoMessageType.GetProperty( "ServerName", typeof( string ) );
			string server = (string)property.GetValue( aseError, null );
			return server;
		}


		[SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "aseError", Justification="Parameter reserved for later use." )]
		private static string GetCatalog( object aseError )
		{
			return "";
		}

		private static string GetProcedure( object aseError )
		{
			Type infoMessageType = aseError.GetType();
			PropertyInfo property = infoMessageType.GetProperty( "ProcName", typeof( string ) );
			string procedure  = (string)property.GetValue( aseError, null );
			return procedure;
		}

		private static int GetLineNumber( object aseError )
		{
			Type infoMessageType = aseError.GetType();
			PropertyInfo property = infoMessageType.GetProperty( "LineNum", typeof( int ) );
			int lineNumber = (int)property.GetValue( aseError, null );
			return lineNumber;
		}

		private static string GetMessage( object aseError )
		{
			Type infoMessageType = aseError.GetType();
			PropertyInfo property = infoMessageType.GetProperty( "Message", typeof( string ) );
			string message = (string)property.GetValue( aseError, null );
			return message;
		}
		



		/// <summary>
		/// Gets the sybase error number of the message.
		/// </summary>
		/// <value>The error number.</value>
		public int ErrorNumber { get { return m_errorNumber; } }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( Message );
			string result = sb.ToString();
			return result;
		}
	}
}
