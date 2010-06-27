using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sqt.DbcProvider.Provider;

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
			IDbcProvider factory = DbConnectionProviderFactory.GetFactory( ProviderType.MsSql );
			Assert.IsInstanceOfType( factory, typeof(MsSqlConnectionFactory) );
		}

		[TestMethod]
		public void GetSybaseFactoryTest()
		{
			IDbcProvider factory = DbConnectionProviderFactory.GetFactory( ProviderType.Sybase );
			Assert.IsInstanceOfType( factory, typeof( SybaseConnectionFactory ) );
		}

		[TestMethod]
		public void GetOracleFactoryTest()
		{
			IDbcProvider factory = DbConnectionProviderFactory.GetFactory( ProviderType.Oracle );
			Assert.IsInstanceOfType( factory, typeof( OracleConnectionFactory ) );
		}

		[TestMethod]
		public void GetIbmDb2FactoryTest()
		{
			IDbcProvider factory = DbConnectionProviderFactory.GetFactory( ProviderType.IbmDb2 );
			Assert.IsInstanceOfType( factory, typeof( IbmDb2ConnectionFactory ) );
		}
	}
}