using System;
using System.Configuration.Install;
using System.IO;

namespace Setup.CustomActions
{
    class AddInFilePreparer
    {
        public AddInFilePreparer(string pathToXmlAddInFile, string pathToAddInAssembly, InstallContext installContext)
        {
            installContext.LogMessage("Preparing AddIn - File " + pathToXmlAddInFile + " with value " + pathToAddInAssembly);

            string fileContent = ReadFile(pathToXmlAddInFile);
            fileContent = fileContent.Replace(@"%AssemblyPath%", pathToAddInAssembly);
            WriteFile(pathToXmlAddInFile, fileContent);
        }


		private string ReadFile( String sFilename )
        {
            string sContent = "";

            if (File.Exists(sFilename))
            {
                StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                sContent = myFile.ReadToEnd();
                myFile.Close();
            }
            return sContent;
        }


		private void WriteFile( String sFilename, String sLines )
        {
            StreamWriter myFile = new StreamWriter(sFilename);
            myFile.Write(sLines);
            myFile.Close();
        }


    }
}
