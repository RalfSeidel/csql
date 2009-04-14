using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace csql
{
	static class ConnectionFactory
	{
		public static DbConnection CreateConnection( CmdArgs cmdArgs )
		{
			DbSystem dbSystem = cmdArgs.System;

			switch ( dbSystem ) {
				case DbSystem.MsSql:
					return CreateMssqlConnection( cmdArgs );
				case DbSystem.MsJet:
					return CreateMsjetConnection( cmdArgs );
				case DbSystem.Sybase:
					return CreateSybaseConnection( cmdArgs );
				case DbSystem.Oracle:
					return CreateOracleConnection( cmdArgs );
				case DbSystem.IbmDb2:
					return CreateIbmdb2Connection( cmdArgs );
				default:
					throw new ArgumentException( "Unexpected database system: " + dbSystem, "cmdArgs" );
			}
		}

		private static DbConnection CreateMssqlConnection( CmdArgs cmdArgs )
		{
			DbDriver dbDriver = cmdArgs.Driver;

			if ( dbDriver == DbDriver.Default )
				dbDriver = DbDriver.Native;

			if ( dbDriver != DbDriver.Native )
				throw new ArgumentException( "The driver " + dbDriver + " is not supported for the MS SQL server", "cmdArgs" );

			DbConnection cnt = new MsSql.MsSqlConnection( cmdArgs );
			return cnt;
		}

		private static DbConnection CreateMsjetConnection( CmdArgs cmdArgs )
		{
			DbDriver dbDriver = cmdArgs.Driver;

			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new ArgumentException( "The native driver for MSJET is not available.", "cmdArgs" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "cmdArgs" );
			}
		}

		private static DbConnection CreateSybaseConnection( CmdArgs cmdArgs )
		{
			DbDriver dbDriver = cmdArgs.Driver;
			DbConnection connection;

			switch ( dbDriver ) {
				case DbDriver.Default:
					goto case DbDriver.Native;
				case DbDriver.Native:
					connection = new Sybase.SybaseConnection( cmdArgs );
					break;
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "cmdArgs" );
			}
			return connection;
		}

		private static DbConnection CreateOracleConnection( CmdArgs cmdArgs )
		{
			DbDriver dbDriver = cmdArgs.Driver;

			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new NotSupportedException( "TODO" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "cmdArgs" );
			}
		}

		private static DbConnection CreateIbmdb2Connection( CmdArgs cmdArgs )
		{
			DbDriver dbDriver = cmdArgs.Driver;

			switch ( dbDriver ) {
				case DbDriver.Native:
					throw new NotSupportedException( "TODO" );
				case DbDriver.OleDb:
					throw new NotSupportedException( "TODO" );
				default:
					throw new ArgumentException( "Unexpected database driver: " + dbDriver, "cmdArgs" );
			}
		}

	}
}
