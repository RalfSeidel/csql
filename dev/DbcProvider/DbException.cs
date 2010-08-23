using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// An wrapper for exceptions raised by the different providers.
	/// </summary>
	[Serializable]
	public class DbException : System.Data.Common.DbException
	{
		/// <summary>
		/// The message encapsulated by this exception type.
		/// </summary>
		private readonly DbMessage message;

		public DbException()
		{
		}

		public DbException( string message )
			: base( message )
		{
			this.message = new DbMessage( message );
		}

		public DbException( string message, Exception innerException )
			: base( message, innerException )
		{
			this.message = new DbMessage( message );
		}

		protected DbException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
			: base( info, context ) 
		{ 
		}

		public DbException( DbMessage message )
			: base( message.Message )
		{
			this.message = message;
		}

		public DbException( DbMessage message, Exception innerException )
			: base( message.Message, innerException )
		{
			this.message = message;
		}

		/// <summary>
		/// The name of the server/datasource where the error occurred.
		/// </summary>
		public string Server
		{
			get { return this.message.Server; }
		}

		public string Catalog
		{
			get { return this.message.Catalog; }
		}

		/// <summary>
		/// The name of the procedure, function or trigger that raised the error.
		/// </summary>
		public string Procedure
		{
			get { return this.message.Procedure; }
		}

		/// <summary>
		/// The line number of the current batch or the line numer of the procedure
		/// at that the error was raised.
		/// </summary>
		public int LineNumber
		{
			get { return this.message.LineNumber; }
		}

		/// <summary>
		/// Gets the message text.
		/// </summary>
		/// <value>The message text.</value>
		public override string Message
		{
			get { return this.message.Message; }
		}

		/// <summary>
		/// Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is a null reference-.
		/// </exception>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
		/// </PermissionSet>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
			base.GetObjectData( info, context );

            info.AddValue( "Message", this.message );
        }
	}

}

