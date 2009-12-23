using System.Drawing;
using System.IO;
using System.Reflection;

namespace csql.addin.Globals
{
	public static class ExtensionMethods
	{
		public static void Log( this string logMessage )
		{
			System.Diagnostics.Debug.WriteLine( logMessage );
		}
	}
}
