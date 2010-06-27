using System;

namespace Sqt.DbcProvider.Provider
{
	/// <summary>
	/// Connection factory of the IBM DB/2 Database Server.
	/// </summary>
	internal class IbmDb2ConnectionFactory : IDbcProvider
	{
		public System.Data.IDbConnection CreateConnection( DbConnectionParameter parameter )
		{
			throw new NotImplementedException();
		}
	}
}
