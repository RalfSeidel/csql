using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace csql.ResultTrace
{
	/// <summary>
	/// Summary description for DateTimeColumnFormatTest
	/// </summary>
	[TestClass]
	public class DateTimeColumnFormatTest
	{
		[TestMethod]
		public void FormatNullTest()
		{
			DateTimeColumnFormat format = new DateTimeColumnFormat();
			var result = format.Format( DBNull.Value );
			Assert.AreEqual( ColumnFormat.NullText, result );
		}

		[TestMethod]
		public void DefaultFormatTest1()
		{
			DateTimeColumnFormat format = new DateTimeColumnFormat();
			var result = format.Format( new DateTime( 2000, 5, 1, 9, 30, 10, 500 ) );
			Assert.AreEqual( "2000-05-01 09:30:10.500", result );
		}

		[TestMethod]
		public void DefaultFormatTest2()
		{
			DateTimeColumnFormat format = new DateTimeColumnFormat();
			var result = format.Format( new DateTime( 1900, 1, 1, 0, 0, 0, 0 ) );
			Assert.AreEqual( "1900-01-01 00:00:00.000", result );
		}

		[TestMethod]
		public void TestAutoFormatWithDateOnlyValues()
		{
			List<object> values = new List<object>();
			values.Add( new DateTime( 1900, 1, 1 ) );
			values.Add( new DateTime( 2099, 12, 31 ) );

			DateTimeColumnFormat format = new DateTimeColumnFormat();
			format.AutoFormat( values, false );

			string formatString = format.FormatString;
			Assert.AreEqual( "{0:yyyy-MM-dd}", formatString );
		}

		[TestMethod]
		public void TestAutoFormatWithDateTimeValues()
		{
			List<object> values = new List<object>();
			values.Add( new DateTime( 1900, 1, 1 ) );
			values.Add( new DateTime( 2099, 12, 31 ) );
			values.Add( new DateTime( 1900, 1, 1, 0, 0, 0, 500 ) );
			values.Add( new DateTime( 2099, 12, 31, 23, 59, 59, 0  ) );

			DateTimeColumnFormat format = new DateTimeColumnFormat();
			format.AutoFormat( values, false );

			string formatString = format.FormatString;
			Assert.AreEqual( "{0:yyyy-MM-dd HH':'mm':'ss'.'fff}", formatString );
		}
	}
}
