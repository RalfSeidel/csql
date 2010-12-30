using System;
using System.Text;

namespace Sqt.DbcProvider.Provider.Sybase
{
	/// <summary>
	/// Connection factory of the Sybase ASE SQL Server.
	/// </summary>
	internal class SybaseConnectionFactory : IWrappedDbConnectionFactory
	{
		public string ProviderName
		{
			get { return "Sybase.Data.AseClient"; }
		}

		public string GetConnectionString( DbConnectionParameter parameter )
		{
			StringBuilder sb = new StringBuilder();

			if ( !String.IsNullOrEmpty( parameter.DatasourceAddress ) ) {
				sb.Append( "DataSource=" ).Append( parameter.DatasourceAddress ).Append( ";" );
			}
			if ( parameter.DatasourcePort != 0 ) {
				sb.Append( "Port=" ).Append( parameter.DatasourcePort ).Append( ";" );
			}
			if ( !String.IsNullOrEmpty( parameter.Catalog ) ) {
				sb.Append( "Database=" ).Append( parameter.Catalog ).Append( ";" );
			}
			if ( !String.IsNullOrEmpty( parameter.UserId ) ) {
				sb.Append( "User ID=" ).Append( parameter.UserId ).Append( ";" );
				sb.Append( "Password=" ).Append( parameter.Password ).Append( ";" );
			}
			else {
				sb.Append( "Integrated Security=SSPI;" );
			}
			if ( !String.IsNullOrEmpty( parameter.ApplicationName ) ) {
				sb.Append( "Application Name=" ).Append( parameter.ApplicationName ).Append( ";" );
			}
			// The following property is a work around for the 
			// error "30182 Invalid amount of parameters Optionen" sometime
			// raised by the provider if it encounters variable names.
			sb.Append( "NamedParameters=false;" );

			string connectionString = sb.ToString();
			return connectionString;
		}

		public WrappedDbConnection CreateConnection( DbConnectionParameter parameter )
		{
			WrappedDbConnection connection = new SybaseConnection( this, parameter );
			return connection;
		}

	}
}
