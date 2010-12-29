using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using Microsoft.Win32;
using System;
using System.IO;
using System.Diagnostics;

namespace Setup.CustomActions
{
	/// <summary>
	/// Installer extensions.
	/// </summary>
    [RunInstaller(true)]
    public class AddInConfigurator : Installer
    {
		/// <summary>
		/// Perform the installation.
		/// </summary>
		/// <param name="stateSaver">
		/// An <see cref="T:System.Collections.IDictionary"/> used to save information needed
		/// to perform a commit, rollback, or uninstall operation.
		/// </param>
		/// <exception cref="T:System.ArgumentException">
		/// The <paramref name="stateSaver"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Exception">
		/// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeInstall"/> event handler of one of the installers in the collection.
		/// -or-
		/// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.AfterInstall"/> event handler of one of the installers in the collection.
		/// </exception>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
	
			// Save target directory and product id to be able to set the install location
			// Note that you have to add the following line to the "CustomActionData" property
			// in the install event of your primary output:
			//	/DP_TargetDir="[TARGETDIR]\" /DP_ProductID="[ProductCode]"
			// To do so select the setup project in the solution and click the icon "Custom Actions Editor"
			// at the top of the solution explorer.
			object productId = Context.Parameters["DP_ProductID"];
			object targetDir = Context.Parameters["DP_TargetDir"];
			if ( productId == null ) {
				throw new NotSupportedException( "The product id is not defined." );
			}
			if ( targetDir == null ) {
				throw new NotSupportedException( "The targetDirectory is not defined." );
			}

			stateSaver.Add( "ProductID", productId.ToString() );
			stateSaver.Add( "TargetDir", targetDir.ToString() );

			var directory = targetDir.ToString() + Path.DirectorySeparatorChar;
			var xmlFile = directory + "csql.addin";
			var dllFile = directory + "csql.addin.dll";
			AddInFilePreparer.Prepare( xmlFile, dllFile, this.Context );
            new AddInLink(this.Context).CreateAddInLinks( xmlFile );
        }


		/// <summary>
		/// Remove the installation.
		/// </summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary"/> that contains the state of the computer after the installation was complete.</param>
		/// <exception cref="T:System.ArgumentException">
		/// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
		/// </exception>
		/// <exception cref="T:System.Configuration.Install.InstallException">
		/// An exception occurred while uninstalling. This exception is ignored and the uninstall continues. However, the application might not be fully uninstalled after the uninstallation completes.
		/// </exception>
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            new AddInLink(this.Context).RemoveAddInLinks();
        }


		/// <summary>
		/// Completes the install transaction.
		/// </summary>
		/// <param name="savedState">
		/// An <see cref="T:System.Collections.IDictionary"/> 
		/// that contains the state of the computer after all the installers in the collection have run.
		/// </param>
		/// <exception cref="T:System.ArgumentException">
		/// The <paramref name="savedState"/> parameter is null.
		/// -or-
		/// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
		/// </exception>
		/// <exception cref="T:System.Configuration.Install.InstallException">
		/// An exception occurred during the <see cref="M:System.Configuration.Install.Installer.Commit(System.Collections.IDictionary)"/>
		/// phase of the installation. This exception is ignored and the installation continues.
		/// However, the application might not function correctly after the installation is complete.
		/// </exception>
        public override void Commit(IDictionary savedState)
        {
			Debugger.Break();

			base.Commit( savedState );

			// Determine install location and update product registry with the information.
			string productId = savedState["ProductID"].ToString();
			string installerKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + productId;
			RegistryKey installerKey = Registry.LocalMachine.OpenSubKey( installerKeyPath, true );

			if ( installerKey != null ) {
				string installLocation = savedState["TargetDir"].ToString();
				installerKey.SetValue( "InstallLocation", installLocation );
				installerKey.Close();
			}
        }

		/// <summary>
		/// Restores the pre-installation state of the computer.
		/// </summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary"/> that contains the pre-installation state of the computer.</param>
		/// <exception cref="T:System.ArgumentException">
		/// The <paramref name="savedState"/> parameter is null.
		/// -or-
		/// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
		/// </exception>
		/// <exception cref="T:System.Configuration.Install.InstallException">
		/// An exception occurred during the <see cref="M:System.Configuration.Install.Installer.Rollback(System.Collections.IDictionary)"/> phase of the installation. This exception is ignored and the rollback continues. However, the computer might not be fully reverted to its initial state after the rollback completes.
		/// </exception>
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);

            new AddInLink(this.Context).RemoveAddInLinks();
        }
    }
}
