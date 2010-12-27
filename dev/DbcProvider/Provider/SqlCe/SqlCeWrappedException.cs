using System;

namespace Sqt.DbcProvider.Provider.SqlCe
{
	/// <summary>
	/// Specialization of a wrapped database connection for SQL CE provider exceptions.
	/// </summary>
	[Serializable]
	public class SqlCeWrappedException : WrappedDbException
	{
		public SqlCeWrappedException() 
		{
		}

		public SqlCeWrappedException( string message ) : base( message ) 
		{ 
		}

		public SqlCeWrappedException( string message, Exception inner ) : base( message, inner ) 
		{ 
		}

		public SqlCeWrappedException( SqlCeMessage sqlceError, Exception innerException )
			: base( sqlceError, innerException )
		{
		}

		protected SqlCeWrappedException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context ) 
			: base( info, context ) 
		{ 
		}
	}
}
