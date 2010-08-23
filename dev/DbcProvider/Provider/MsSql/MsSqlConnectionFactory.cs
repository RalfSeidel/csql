using System.Data.SqlClient;
using System;
using System.Diagnostics;

namespace Sqt.DbcProvider.Provider.MsSql
{
	/// <summary>
	/// Connection factory of the Microsoft SQL Server.
	/// </summary>
	internal class MsSqlConnectionFactory : IDbConnectionFactory
	{
		/// <inheritdoc/>
		public DbConnection CreateConnection( DbConnectionParameter parameter )
		{
			var connection = new MsSqlConnection( this, parameter );
			return connection;
		}

		/// <summary>
		/// Creates the ADO.NET connection.
		/// </summary>
		/// <param name="parameter">The connection parameter.</param>
		/// <returns>An open ADO connection.</returns>
		internal static SqlConnection CreateAdoConnection( DbConnectionParameter parameter )
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
			Trace.WriteLineIf( parameter.VerbositySwitch.TraceVerbose, "Connecting to MS SQL Server using following connection string:\r\n" + connectionString );

			SqlConnection connection = new SqlConnection( connectionString );
			return connection;
		}
	}
}
