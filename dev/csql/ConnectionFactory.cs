using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace csql
{
	static class ConnectionFactory
	{
		public static DbConnection CreateConnection( DbSystem dbSystem, DbDriver dbDriver )
		{
			switch ( dbSystem ) {
				case DbSystem.MsSql:
					return CreateMssqlConnection( dbDriver );
				case DbSystem.MsJet:
					return CreateMsjetConnection( dbDriver );
				case DbSystem.Sybase:
					return CreateSybaseConnection( dbDriver );
				case DbSystem.Oracle:
					return CreateOracleConnection( dbDriver );
				case DbSystem.IbmDb2:
					return CreateIbmdb2Connection( dbDriver );
				default:
					throw new ArgumentException( "Unexpected database system: " + dbSystem, "dbSystem" );
			}
		}

		private static DbConnection CreateMssqlConnection( DbDriver dbDriver )
		{
			if ( dbDriver == DbDriver.Default )
				dbDriver = DbDriver.Native;

			if ( dbDriver != DbDriver.Native )
				throw new ArgumentException( "The driver " + dbDriver + " is not supported for the MS SQL server", "dbDriver" );

			DbConnection cnt = new MsSqlConnection();
			return cnt;
		}

		private static DbConnection CreateMsjetConnection( DbDriver dbDriver )
		{
			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new ArgumentException( "The native driver for MSJET is not available.", "dbDriver" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "dbDriver" );
			}
		}

		private static DbConnection CreateSybaseConnection( DbDriver dbDriver )
		{
			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new NotSupportedException( "TODO" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "dbDriver" );
			}
		}

		private static DbConnection CreateOracleConnection( DbDriver dbDriver )
		{
			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new NotSupportedException( "TODO" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "dbDriver" );
			}
		}

		private static DbConnection CreateIbmdb2Connection( DbDriver dbDriver )
		{
			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new NotSupportedException( "TODO" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "dbDriver" );
			}
		}

	}
}
