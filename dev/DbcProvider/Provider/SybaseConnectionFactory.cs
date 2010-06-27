using System;

namespace Sqt.DbcProvider.Provider
{
	/// <summary>
	/// Connection factory of the Sybase ASE SQL Server.
	/// </summary>
	internal class SybaseConnectionFactory : IDbcProvider
	{
		public System.Data.IDbConnection CreateConnection( DbConnectionParameter parameter )
		{
			throw new NotImplementedException();
		}
	}
}
