using System.Data.SqlClient;
using System;

namespace Sqt.DbcProvider.Provider
{
	/// <summary>
	/// Connection factory of the Microsoft SQL Server.
	/// </summary>
	internal class MsSqlConnectionFactory : IDbcProvider
	{
		/// <inheritdoc/>
		public System.Data.IDbConnection CreateConnection( DbConnectionParameter parameter )
		{
			SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();

			if ( !String.IsNullOrEmpty( parameter.DatasourceAddress ) )
				connectionStringBuilder.DataSource = parameter.DatasourceAddress;

			if ( !String.IsNullOrEmpty( parameter.Catalog ) )
				connectionStringBuilder.InitialCatalog = parameter.Catalog;

			connectionStringBuilder.IntegratedSecurity = parameter.IntegratedSecurity;

			if ( !String.IsNullOrEmpty( parameter.UserId ) ) {
				connectionStringBuilder.UserID = parameter.UserId;
				connectionStringBuilder.Password = parameter.Password ?? "";
			}

			if ( parameter.Timeout > 0 ) {
				connectionStringBuilder.ConnectTimeout = parameter.Timeout;
			}

			string connectionString = connectionStringBuilder.ConnectionString;
			SqlConnection connection = new SqlConnection( connectionString );
			return connection;
		}
	}
}
