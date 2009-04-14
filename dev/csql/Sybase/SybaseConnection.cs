﻿using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace csql.Sybase
{
	/// <summary>
	/// Specialization of the database connection using the native
	/// Sybase <see cref="T:Sybase.Data.AseClient.AseConnection"/> object.
	/// </summary>
	public class SybaseConnection : DbConnection
	{
		public SybaseConnection( CmdArgs cmdArgs )
			: base( cmdArgs )
		{
		}

		protected override IDbConnection CreateAdoConnection( CmdArgs cmdArgs )
		{
			StringBuilder sb = new StringBuilder();

			if ( cmdArgs.Server.Length != 0 ) {
				sb.Append( "DataSource=" ).Append( cmdArgs.Server ).Append( ";" );
			}
			if ( cmdArgs.ServerPort != 0 ) {
				sb.Append( "Port=" ).Append( cmdArgs.ServerPort ).Append( ";" );
			}
			if ( !String.IsNullOrEmpty( cmdArgs.Database ) ) {
				sb.Append( "Database=" ).Append( cmdArgs.Database ).Append( ";" );
			}
			if ( !String.IsNullOrEmpty( cmdArgs.User ) ) {
				sb.Append( "User ID=" ).Append( cmdArgs.User ).Append( ";" );
				sb.Append( "Password=" ).Append( cmdArgs.Password ).Append( ";" );
			} else {
				sb.Append( "Integrated Security=SSPI;" );
			}
			sb.Append( "Application Name=" ).Append( "csql" ).Append( ";" );
			// The following property is a work around for the 
			// error "30182 Invalid amount of parameters Optionen" sometime
			// raised by the provider if it encounters variable names.
			sb.Append( "NamedParameters=false;" );


			IDbConnection adoConnection = CreateAseConnection();
			string connectionString = sb.ToString();
			adoConnection.ConnectionString = connectionString;
			adoConnection.Open();
			return adoConnection;
		}

		private static string AseAssemblyPath
		{
			get
			{
				string file = @"Sybase.Data.AseClient.dll";
				string root;
				string path;

				// 1. Check sybase environment.
				root = System.Environment.GetEnvironmentVariable( "SYBASE" );
				if ( root == null ) {
					root = System.Environment.GetEnvironmentVariable( "SYBROOT" );
				}
				if ( root == null ) {
					string systemDrive = System.Environment.GetEnvironmentVariable( "SystemDrive" ) + "\\";
					root = Path.Combine( systemDrive, "sybase" );
				}
				if ( root == null || !Directory.Exists( root ) ) {
					return file;
				}

				path = Path.Combine( root, @"DataAccess\ADONET\dll" );
				path = Path.Combine( path, file );

				if ( File.Exists( path ) )
					return path;


				// 2. Check current directory.
				Process thisProcess = System.Diagnostics.Process.GetCurrentProcess();
				string thisPath = thisProcess.MainModule.FileName;
				root = Path.GetDirectoryName( thisPath );
				path = Path.Combine( root, file );
				if ( File.Exists( path ) )
					return path;


				Trace.WriteLineIf( Program.TraceLevel.TraceWarning, "Can't find sybase data provider assembly (" + file + ")." );
				return null;
			}
		}

		/// <summary>
		/// Loads the sybase data provider assembly.
		/// </summary>
		/// <value>The ase assembly.</value>
		private static Assembly AseAssembly
		{
			get
			{
				string assemblyPath = AseAssemblyPath;
				if ( assemblyPath != null ) {
					Assembly assembly = Assembly.LoadFile( assemblyPath );
					return assembly;
				} else {
					// Try to load from GAC
					Assembly assembly = Assembly.Load( "Sybase.Data.AseClient" );
					return assembly;
				}
			}
		}

		/// <summary>
		/// Creates the ADO database connection useing the sybase data provider.
		/// </summary>
		/// <returns>
		/// The database connection connection object.
		/// </returns>
		private IDbConnection CreateAseConnection()
		{
			Assembly assembly = AseAssembly;
			Object connection = assembly.CreateInstance( "Sybase.Data.AseClient.AseConnection" );

			Type       connectionType   = connection.GetType();
			EventInfo  eventInfo        = connectionType.GetEvent( "InfoMessage" );
			Type       eventHandlerType = eventInfo.EventHandlerType;
			Type       thisType         = GetType();
			MethodInfo eventHandler     = thisType.GetMethod( "OnAseConnectionInfoEvent" );

			Delegate d = Delegate.CreateDelegate( eventHandlerType, this, eventHandler, true );

			eventInfo.AddEventHandler( connection, d );

			return (IDbConnection)connection;
		}

		/// <summary>
		/// Called when the sybase data provide raises the info message event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// 
		[SuppressMessage( "Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification="Need public access to be able to access the method info via reflection" )]
		public void OnAseConnectionInfoEvent( object sender, EventArgs e )
		{
			Type eType = e.GetType();
			PropertyInfo property = eType.GetProperty( "Message", typeof( string ) );
			object value = property.GetValue( e, null );
			string message = (string)value;

			property = eType.GetProperty( "Errors" );
			value = property.GetValue( e, null );
			IEnumerable errors = (IEnumerable)value;

			if ( errors == null ) {
				SybaseMessageEventArgs eventArgs = new SybaseMessageEventArgs( message );
				OnDbMessage( eventArgs );
			} else {
				// Sybase sometimes repeats the same message several time.
				// To avoid message spamming we only emit each equal message one time.
				object prevError = null;
				string prevMessage = null;
				foreach ( object error in errors ) {
					if ( error == null )
						continue;

					if ( Object.Equals( prevError, error ) )
						continue;
					prevError = error;


					SybaseError infoMessage = new SybaseError( error );
					if ( Object.Equals( prevMessage, infoMessage.Message ) )
						continue;
					prevMessage = infoMessage.Message;

					SybaseMessageEventArgs eventArgs = new SybaseMessageEventArgs( infoMessage );
					OnDbMessage( eventArgs );

				}
			}
		}


		/// <summary>
		/// Create a statement batch that will just echo the given messages texts.
		/// </summary>
		/// <param name="messages">The message texts.</param>
		/// <returns>
		/// Batch with some print messages.
		/// </returns>
		public override string GetPrintStatements( System.Collections.Generic.IEnumerable<string> messages )
		{
			StringBuilder sb = new StringBuilder();
			foreach ( string message in messages ) {
				sb.Append( "print '" ).Append( message.Replace( "'", "''" ) ).AppendLine( "'" );
			}
			string statement = sb.ToString();
			return statement;
		}

		public override IDataReader Execute( string statement )
		{
			try {
				return base.Execute( statement );
			}
			catch ( Exception e ) {
				Type type = e.GetType();
				if ( type.FullName == "Sybase.Data.AseClient.AseException" ) {
					PropertyInfo property = type.GetProperty( "Errors" );
					object value = property.GetValue( e, null );
					if ( value != null ) {
						IEnumerable errors = (IEnumerable)value;
						IEnumerator enumerator = errors.GetEnumerator();
						if ( enumerator.MoveNext() ) {
							object error = enumerator.Current;
							SybaseError sybaseError = new SybaseError( error );
							SybaseException sybaseException = new SybaseException( sybaseError, e );
							throw sybaseException;
						}
					}
				} 
				throw;
			}
		}
	}
}
