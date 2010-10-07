using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csql.addin.Settings;

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
		public static bool IsSqlScript( CSqlParameter currentPararmeter, string fileName )
		{
			IEnumerable<string> extensions = currentPararmeter.ScriptExtensions;
			if ( extensions == null || !extensions.GetEnumerator().MoveNext() ) {
				extensions = DefaultScriptExtensions;
			}
			foreach ( string extension in extensions ) {
				if ( fileName.EndsWith( extension, StringComparison.InvariantCultureIgnoreCase ) )
					return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the default file extensions which are considered to be sql scripts.
		/// </summary>
		/// <value>The default script extensions.</value>
		private static IEnumerable<string> DefaultScriptExtensions
		{
			get { return new string[] { ".csql", ".sql", ".ins" }; }
		}
	}
}
