using System;

namespace Sqt.DbcProvider.Provider.Oracle
{
	/// <summary>
	/// Connection factory of the Oracle Database Server.
	/// </summary>
	internal class OracleConnectionFactory : IWrappedDbConnectionFactory
	{
		public string ProviderName
		{
			get { throw new NotImplementedException(); }
		}

		public string GetConnectionString( DbConnectionParameter parameter )
		{
			throw new NotImplementedException();
		}

		public WrappedDbConnection CreateConnection( DbConnectionParameter parameter )
		{
			throw new NotImplementedException();
		}
	}
}
