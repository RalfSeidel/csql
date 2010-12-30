using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sqt.DbcProvider
{
	[TestClass]
	public class MruDbConnectionParameterAdapterTest
	{
		[TestMethod]
		public void GetMruDbConnectionParameterTest()
		{
			var datasources1 = new Datasources() {
				Provider = ProviderType.Oracle
			};

			var datasource1_1 = new Datasource() {
				Address = "datasource1_1"
			};
			var datasourcesList1 = new List<Datasource>();
			datasourcesList1.Add( datasource1_1 );
			datasources1.Datasource = datasourcesList1;

			var datasources2 = new Datasources();
			datasources2.Provider = ProviderType.Sybase;

			var history = new MruConnections()
			{
				Datasources = new List<Datasources>()
			};
			history.Datasources.Add( datasources1 );
			history.Datasources.Add( datasources2 );


			DbConnectionParameter murParameter = MruDbConnectionParameterAdapter.GetMruDbConnectionParameter( history );
			Assert.AreEqual( datasources1.Provider, murParameter.Provider );
			Assert.AreEqual( datasource1_1.Address, murParameter.DatasourceAddress );

		}

		[TestMethod]
		public void SetMruDbConnectionParameterTest1()
		{
			MruConnections mruConnections = new MruConnections();
			DbConnectionParameter parameter = new DbConnectionParameter();
			parameter.Provider = ProviderType.MsSql;
			parameter.DatasourceAddress = "server";
			parameter.DatasourcePort = 4711;
			parameter.Catalog = "catalog";
			parameter.UserId = "test";
			parameter.Password = "password";

			MruDbConnectionParameterAdapter.SetMruDbConnectionParameter( mruConnections, parameter );

			DbConnectionParameter mruParameter = MruDbConnectionParameterAdapter.GetMruDbConnectionParameter( mruConnections );

			Assert.AreEqual( parameter, mruParameter );
		}

	}
}
