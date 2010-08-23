using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csql.Test
{
	/// <summary>
	/// Unit tests for the class <see cref="T:ProcessorContext"/>
	/// </summary>
	[TestClass]
	public class ProcessorContextTest
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
		public void ConstructorTest1()
		{
			string script = "scriptname";
			ProcessorContext context = new ProcessorContext( script );

			Assert.AreEqual( script, context.CurrentFile );
			Assert.AreEqual( 1, context.CurrentBatchLineNo );
		}


		[TestMethod]
		public void AppendLineTest()
		{
			string script = "scriptname";
			ProcessorContext context = new ProcessorContext( script );
			context.AppendLine( "content" );
			Assert.AreEqual( "content", context.CurrentBatch.TrimEnd( '\r', '\n')  );
			Assert.AreEqual( 2, context.CurrentBatchLineNo );
			Assert.AreEqual( 2, context.CurrentFileLineNumber );
		}


		[TestMethod]
		public void StartNextBatchTest()
		{
			string script = "scriptname";
			ProcessorContext context = new ProcessorContext( script );
			context.AppendLine( "content" );

			context.StartNextBatch();

			Assert.AreEqual( "", context.CurrentBatch );
			Assert.AreEqual( 1, context.CurrentBatchLineNo );
			Assert.AreEqual( 2, context.CurrentFileLineNumber );
		}

	}
}
