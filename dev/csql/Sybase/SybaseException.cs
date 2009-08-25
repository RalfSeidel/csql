using System;

namespace csql.Sybase
{
	[Serializable]
	public class SybaseException : DbException
	{
		public SybaseException() {}
		public SybaseException( string message ) : base( message ) { }
		public SybaseException( string message, Exception inner ) : base( message, inner ) { }
		protected SybaseException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
		public SybaseException( SybaseError sybaseError, Exception innerException )
		: base( sybaseError, innerException ) 
		{ 
		}
	}
}
