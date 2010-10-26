using System;

namespace Sqt.DbcProvider.Provider.IbmDb2
{
	/// <summary>
	/// Connection factory of the IBM DB/2 Database Server.
	/// </summary>
	internal class IbmDb2ConnectionFactory : IDbConnectionFactory
	{

		public string ProviderName
		{
			get { throw new NotImplementedException(); }
		}

		public string GetConnectionString( DbConnectionParameter parameter )
		{
			throw new NotImplementedException();
		}

		public DbConnection CreateConnection( DbConnectionParameter parameter )
		{
			throw new NotImplementedException();
		}
	}
}
