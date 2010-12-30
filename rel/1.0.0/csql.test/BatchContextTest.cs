using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csql.Test
{
	/// <summary>
	/// Unit tests for the class <see cref="T:BatchContext"/>
	/// </summary>
	[TestClass]
	public class BatchContextTest
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void ConstructorTest()
		{
			string script = "scriptname";
			BatchContext context = new BatchContext( 1, script, 2 );

			Assert.AreEqual( script, context.File );
			Assert.AreEqual( 1, context.BatchOffset );
			Assert.AreEqual( 2, context.LineNumber );
		}

		[TestMethod]
		public void SetLineNumberTest()
		{
			string script = "scriptname";
			BatchContext context = new BatchContext( 1, script, 2 );

			context.LineNumber = 10;
			Assert.AreEqual( 10, context.LineNumber );
		}

		[TestMethod]
		public void SetFileTest()
		{
			string script = "scriptname";
			BatchContext context = new BatchContext( 1, script, 2 );

			context.File = "test";
			Assert.AreEqual( "test", context.File );
		}
	}
}
