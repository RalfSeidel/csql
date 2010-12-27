using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using csql.exe;

namespace csql
{
	/// <summary>
	/// Summary description for CmdArgsTest
	/// </summary>
	[TestClass]
	public class CmdArgsTest
	{
		/// <summary>
		/// Verify that specifieing a user name in the command line arguments
		/// will implicit switch off "integrated security".
		/// </summary>
		[TestMethod]
		public void ImpliciteIntegratedSecurityTest1()
		{
			CmdArgs args = new CmdArgs();
			args.User = "Test";

			CSqlOptions resultingOptions = args.CreateCsqlOptions();
			Assert.IsFalse( resultingOptions.ConnectionParameter.IntegratedSecurity );
		}

		/// <summary>
		/// Verify that "integrated security" is switched on if the command line
		/// argument do not defined a user login id.
		/// </summary>
		[TestMethod]
		public void ImpliciteIntegratedSecurityTest2()
		{
			CmdArgs args = new CmdArgs();

			CSqlOptions resultingOptions = args.CreateCsqlOptions();
			Assert.IsTrue( resultingOptions.ConnectionParameter.IntegratedSecurity );
		}

		/// <summary>
		/// Verify that "integrated security" is switched on if the command line
		/// argument do not defined an empty user name for the login id.
		/// </summary>
		[TestMethod]
		public void ImpliciteIntegratedSecurityTest3()
		{
			CmdArgs args = new CmdArgs();
			args.User = string.Empty;

			CSqlOptions resultingOptions = args.CreateCsqlOptions();
			Assert.IsTrue( resultingOptions.ConnectionParameter.IntegratedSecurity );
		}
	}
}
