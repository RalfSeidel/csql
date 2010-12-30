using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;

namespace Sqt.DbcProvider.Provider.SqlCe
{
	/// <summary>
	/// Connection factory of the Microsoft SQL CE local database files.
	/// </summary>
	internal class SqlCeConnectionFactory : IWrappedDbConnectionFactory
	{
		/// <summary>
		/// Gets the name of the provider as used by the ADO provider/factory model.
		/// </summary>
		public string ProviderName
		{
			get { return "Microsoft.SqlServerCe.Client.3.5"; }
		}

		/// <inheritdoc/>
		public string GetConnectionString( DbConnectionParameter parameter )
		{
			DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();
			string dataSource = string.Empty;

			if ( !string.IsNullOrEmpty( parameter.DatasourceAddress ) ) {
				dataSource = parameter.DatasourceAddress;
			}
			if ( !string.IsNullOrEmpty( parameter.Catalog ) ) {
				dataSource = Path.Combine( dataSource, parameter.Catalog );
			}

			if ( !string.IsNullOrEmpty( dataSource ) ) {
				connectionStringBuilder.Add( "Data Source", dataSource );
			}

			return connectionStringBuilder.ConnectionString;
		}

		/// <inheritdoc/>
		public WrappedDbConnection CreateConnection( DbConnectionParameter parameter )
		{
			var connection = new SqlCeWrappedConnection( this, parameter );
			return connection;
		}

		/// <summary>
		/// Creates the ADO.NET connection.
		/// </summary>
		/// <param name="parameter">The connection parameter.</param>
		/// <returns>An open ADO connection.</returns>
		internal System.Data.SqlServerCe.SqlCeConnection CreateAdoConnection( DbConnectionParameter parameter )
		{
			string connectionString = GetConnectionString( parameter );
			Trace.WriteLineIf( parameter.VerbositySwitch.TraceVerbose, "Connecting to SQL CE database using following connection string:\r\n" + connectionString );

			System.Data.SqlServerCe.SqlCeConnection connection = new System.Data.SqlServerCe.SqlCeConnection( connectionString );
			return connection;
		}
	}
}
