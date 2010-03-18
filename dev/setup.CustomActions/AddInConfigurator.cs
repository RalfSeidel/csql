using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace Setup.CustomActions
{
    [RunInstaller(true)]
    public class AddInConfigurator : Installer
    {

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

			var xmlFile = Context.Parameters["addinxmlfile"];
			var dllFile = Context.Parameters["addinassembly"];
			new AddInFilePreparer( xmlFile, dllFile, this.Context );
            new AddInLink(this.Context).CreateAddInLinks( xmlFile );
        }



        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            new AddInLink(this.Context).RemoveAddInLinks();
        }



        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);

            new AddInLink(this.Context).RemoveAddInLinks();
        }
    }
}
