using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sqt.DbcProvider.Gui;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Tests for <see cref="T:DbConnectionHistoryViewModel.ProviderList"/> 
	/// </summary>
	[TestClass]
	public class DbConnectionViewModelProviderListTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the
		/// current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		public DbConnectionViewModelProviderListTest()
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
		/// Create a provider list.
		/// </summary>
		[TestMethod]
		public void ProviderListDefaultCreationTest()
		{
			var list = DbConnectionViewModel.ProviderList.CreateProviderList();
			AssertContainsAllProvidersExceptUndefined( list );
			AssertContainsNotUndefinedProvider( list );
		}

		/// <summary>
		/// Create a provider list.
		/// </summary>
		[TestMethod]
		public void ProviderListCreationTest2()
		{
			var history = new MruConnections() {
				Datasources = new List<Datasources>()
			};
			var list = DbConnectionViewModel.ProviderList.CreateProviderList( history );
			AssertContainsAllProvidersExceptUndefined( list );
			AssertContainsNotUndefinedProvider( list );
		}

		/// <summary>
		/// Create a provider list.
		/// </summary>
		[TestMethod]
		public void ProviderListCreationTest3()
		{
			var ds1 = new Datasources();
			ds1.Provider = ProviderType.Oracle;

			var ds2 = new Datasources();
			ds2.Provider = ProviderType.Sybase;

			var history = new MruConnections() {
				Datasources = new List<Datasources>()
			};
			history.Datasources.Add( ds1 );
			history.Datasources.Add( ds2 );
			var list = DbConnectionViewModel.ProviderList.CreateProviderList( history );
			var items = list.ToArray();
			Assert.IsTrue( items.Length >= 2 );

			var item1 = items[0];
			Assert.AreEqual( ProviderType.Oracle, item1.ID );

			var item2 = items[1];
			Assert.AreEqual( ProviderType.Sybase, item2.ID );

			AssertContainsAllProvidersExceptUndefined( list );
			AssertContainsNotUndefinedProvider( list );
		}


		private void AssertContainsAllProvidersExceptUndefined( DbConnectionViewModel.ProviderList list )
		{
			var enumValues = System.Enum.GetValues( typeof( ProviderType ) );
			foreach ( ProviderType e in enumValues ) {
				if ( e == ProviderType.Undefined )
					continue;

				var isContained = false;
				foreach ( var item in list ) {
					var itemProvider = item.ID;
					if ( itemProvider == e ) {
						isContained = true;
						break;
					}
				}

				Assert.IsTrue( isContained, "The provider list does not contain " + e.ToString() );
			}
		}

		private void AssertContainsNotUndefinedProvider( DbConnectionViewModel.ProviderList list )
		{
			var enumValues = System.Enum.GetValues( typeof( ProviderType ) );
			foreach ( var item in list ) {
				var itemProvider = item.ID;
				if ( itemProvider == ProviderType.Undefined ) {
					Assert.Fail( "The list is not supposed to contain the undefined provider" );
				}
			}
		}

	}
}
