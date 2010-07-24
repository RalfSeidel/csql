using System;
using System.Runtime.Serialization;

namespace csql
{
	/// <summary>
	/// Exception throw if the program has to terminate.
	/// </summary>
	/// <remarks>
	/// The exception is thrown when another error condition has
	/// been handled an the only thing to do is to end the program.
	/// </remarks>
	[Serializable]
	public sealed class TerminateException : Exception
	{
		private readonly ExitCode exitCode;

		/// <summary>
		/// Initializes a new instance of the <see cref="TerminateException"/> class.
		/// </summary>
		/// <param name="exitCode">The exit code for the application.</param>
		public TerminateException( ExitCode exitCode )
		{
			this.exitCode = exitCode;
		}

		/// <summary>
		/// Just implemented to make code analysis happy.
		/// </summary>
		[Obsolete( "Always use the constructor TerminateException( ExitCode )" )]
		public TerminateException()
		: base( "Program Termination" )
		{
			exitCode = ExitCode.GeneralError;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TerminateException"/> 
		/// class with the specified message.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		[Obsolete( "Always use the constructor TerminateException( ExitCode )" )]
		public TerminateException( string message )
		: base( message ) 
		{
			exitCode = ExitCode.GeneralError;
		}

		/// <summary>
		/// Initializes a new instance of the this class with the specified
		/// error message and a reference to the inner exception that is the cause of
		/// this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or <c>null</c>
		/// if no inner exception is specified.
		/// </param>
		[Obsolete( "Always use the constructor TerminateException( ExitCode )" )]
		public TerminateException( string message, Exception innerException )
		: base( message, innerException )
		{
            exitCode = ExitCode.GeneralError;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="TerminateException"/> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		[Obsolete( "Always use the constructor TerminateException( ExitCode )" )]
		private TerminateException( SerializationInfo info, StreamingContext context )
		: base( info, context )
		{
		}
					
					
		/// <summary>
		/// Gets the exit code to be returned by the program.
		/// </summary>
		/// <value>The exit code.</value>
		public ExitCode ExitCode
		{
			get { return exitCode; }
		}

		/// <summary>
		/// Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). 
		/// </exception>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
		/// </PermissionSet>
		[System.Security.Permissions.SecurityPermission( System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter )]
		public override void GetObjectData( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
		{
			base.GetObjectData( info, context );
			info.AddValue( "ExitCode", exitCode );
		}
	}
}
