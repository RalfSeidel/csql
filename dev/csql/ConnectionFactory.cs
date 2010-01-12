using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace csql
{
	static class ConnectionFactory
	{
		public static DbConnection CreateConnection( CsqlOptions csqlOptions )
		{
			DbSystem dbSystem = csqlOptions.DbSystem;

			switch ( dbSystem ) {
				case DbSystem.MsSql:
					return CreateMssqlConnection( csqlOptions );
				case DbSystem.MsJet:
					return CreateMsjetConnection( csqlOptions );
				case DbSystem.Sybase:
					return CreateSybaseConnection( csqlOptions );
				case DbSystem.Oracle:
					return CreateOracleConnection( csqlOptions );
				case DbSystem.IbmDb2:
					return CreateIbmdb2Connection( csqlOptions );
				default:
					throw new ArgumentException( "Unexpected database system: " + dbSystem, "csqlOptions" );
			}
		}

		private static DbConnection CreateMssqlConnection( CsqlOptions csqlOptions )
		{
			DbDriver dbDriver = csqlOptions.DbDriver;

			if ( dbDriver == DbDriver.Default )
				dbDriver = DbDriver.Native;

			if ( dbDriver != DbDriver.Native )
				throw new ArgumentException( "The driver " + dbDriver + " is not supported for the MS SQL server", "csqlOptions" );

			DbConnection cnt = new MsSql.MsSqlConnection( csqlOptions );
			return cnt;
		}

		private static DbConnection CreateMsjetConnection( CsqlOptions csqlOptions )
		{
			DbDriver dbDriver = csqlOptions.DbDriver;

			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new ArgumentException( "The native driver for MSJET is not available.", "csqlOptions" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "csqlOptions" );
			}
		}

		private static DbConnection CreateSybaseConnection( CsqlOptions csqlOptions )
		{
			DbDriver dbDriver = csqlOptions.DbDriver;
			DbConnection connection;

			switch ( dbDriver ) {
				case DbDriver.Default:
					goto case DbDriver.Native;
				case DbDriver.Native:
					connection = new Sybase.SybaseConnection( csqlOptions );
					break;
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "csqlOptions" );
			}
			return connection;
		}

		private static DbConnection CreateOracleConnection( CsqlOptions csqlOptions )
		{
			DbDriver dbDriver = csqlOptions.DbDriver;

			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new NotSupportedException( "TODO" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "csqlOptions" );
			}
		}

		private static DbConnection CreateIbmdb2Connection( CsqlOptions csqlOptions )
		{
			DbDriver dbDriver = csqlOptions.DbDriver;

			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new NotSupportedException( "TODO" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "csqlOptions" );
			}
		}

	}
}
