using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
	/// <summary>
	/// Supported database systems.
	/// </summary>
	public enum DbSystem
	{
		/// <summary>
		/// Microsoft SQL Server
		/// </summary>
		MsSql,
		/// <summary>
		/// Microsoft JET Database
		/// </summary>
		MsJet,
		/// <summary>
		/// Sybase ASE Server
		/// </summary>
		Sybase,
		/// <summary>
		/// Oracle
		/// </summary>
		Oracle,
		/// <summary>
		/// IBM DB/2
		/// </summary>
		IbmDb2
	}
}
