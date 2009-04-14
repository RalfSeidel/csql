using System;
using System.Collections.Generic;
using System.Text;

namespace csql.Sybase
{
	/// <summary>
	/// Specialization of a message returned by a server for sybase ase server.
	/// </summary>
	public class SybaseMessageEventArgs : DbMessageEventArgs
	{
		public SybaseMessageEventArgs( string message ) : base( message )
		{
		}

		public SybaseMessageEventArgs( SybaseError message ) : base( message.Server, "", message.Procedure, message.LineNumber, message.Message )
		{
		}
	}
}
