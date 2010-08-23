using System;

namespace Sqt.DbcProvider.Provider.Sybase
{
	/// <summary>
	/// Connection factory of the Sybase ASE SQL Server.
	/// </summary>
	internal class SybaseConnectionFactory : IDbConnectionFactory
	{
		public DbConnection CreateConnection( DbConnectionParameter parameter )
		{
			DbConnection connection = new SybaseConnection( this, parameter );
			return connection;
		}
	}
}
