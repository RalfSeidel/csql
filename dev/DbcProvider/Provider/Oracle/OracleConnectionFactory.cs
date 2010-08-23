using System;

namespace Sqt.DbcProvider.Provider.Oracle
{
	/// <summary>
	/// Connection factory of the Oracle Database Server.
	/// </summary>
	internal class OracleConnectionFactory : IDbConnectionFactory
	{
		public DbConnection CreateConnection( DbConnectionParameter parameter )
		{
			throw new NotImplementedException();
		}
	}
}
