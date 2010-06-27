using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sqt.DbcProvider.Gui;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Tests for <see cref="T:DbConnectionHistoryViewModel.CatalogList"/> 
	/// </summary>
	[TestClass]
	public class DbConnectionViewModelComboListTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the
		/// current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		public DbConnectionViewModelComboListTest()
		{
		}

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


		/// <summary>
		/// Create a catalog list and check that it contains exactly one item
		/// with the IsRefresh property.
		/// </summary>
		[TestMethod]
		public void CatalogListCreationTest()
		{
			var list = new DbConnectionViewModel.ComboList();
			var count = list.Count();
			Assert.AreEqual( 0, count );
		}


		/// <summary>
		/// Create a catalog list.
		/// </summary>
		[TestMethod]
		public void CatalogListCreationTest2()
		{
			var catalogs  = new string[] { "tempdb", "master" };
			var list = new DbConnectionViewModel.ComboList( catalogs );
			var count = list.Count();
			Assert.AreEqual( 2, count );

			var isRefreshCount = list.Count( item => item.IsRefresh );
			Assert.AreEqual( 0, isRefreshCount );
		}
	}
}
