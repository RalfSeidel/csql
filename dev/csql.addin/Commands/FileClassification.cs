using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csql.addin.Commands
{
	internal static class FileClassification
	{
		/// <summary>
		/// Checks if the extension of the specified file matches with one
		/// of the extensions recognized as sql script files.
		/// </summary>
		/// <param name="fileName">The file name</param>
		/// <returns>
		/// <c>true</c> if the specified file name ends with a sql script extension; otherwise, <c>false</c>.
		/// </returns>
		internal static bool IsSqlScript( string fileName )
		{
			fileName = fileName.ToLowerInvariant();
			bool isSqlScript = fileName.EndsWith( ".csql" ) || fileName.EndsWith( ".sql" );
			return isSqlScript;
		}
	}
}
