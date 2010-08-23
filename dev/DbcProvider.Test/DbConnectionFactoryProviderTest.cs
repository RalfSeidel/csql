using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sqt.DbcProvider.Provider;
using Sqt.DbcProvider.Provider.MsSql;
using Sqt.DbcProvider.Provider.Sybase;
using Sqt.DbcProvider.Provider.IbmDb2;
using Sqt.DbcProvider.Provider.Oracle;

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
			IDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.MsSql );
			Assert.IsInstanceOfType( factory, typeof( MsSqlConnectionFactory) );
		}

		[TestMethod]
		public void GetSybaseFactoryTest()
		{
			IDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.Sybase );
			Assert.IsInstanceOfType( factory, typeof( SybaseConnectionFactory ) );
		}

		[TestMethod]
		public void GetOracleFactoryTest()
		{
			IDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.Oracle );
			Assert.IsInstanceOfType( factory, typeof( OracleConnectionFactory ) );
		}

		[TestMethod]
		public void GetIbmDb2FactoryTest()
		{
			IDbConnectionFactory factory = DbConnectionFactoryProvider.GetFactory( ProviderType.IbmDb2 );
			Assert.IsInstanceOfType( factory, typeof( IbmDb2ConnectionFactory ) );
		}
	}
}