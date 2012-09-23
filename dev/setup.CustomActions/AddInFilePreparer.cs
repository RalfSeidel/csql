using System;
using System.Configuration.Install;
using System.IO;

namespace Setup.CustomActions
{
	/// <summary>
	/// Retrieves the addin file template and replaces the variables with the 
	/// values used on the target machine.
	/// </summary>
	static class AddInFilePreparer
	{
		public static void Prepare( string pathToXmlAddInFile, string pathToAddInAssembly, InstallContext installContext )
		{
			installContext.LogMessage( "Preparing AddIn - File " + pathToXmlAddInFile + " with value " + pathToAddInAssembly );

			string fileContent = ReadFile( pathToXmlAddInFile );
			fileContent = fileContent.Replace( @"%AssemblyPath%", pathToAddInAssembly );
			WriteFile( pathToXmlAddInFile, fileContent );
		}


		private static string ReadFile( String sFilename )
		{
			string sContent = string.Empty;

			if ( File.Exists( sFilename ) ) {
				StreamReader myFile = new StreamReader( sFilename, System.Text.Encoding.Default );
				sContent = myFile.ReadToEnd();
				myFile.Close();
			}
			return sContent;
		}


		private static void WriteFile( String sFilename, String sLines )
		{
			StreamWriter myFile = new StreamWriter( sFilename );
			myFile.Write( sLines );
			myFile.Close();
		}
	}
}
