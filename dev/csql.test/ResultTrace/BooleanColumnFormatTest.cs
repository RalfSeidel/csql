using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csql.ResultTrace
{
	/// <summary>
	/// Summary description for BooleanColumnFormatTest
	/// </summary>
	[TestClass]
	public class BooleanColumnFormatTest
	{
		[TestMethod]
		public void FormatTrueTest()
		{
			BooleanColumnFormat format = new BooleanColumnFormat();
			var result = format.Format( true );
			Assert.AreEqual( "1", result );
		}

		[TestMethod]
		public void FormatFalseTest()
		{
			BooleanColumnFormat format = new BooleanColumnFormat();
			var result = format.Format( false );
			Assert.AreEqual( "0", result );
		}

		[TestMethod]
		public void FormatNullTest()
		{
			BooleanColumnFormat format = new BooleanColumnFormat();
			var result = format.Format( DBNull.Value );
			Assert.AreEqual( "-", result );
		}
	}
}
