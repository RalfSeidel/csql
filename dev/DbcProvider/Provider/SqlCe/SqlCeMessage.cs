using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data.SqlServerCe;
using System.Diagnostics.CodeAnalysis;

namespace Sqt.DbcProvider.Provider.SqlCe
{
	[DebuggerDisplay( "{Message}" )]
	public class SqlCeMessage : DbMessage
	{
		/// <summary>
		/// The native SQL CE error number.
		/// </summary>
		private readonly int errorNumber;

		public SqlCeMessage( SqlCeError sqlceError )
			: this( TraceLevel.Error, sqlceError  )
		{
			this.errorNumber = sqlceError.NativeError;
		}

		public SqlCeMessage( TraceLevel serverity, SqlCeError sqlceError )
			: base( serverity, GetServer( sqlceError ), GetCatalog( sqlceError ), GetProcedure( sqlceError ), GetLineNumber( sqlceError ), GetMessage( sqlceError ) )
		{
			this.errorNumber = sqlceError.NativeError;
		}

		#region SQL CE exception property extraction.

		[SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="sqlceError", Justification="Parameter reserved for later use." )]
		private static string GetServer( SqlCeError sqlceError )
		{
			return string.Empty;
		}

		[SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="sqlceError", Justification="Parameter reserved for later use." )]
		private static string GetCatalog( SqlCeError sqlceError )
		{
			return string.Empty;
		}

		[SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="sqlceError", Justification="Parameter reserved for later use." )]
		private static string GetProcedure( SqlCeError sqlceError )
		{
			return string.Empty;
		}

		private static int GetLineNumber( SqlCeError sqlceError )
		{
			int lineNumber = 0;
			return lineNumber;
		}

		private static string GetMessage( SqlCeError sqlceError )
		{
			return sqlceError.Message;
		}

		#endregion

	}
}
