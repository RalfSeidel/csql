using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
	/// <summary>
	/// The supported driver / provider to use to establish the database server connection.
	/// </summary>
	public enum DbDriver
	{
		/// <summary>
		/// Use the program default of the specified provider.
		/// </summary>
		Default,
		/// <summary>
		/// Use the native ADO.NET driver if available.
		/// </summary>
		Native,
		/// <summary>
		/// Force using the OLEDB provider.
		/// </summary>
		OleDb,
		/// <summary>
		/// Force using the ODBC provider.
		/// </summary>
		Odbc
	}
}
