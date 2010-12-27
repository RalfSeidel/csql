using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;

namespace Sqt.DbcProvider.Provider.SqlCe
{
	/// <summary>
	/// Connection wrapper for native MS SQL Server connections.
	/// </summary>
	internal class SqlCeWrappedConnection : WrappedDbConnection
	{
		private readonly SqlCeConnectionFactory connectionFactory;

		internal SqlCeWrappedConnection( SqlCeConnectionFactory connectionFactory, DbConnectionParameter parameter )
			: base( parameter )
		{
			this.connectionFactory = connectionFactory;
		}

		protected override IDbConnection CreateAdoConnection( DbConnectionParameter parameter )
		{
			var adoConnection = connectionFactory.CreateAdoConnection( parameter );
			adoConnection.InfoMessage += new SqlCeInfoMessageEventHandler( InfoMessageHandler );
			adoConnection.Open();
			return adoConnection;
		}

		public override System.Exception GetMappedException( Exception ex )
		{
			SqlCeException sqlceException = ex as SqlCeException;
			if ( sqlceException == null )
				return base.GetMappedException( ex );

			if ( sqlceException.Errors == null || sqlceException.Errors.Count == 0 ) 
				return base.GetMappedException( ex );

			SqlCeError firstError = sqlceException.Errors[0];
			SqlCeMessage message = new SqlCeMessage( firstError );
			SqlCeWrappedException wrappedException = new SqlCeWrappedException( message, ex );
			return wrappedException;
		}

		/// <summary>
		/// Handle for the server messages.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		/// The <see cref="System.Data.SqlClient.SqlInfoMessageEventArgs"/> instance containing the message data.
		/// </param>
		private void InfoMessageHandler( object sender, SqlCeInfoMessageEventArgs e )
		{
			SqlCeErrorCollection errors = e.Errors;
			if ( errors == null || errors.Count == 0 ) {
				DbMessageEventArgs eventArgs = new DbMessageEventArgs( e.Message );
				OnDbMessage( eventArgs );
			}
			else {
				foreach ( SqlCeError error in errors ) {
					TraceLevel severity = GetSeverity( error );
					SqlCeMessage message = new SqlCeMessage( error );
					DbMessageEventArgs eventArgs = new DbMessageEventArgs( message );
					OnDbMessage( eventArgs );
				}
			}
		}

		/// <summary>
		/// Determine the tracelevel of the sql server message.
		/// </summary>
		/// <param name="error">The message data send by the SQL Server.</param>
		/// <returns>The internal trace level for the message.</returns>
		private static TraceLevel GetSeverity( SqlCeError error )
		{
			switch ( error.NativeError ) {
				default:
					return TraceLevel.Info;

			}
		}
	}
}
