using System;

namespace Sqt.DbcProvider.Provider.Sybase
{
	/// <summary>
	/// Specialization of a wrapped database connection for Sybase ASE provider exceptions.
	/// </summary>
	[Serializable]
	public class SybaseWrappedException : WrappedDbException
	{
		public SybaseWrappedException() 
		{
		}

		public SybaseWrappedException( string message ) : base( message ) 
		{ 
		}

		public SybaseWrappedException( string message, Exception inner ) : base( message, inner ) 
		{ 
		}

		public SybaseWrappedException( SybaseMessage sybaseError, Exception innerException )
			: base( sybaseError, innerException )
		{
		}

		protected SybaseWrappedException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context ) 
			: base( info, context ) 
		{ 
		}

	}
}
