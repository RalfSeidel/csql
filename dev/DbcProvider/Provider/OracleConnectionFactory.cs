using System;

namespace Sqt.DbcProvider.Provider
{
	/// <summary>
	/// Connection factory of the Oracle Database Server.
	/// </summary>
	internal class OracleConnectionFactory : IDbcProvider
	{
		public System.Data.IDbConnection CreateConnection( DbConnectionParameter parameter )
		{
			throw new NotImplementedException();
		}
	}
}
