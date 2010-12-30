using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csql.ResultTrace
{
	/// <summary>
	/// Summary description for BinaryColumnFormatTest
	/// </summary>
	[TestClass]
	public class BinaryColumnFormatTest
	{
		[TestMethod]
		public void FormatNullTest()
		{
			BinaryColumnFormat format = new BinaryColumnFormat(10);
			var result = format.Format( DBNull.Value );
			Assert.AreEqual( ColumnFormat.NullText, result );
		}

		[TestMethod]
		public void FormatShorterThanMaxWidthTest()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );
			var result = format.Format( new byte[] { 0 } );
			Assert.AreEqual( "0x00", result );
		}

		[TestMethod]
		public void FormatEmptyTest()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );
			var result = format.Format( new byte[] {} );
			Assert.AreEqual( "0x", result );
		}


		[TestMethod]
		public void FormatSingleByteTest1()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );
			var result = format.Format( new byte[] { 0 } );
			Assert.AreEqual( "0x00", result );
		}

		[TestMethod]
		public void FormatSingleByteTest2()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );
			var result = format.Format( new byte[] { 15 } );
			Assert.AreEqual( "0x0F", result );
		}

		[TestMethod]
		public void FormatSingleByteTest3()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );
			var result = format.Format( new byte[] { 255 } );
			Assert.AreEqual( "0xFF", result );
		}

		[TestMethod]
		public void FormatLongerThanMaxWidthTest()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );
			var result = format.Format( new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } );
			Assert.AreEqual( "0x000102030405060708090A", result );
		}

		[TestMethod]
		public void AutoFormatTest1()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );

			byte[][] values = new byte[][] 
			{ 
				new byte[] { 0 },
				new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
			};
			format.AutoFormat( values, false );

			Assert.AreEqual( 22, format.MaxWidth );
		}

		[TestMethod]
		public void AutoFormatTest2()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );

			byte[][] values = new byte[][] 
			{ 
				new byte[] { 0 }
			};
			format.AutoFormat( values, false );

			// Not less than the characters required to display null values.
			Assert.AreEqual( ColumnFormat.NullText.Length, format.MaxWidth );
		}

		[TestMethod]
		public void AutoFormatTest3()
		{
			BinaryColumnFormat format = new BinaryColumnFormat( 10 );

			byte[][] values = new byte[][] 
			{ 
				new byte[] { 0 },
				new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
			};
			format.AutoFormat( values, false );

			Assert.AreEqual( 22, format.MaxWidth );
		}
	}
}
