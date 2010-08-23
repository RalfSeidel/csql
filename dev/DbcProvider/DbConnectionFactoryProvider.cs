using System;
using System.Collections.Generic;
using Sqt.DbcProvider.Provider;
using Sqt.DbcProvider.Provider.Sybase;
using Sqt.DbcProvider.Provider.MsSql;
using Sqt.DbcProvider.Provider.Oracle;
using Sqt.DbcProvider.Provider.IbmDb2;
using System.Diagnostics.CodeAnalysis;


namespace Sqt.DbcProvider
{
	/// <summary>
	/// Meta factory to provide the factories for supported database provider.
	/// </summary>
	public static class DbConnectionFactoryProvider
	{
		private static readonly IDictionary<ProviderType, IDbConnectionFactory> provider = new Dictionary<ProviderType, IDbConnectionFactory>();


		[SuppressMessage( "Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Only way to initialize the collection content." )]
		static DbConnectionFactoryProvider() 
		{
			provider.Add( ProviderType.MsSql, new MsSqlConnectionFactory() );
			provider.Add( ProviderType.Sybase, new SybaseConnectionFactory() );
			provider.Add( ProviderType.Oracle, new OracleConnectionFactory() );
			provider.Add( ProviderType.IbmDb2, new IbmDb2ConnectionFactory() );
		}

		/// <summary>
		/// Gets the factory for the specified provider.
		/// </summary>
		public static IDbConnectionFactory GetFactory( ProviderType providerType )
		{
			if ( providerType == ProviderType.Undefined )
				throw new ArgumentNullException( "providerType" );

			IDbConnectionFactory factory;
			bool exists = provider.TryGetValue( providerType, out factory );
			if ( !exists ) {
				throw new ArgumentException( "Unsupported database connection provider: " + providerType, "providerType" );
			}

			return factory;
		}

		/// <summary>
		/// Creates a new open database connection.
		/// </summary>
		public static DbConnection CreateConnection( DbConnectionParameter parameter )
		{
			var factory = GetFactory( parameter.Provider );
			var connection = factory.CreateConnection( parameter );
			return connection;
		}
	}
}
