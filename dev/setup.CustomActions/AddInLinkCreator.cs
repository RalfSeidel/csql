using System;
using System.Configuration.Install;
using System.IO;

namespace Setup.CustomActions
{
	/// <summary>
	/// Helper class to create and remove the csql.addin file in the visual studio addin directory.
	/// </summary>
	public class AddInLink
	{
		private InstallContext installContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="AddInLink"/> class.
		/// </summary>
		/// <param name="installContext">The install context.</param>
		public AddInLink( InstallContext installContext )
		{
			this.installContext = installContext;
		}


		public void CreateAddInLinks( string pathOfAddInFile )
		{
			installContext.LogMessage( "Creating AddIn - Links" );


			if ( Directory.Exists( GetVS2005Folder() ) ) {
				CreateDirectoryIfNotExisting( GetVS2005Folder() + @"\Addins" );
				File.Copy( pathOfAddInFile, GetVS2005Folder() + @"\Addins\csql.AddIn", true );
			}

			if ( Directory.Exists( GetVS2008Folder() ) ) {
				CreateDirectoryIfNotExisting( GetVS2008Folder() + @"\Addins" );
				File.Copy( pathOfAddInFile, GetVS2008Folder() + @"\Addins\csql.AddIn", true );
			}

			if ( Directory.Exists( GetVS2010Folder() ) ) {
				CreateDirectoryIfNotExisting( GetVS2010Folder() + @"\Addins" );
				File.Copy( pathOfAddInFile, GetVS2010Folder() + @"\Addins\csql.AddIn", true );
			}

			if ( Directory.Exists( GetVS2012Folder() ) ) {
				CreateDirectoryIfNotExisting( GetVS2012Folder() + @"\Addins" );
				File.Copy( pathOfAddInFile, GetVS2012Folder() + @"\Addins\csql.AddIn", true );
			}
		}


		public void RemoveAddInLinks()
		{
			installContext.LogMessage( "Removing AddIn - Links" );

			RemoveFileIfExists( GetVS2005Folder() + @"\Addins\csql.AddIn" );
			RemoveFileIfExists( GetVS2008Folder() + @"\Addins\csql.AddIn" );
			RemoveFileIfExists( GetVS2010Folder() + @"\Addins\csql.AddIn" );
			RemoveFileIfExists( GetVS2012Folder() + @"\Addins\csql.AddIn" );
		}

		private static void CreateDirectoryIfNotExisting( string directoryPath )
		{
			if ( !Directory.Exists( directoryPath ) ) {
				Directory.CreateDirectory( directoryPath );
			}
		}

		private static void RemoveFileIfExists( string filename )
		{
			if ( File.Exists( filename ) ) {
				File.Delete( filename );
			}
		}

		private static string GetVS2005Folder()
		{
			return Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\Visual Studio 2005";
		}

		private static string GetVS2008Folder()
		{
			return Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\Visual Studio 2008";
		}

		private static string GetVS2010Folder()
		{
			return Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\Visual Studio 2010";
		}

		private static string GetVS2012Folder()
		{
			return Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\Visual Studio 11";
		}
	}
}
