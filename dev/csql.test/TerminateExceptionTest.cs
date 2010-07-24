using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using csql;

namespace CSql.Test
{
	/// <summary>
	/// Summary description for TerminateExceptionTest
	/// </summary>
	[TestClass]
	public class TerminateExceptionTest
	{
		[TestMethod]
		public void ConstrutorSuccessCodeTest()
		{
			TerminateException ex = new TerminateException( ExitCode.Success );
			Assert.AreEqual( ExitCode.Success, ex.ExitCode );
		}

		[TestMethod]
		public void ConstrutorFailureCodeTest()
		{
			TerminateException ex = new TerminateException( ExitCode.ArgumentsError );
			Assert.AreEqual( ExitCode.ArgumentsError, ex.ExitCode );
		}
	}
}
