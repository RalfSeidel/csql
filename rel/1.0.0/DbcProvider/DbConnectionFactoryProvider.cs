using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sqt.DbcProvider.Provider;
using Sqt.DbcProvider.Provider.IbmDb2;
using Sqt.DbcProvider.Provider.MsSql;
using Sqt.DbcProvider.Provider.Oracle;
using Sqt.DbcProvider.Provider.SqlCe;
using Sqt.DbcProvider.Provider.Sybase;


namespace Sqt.DbcProvider
{
	/// <summary>
	/// Meta factory to provide the factories for supported database provider.
	/// </summary>
	public static class DbConnectionFactoryProvider
	{
		private static readonly IDictionary<ProviderType, IWrappedDbConnectionFactory> provider = new Dictionary<ProviderType, IWrappedDbConnectionFactory>();


		[SuppressMessage( "Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Only way to initialize the collection content." )]
		static DbConnectionFactoryProvider() 
		{
			provider.Add( ProviderType.MsSql, new MsSqlConnectionFactory() );
			provider.Add( ProviderType.SqlCe, new SqlCeConnectionFactory() );
			provider.Add( ProviderType.Sybase, new SybaseConnectionFactory() );
			provider.Add( ProviderType.Oracle, new OracleConnectionFactory() );
			provider.Add( ProviderType.IbmDb2, new IbmDb2ConnectionFactory() );
		}

		/// <summary>
		/// Gets the factory for the specified provider.
		/// </summary>
		public static IWrappedDbConnectionFactory GetFactory( ProviderType providerType )
		{
			if ( providerType == ProviderType.Undefined )
				throw new ArgumentNullException( "providerType" );

			IWrappedDbConnectionFactory factory;
			bool exists = provider.TryGetValue( providerType, out factory );
			if ( !exists ) {
				throw new ArgumentException( "Unsupported database connection provider: " + providerType, "providerType" );
			}

			return factory;
		}

		/// <summary>
		/// Creates a new open database connection.
		/// </summary>
		public static WrappedDbConnection CreateConnection( DbConnectionParameter parameter )
		{
			var factory = GetFactory( parameter.Provider );
			var connection = factory.CreateConnection( parameter );
			return connection;
		}
	}
}
