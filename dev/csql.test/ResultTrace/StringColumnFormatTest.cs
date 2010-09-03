using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csql.ResultTrace
{
	/// <summary>
	/// Summary description for StringColumnFormatTest
	/// </summary>
	[TestClass]
	public class StringColumnFormatTest
	{
		[TestMethod]
		public void FormatNullTest()
		{
			StringColumnFormat format = new StringColumnFormat(10);
			var result = format.Format( DBNull.Value );
			Assert.AreEqual( ColumnFormat.NullText, result );
		}

		[TestMethod]
		public void FormatShorterThanMaxWidthTest()
		{
			StringColumnFormat format = new StringColumnFormat( 10 );
			var result = format.Format( "A" );
			Assert.AreEqual( "A", result );
		}

		[TestMethod]
		public void FormatLongerThanMaxWidthTest()
		{
			StringColumnFormat format = new StringColumnFormat( 10 );
			var result = format.Format( "ABCDEFGHIJKLMNOPQRSTUVWXYZ" );
			Assert.AreEqual( "ABCDEFGHIJKLMNOPQRSTUVWXYZ", result );
		}

		[TestMethod]
		public void AutoFormatTest1()
		{
			StringColumnFormat format = new StringColumnFormat( 10 );

			string[] values = new string[] { "1", "12", "123", "1234", "123456", "1234567" };
			format.AutoFormat( values, false );

			Assert.AreEqual( 7, format.MaxWidth );
		}

		[TestMethod]
		public void AutoFormatTest2()
		{
			StringColumnFormat format = new StringColumnFormat( 10 );

			string[] values = new string[] { "1", "12" };
			format.AutoFormat( values, false );

			// Not less than the characters required to display null values.
			Assert.AreEqual( ColumnFormat.NullText.Length, format.MaxWidth );
		}
	}
}
