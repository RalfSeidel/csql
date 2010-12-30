using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sqt.DbcProvider.Provider;
using Sqt.DbcProvider.Provider.IbmDb2;
using Sqt.DbcProvider.Provider.MsSql;
using Sqt.DbcProvider.Provider.Oracle;
using Sqt.DbcProvider.Provider.SqlCe;
using Sqt.DbcProvider.Provider.Sybase;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Tests for <see cref="T:DbConnectionFactoryProvider"/> 
	/// </summary>
	[TestClass]
	public class DbConnectionFactoryProviderTest
	{
		[TestMethod]
		public void GetMsSqlFactoryTest()
		{
			IWrappedDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.MsSql );
			Assert.IsInstanceOfType( factory, typeof( MsSqlConnectionFactory) );
		}

		[TestMethod]
		public void GetSqlCeFactoryTest()
		{
			IWrappedDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.SqlCe );
			Assert.IsInstanceOfType( factory, typeof( SqlCeConnectionFactory ) );
		}

		[TestMethod]
		public void GetSybaseFactoryTest()
		{
			IWrappedDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.Sybase );
			Assert.IsInstanceOfType( factory, typeof( SybaseConnectionFactory ) );
		}

		[TestMethod]
		public void GetOracleFactoryTest()
		{
			IWrappedDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.Oracle );
			Assert.IsInstanceOfType( factory, typeof( OracleConnectionFactory ) );
		}

		[TestMethod]
		public void GetIbmDb2FactoryTest()
		{
			IWrappedDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.IbmDb2 );
			Assert.IsInstanceOfType( factory, typeof( IbmDb2ConnectionFactory ) );
		}
	}
}