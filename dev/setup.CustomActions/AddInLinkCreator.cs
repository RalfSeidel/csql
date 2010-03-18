using System;
using System.Configuration.Install;
using System.IO;

namespace Setup.CustomActions
{
    public class AddInLink
    {
        private InstallContext installContext;

        public AddInLink(InstallContext installContext)
        {
            this.installContext = installContext;
        }


        public void CreateAddInLinks(string PathToAddInFile)
        {
            installContext.LogMessage("Creating AddIn - Links");



            if (Directory.Exists(GetVS2005Folder()))
            {
                CreateDirectoryIfNotExisting(GetVS2005Folder() + @"\Addins");
                File.Copy(PathToAddInFile, GetVS2005Folder() + @"\Addins\csql.AddIn",true);
            }

            if (Directory.Exists(GetVS2008Folder()))
            {
                CreateDirectoryIfNotExisting(GetVS2008Folder() + @"\Addins");
				File.Copy( PathToAddInFile, GetVS2008Folder() + @"\Addins\csql.AddIn", true );
            }

            if (Directory.Exists(GetVS2010Folder()))
            {
                CreateDirectoryIfNotExisting(GetVS2010Folder() + @"\Addins");
				File.Copy( PathToAddInFile, GetVS2010Folder() + @"\Addins\csql.AddIn", true );
            }
        }



        public void RemoveAddInLinks()
        {
            installContext.LogMessage("Removing AddIn - Links");

            RemoveFileIfExists(GetVS2005Folder() + @"\Addins\sqtpp.AddIn");
            RemoveFileIfExists(GetVS2008Folder() + @"\Addins\sqtpp.AddIn");
            RemoveFileIfExists(GetVS2010Folder() + @"\Addins\sqtpp.AddIn");
        }

		private void CreateDirectoryIfNotExisting( string directoryPath )
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }


		private void RemoveFileIfExists( string filename )
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }


		private string GetVS2005Folder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2005";
        }

		private string GetVS2008Folder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2008";
        }

		private string GetVS2010Folder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2010";
        }
    }
}
